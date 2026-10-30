# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All `dotnet` commands must be run from the `./src` directory.

```bash
dotnet restore
dotnet build --no-restore -warnaserror
dotnet format --verify-no-changes             # check code style (CI enforces this)
dotnet format                                 # auto-fix code style
dotnet test --no-build --verbosity normal     # run xUnit tests
dotnet tool restore && dotnet stryker --mutation-level Complete --break-at 100
dotnet pack --configuration Release -p:Version=<version> --output .
```

CI additionally passes `-p:AssemblyVersion` (pinned to the major) and `-p:FileVersion`; see `.github/workflows/publish-nuget.yml`. Stryker leaves a mutated assembly in `bin/`, so rebuild before running `dotnet test --no-build` after it.

Until `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions` is published, restore needs a local feed with it: pack that repo with `dotnet pack -c Release -p:Version=0.1.0-preview.0.2.0 -o <feed>` and run `dotnet restore -s <feed> -s https://api.nuget.org/v3/index.json`. Do not commit a `nuget.config` or the feed; CI fails at restore until the package is on NuGet.org.

## Architecture

This package implements `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions`: one `sealed record` per PureQL literal (`PureQL.CSharp.Model.Literals.LiteralInteger` … `LiteralUuidNullable`) and per list literal (`PureQL.CSharp.Model.Lists.ListLiteralInteger` … `ListLiteralUuid`), so a query parameter can be built from Pure primitives and turned into a literal by the code that runs the query. It has no public abstractions of its own.

**Records:** one folder and namespace per abstraction, like `Pure.RelationalSchema` (`.Column`, `.ColumnType`):

- `ParameterType/`: `IntegerParameterType`, `DecimalParameterType`, `StringParameterType`, `BooleanParameterType`, `DateParameterType`, `TimeParameterType`, `DateTimeParameterType`, `UuidParameterType` and a `Nullable…ParameterType` for each. `Name` is the PureQL type name copied from `PureQL.CSharp.Model.Types` (`integer`, `decimal`, `string`, `boolean`, `date`, `time`, `datetime`, `uuid`); a nullable type has its non-null type's name (`TypeIntegerNullable.Name` is `integer`) and `Nullable` true.
- `Parameter/`: `IntegerParameter(INumber<long>)`, `DecimalParameter(INumber<decimal>)`, `StringParameter(IString)`, `BooleanParameter(IBool)`, `DateParameter(IDate)`, `TimeParameter(ITime)`, `DateTimeParameter(IDateTime)`, `UuidParameter(IGuid)`, and a parameterless `Nullable…Parameter` for each type, whose `Value` is `EmptyString`. `Type` is the matching type record.
- `Parameter/` also holds the relative parameters: `RelativeDateParameter(INumber<long> days)` and `RelativeDateParameter(IString)`, `RelativeTimeParameter(IString)` and `RelativeDateTimeParameter(IString)`. `Value` is date math, the relative values of Elasticsearch and Grafana (`now-10d`, `now-1w/w`, `now-30m`), and `Type` the non-null type of the value it gives. The `IString` is stored as given, as `StringParameter` stores it; the days are written `now` + sign + invariant number + `d` (`now-10d`, `now+3d`, `now+0d`) through `CellText`'s `Lazy<string>` constructor. This package never evaluates date math: `Pure.RelationalSchema.Storage.PureQL.Projection` does, against the UTC instant the query runs. The grammar the README documents (`now`, then `+N`/`-N` of a unit or `/unit` rounding, applied left to right; `y M w d h m s`, case-sensitive, weeks from Monday; a date takes `y M w d`, a time `h m s` and wraps around midnight, a datetime all) is the contract with that evaluator: change them together.
- `ListParameter/`: `IntegerListParameter(IEnumerable<INumber<long>>)`, `DecimalListParameter(IEnumerable<INumber<decimal>>)`, `StringListParameter(IEnumerable<IString>)`, `BooleanListParameter(IEnumerable<IBool>)`, `DateListParameter(IEnumerable<IDate>)`, `TimeListParameter(IEnumerable<ITime>)`, `DateTimeListParameter(IEnumerable<IDateTime>)` and `UuidListParameter(IEnumerable<IGuid>)`, implementing `IListParameter`. `Type` is the matching `…ListParameterType` in `ParameterType/`, whose `Name` is the list type name copied from `PureQL.CSharp.Model.Types` (`integerList`, `decimalList`, `stringList`, `booleanList`, `dateList`, `timeList`, `datetimeList`, `uuidList`) and whose `Nullable` is false: a PureQL list type is never nullable. `Items` wraps each value in the scalar record of its type (`IntegerParameter` …), so a PureQL list, which holds no `null`, has no typed-null item. The constructor stores `values.Select(…)`: the sequence is neither enumerated nor copied, as in the `Pure.RelationalSchema` records, and each enumeration of `Items` enumerates it again.
- `ListText` (internal): the `Value` of a list parameter, informational since the projection reads `Items`: a JSON array of the items' `Value` texts, each as a JSON string (`["paid","shipped"]`, `[]` for an empty list). It holds a `Lazy<string>` like `CellText`, so the items are read on the first read of the text, once. It writes with `Utf8JsonWriter` and `JavaScriptEncoder.UnsafeRelaxedJsonEscaping`, never the reflection-based `JsonSerializer` (AOT): `\"`, `\\` and the short escapes, `\uXXXX` for other control characters, characters outside the Basic Multilingual Plane and a few invisible ones, the rest as is; a lone surrogate becomes U+FFFD. The writer is disposed, which flushes it, in its own method rather than flushed explicitly, so no `Flush()` statement mutant survives.
- `CellText` (internal): the `IString` of a non-string value. Like `Pure.Primitives.String.String`, it holds a `Lazy<string>` over the abstraction, so a constructor never reads the value; it is read on the first `TextValue` or enumeration and kept. A `StringParameter`'s value is the given `IString` itself.

Every record, `CellText` and `ListText` throw `NotSupportedException` from `GetHashCode` and `ToString`, as in the other Pure records.

**The cell text contract:** except for the relative parameters, `Value` must be exactly what the projection writes for a result cell (`CellText.cs` in `Pure.RelationalSchema.Storage.PureQL.Projection`) and what its `StoredColumn.cs` parses back with `InvariantCulture`. If the projection changes the format, change `CellText` here, its tests, the README and the Abstractions' XML docs together.

- integer: `long.ToString(InvariantCulture)`
- decimal: `(value / 1.0000000000000000000000000000m).ToString(InvariantCulture)`. Dividing by a one with 28 decimal places drops the scale, so there are no trailing zeros, no exponent and no `-0`. `One` is a static property, not a `const`: coverlet reports the static-constructor line of a `const decimal` as never hit.
- boolean: `bool.TrueString` / `bool.FalseString`
- date: a `DateOnly` from the parts, `yyyy-MM-dd`
- time and datetime: a UTC `DateTime` from the parts plus `Nanosecond / TimeSpan.NanosecondsPerTick` ticks, formatted `HH:mm:ss.FFFFFFF` (through `TimeOnly.FromDateTime`) or `yyyy-MM-ddTHH:mm:ss.FFFFFFF`. The fraction appears only when non-zero; nanoseconds below a tick are dropped. Invalid parts throw `ArgumentOutOfRangeException` when the text is read.
- uuid: `Guid.ToString("D")`, lower case. Its `// Stryker disable once String` is there because the empty-format mutant is the same `D` format.
- typed null: the empty string

Do not use the `Pure.Primitives` `String(IDate)`/`String(ITime)`/`String(IDateTime)` constructors: they write `M/d/yyyy` and `H:m:s.ms.us`, which the projection cannot parse. `Pure.RelationalSchema.Storage.Samples`' `InvariantCellText` is a related precedent, but it drops fractional seconds and has no decimals.

**Dependencies:** the library references only `Pure.Primitives` 3.7.0 and `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions` 0.1.0-preview.0.2.0 (for `IListParameter`); `Pure.Primitives.Abstractions` 4.3.0 comes transitively through both. Do not add direct references to packages that already come transitively, and do not reference `PureQL.CSharp.Model` or the projection: in XML docs, name their types with `<c>`, not `<see cref>`. Converting a parameter to an `ILiteral` belongs to the code that runs the query (the README shows it).

**Testing:** the tests mirror the folders (`Parameter/`, `ListParameter/`, `ParameterType/`), one test record per library record.

- Relative parameter tests assert the type, that the expression is the given `IString` itself (`Assert.Same`) and not read (`Fakes/UnreadText`), the days text over signs, zero and the `long` extremes under a non-invariant culture, and that the days are read only when the text is.
- Type tests assert `Name`, `Nullable` and the throwing `GetHashCode`/`ToString`.
- Parameter tests assert the `Type` record, the exact `Value` text over edge cases (signs and extremes, decimal scale and 28-digit fractions, Unicode and empty strings, date range, midnight, milliseconds, microseconds, ticks and nanoseconds from `Time`'s parts, uuid case), the same text under a non-invariant `CultureInfo.CurrentCulture` (`Fakes/Characters.InCulture`), and that the text parses back to the value with the same `X.Parse(text, …, CultureInfo.InvariantCulture)` calls `StoredColumn.cs` uses.
- `Fakes/UnreadValue` implements every value abstraction and throws `ValueReadException` on any read, proving a constructor and `Type` do not read the value and `Value.TextValue` does. `Fakes/InvalidDate` is February 30th, which `Pure.Primitives.Date.Date` would refuse.
- Enumeration is checked generically and non-generically (`Fakes/Characters`).
- List parameter tests assert the list `Type`, that `Items` are the scalar records in order with repeats, the exact `Value` JSON (string escaping: quotes, backslashes, control characters, non-ASCII, surrogate pairs, U+2028/U+2029, a lone surrogate; `[]` for an empty list), that `JsonDocument` parses it back to the items' texts and those to the values, and laziness: `Fakes/UnreadValues` throws when enumerated (the constructor and `Type` do not enumerate), `Fakes/UnreadValue` and `Fakes/UnreadText` show `Items` does not read the values, and `Fakes/CountedValues` shows `Value` enumerates them once and each `Items` enumeration once more. Write `\u` escapes in tests as C# escapes or raw-string text, never as the raw characters.

CI runs mutation testing via `dotnet-stryker` at `Complete` level with a 100% break threshold (measured: 100%, 66 of 66 tested mutants killed, 214 compile errors, 5 ignored) and requires >=99% line coverage (warning at 99%; measured: 100% line and branch).

**CI style check:** the `Code style check by csharpier` step in `.github/workflows/build-and-test.yml` is commented out on purpose. csharpier's default width of 100 conflicts with the 90-column limit, and `dotnet format` is the authoritative check.

**Multi-targeting:** net8.0, net9.0, net10.0. `IsAotCompatible` is `true` and the build must stay free of IL2026/IL3050 warnings.

**Documentation:** `GenerateDocumentationFile` is on, so the XML docs ship in the package and a public member without a doc comment fails the build (CS1591 under `-warnaserror`).

**Package validation:** `EnablePackageValidation = true`. `PackageValidationBaselineVersion` is the last published version (`0.1.0-preview.0.1.0`); bump it after each release so breaking API changes fail the build.

**Publishing:** triggered by pushing a semver tag matching `*.*.*`. The tag name becomes the `PackageVersion`. Packages are published to both GitHub Packages and NuGet.org.

## Code Style

Enforced via `.editorconfig` and `dotnet format --verify-no-changes` in CI:

- No `var` — always use explicit types
- No expression-bodied methods or constructors — properties, indexers, accessors, and lambdas may be expression-bodied
- File-scoped namespace declarations; namespaces match folders
- Private fields prefixed with `_` (underscore camelCase)
- No non-private instance fields
- `using` directives outside the namespace, System directives sorted first
- Max line length: 90 characters. `dotnet format` does not enforce it, so check new code yourself. The namespace declarations and the `using` directives of this package's own namespaces are longer and cannot be wrapped; that is the only exception
- Braces on new lines (`csharp_new_line_before_open_brace = all`)
- No implicit object creation (`new()` without type) when the type is apparent
- Avoid partial `switch`es: the `dotnet format` "populate switch" fixes (IDE0010/IDE0072) insert `throw new NotImplementedException()` arms

## Commit Messages

Do not mention Claude or AI assistance in commit messages.
