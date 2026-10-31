# Pure.RelationalSchema.Storage.PureQL.Projection.Parameters

Query parameters for `Pure.RelationalSchema.Storage.PureQL.Projection` — sealed records implementing `IParameter`, `IListParameter` and `IParameterType` for every PureQL literal and list literal, with the bound value as invariant cell text.

[![.NET build & test](https://github.com/kudima03/Pure.RelationalSchema.Storage.PureQL.Projection.Parameters/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.RelationalSchema.Storage.PureQL.Projection.Parameters/actions/workflows/build-and-test.yml)
[![publish NuGet](https://github.com/kudima03/Pure.RelationalSchema.Storage.PureQL.Projection.Parameters/actions/workflows/publish-nuget.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.RelationalSchema.Storage.PureQL.Projection.Parameters/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/Pure.RelationalSchema.Storage.PureQL.Projection.Parameters)](https://www.nuget.org/packages/Pure.RelationalSchema.Storage.PureQL.Projection.Parameters)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

A PureQL query can read named parameters, and `Pure.RelationalSchema.Storage.PureQL.Projection` binds them by name to PureQL literals: `PureQL.CSharp.Model.Literals.ILiteral` and its 16 records, from `LiteralInteger(long)` to `LiteralUuidNullable()`, one with a value and one typed null for each of the eight PureQL types. A list parameter, which only `in` reads, is bound to a list literal: `ListLiteralInteger(IEnumerable<long>)` to `ListLiteralUuid(IEnumerable<Guid>)` in `PureQL.CSharp.Model.Lists`.

`Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions` describes the Pure-style counterpart of a literal: an `IParameter` with its `IParameterType` and its value as the text a stored cell holds. Its `IListParameter` is the counterpart of a list literal: a list type and the items, each a parameter of the item type. This package implements those interfaces with one sealed record per literal and per list literal, built from `Pure.Primitives` abstractions only. It does not depend on `PureQL.CSharp.Model` or the projection, so a domain layer can build, store or send parameters without them, and AOT-compiled code can use it.

- **Exact cell text.** Except for the current and offset parameters, `Value` is in the format the projection writes result cells (`CellText`) and parses stored cells (`StoredColumn`), so the projection's own `X.Parse(text, CultureInfo.InvariantCulture)` calls read it back to the original value.
- **Invariant.** The text does not depend on the current culture.
- **Lazy.** A constructor does not read the value. It is read when `Value.TextValue`, or its characters, is first read, as `Pure.Primitives.String.String` does.
- **Typed nulls.** Each type has a parameterless `Nullable…Parameter` whose value is the empty string, the storage layer's NULL, and whose type has `Nullable` true.
- **Lists.** Each type has a `…ListParameter` whose items are the scalar parameters of its values, and whose value is a JSON array of their texts, written without reflection.
- **Current and offset dates and times.** `CurrentDateParameter`, `DateOffsetParameter` and their time and datetime counterparts hold date math such as `now` or `now-10d`, which the projection evaluates against the instant the query runs.

## Installation

```
dotnet add package Pure.RelationalSchema.Storage.PureQL.Projection.Parameters
```

The interfaces come with it from `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions`, and the primitives from `Pure.Primitives` through `Pure.Primitives.String.Operations`.

## Parameters

The parameters live in `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter`, the list parameters in `….Parameters.ListParameter` and the types in `….Parameters.ParameterType`, one folder and namespace per abstraction, as in `Pure.RelationalSchema`.

| PureQL literal | Parameter | Type | `Name` | `Nullable` | `Value` |
|----------------|-----------|------|--------|------------|---------|
| `LiteralInteger(long)` | `IntegerParameter(INumber<long>)` | `IntegerParameterType` | `integer` | `False` | `-42` |
| `LiteralDecimal(decimal)` | `DecimalParameter(INumber<decimal>)` | `DecimalParameterType` | `decimal` | `False` | `1.5` for `1.50m`: exact, no exponent, no trailing zeros in the fraction |
| `LiteralString(string)` | `StringParameter(IString)` | `StringParameterType` | `string` | `False` | the string itself |
| `LiteralBoolean(bool)` | `BooleanParameter(IBool)` | `BooleanParameterType` | `boolean` | `False` | `True` or `False` |
| `LiteralDate(DateOnly)` | `DateParameter(IDate)` | `DateParameterType` | `date` | `False` | `2026-10-29` (`yyyy-MM-dd`) |
| `LiteralTime(TimeOnly)` | `TimeParameter(ITime)` | `TimeParameterType` | `time` | `False` | `08:30:00`, `08:30:00.25` (`HH:mm:ss.FFFFFFF`) |
| `LiteralDatetime(DateTimeOffset)` | `DateTimeParameter(IDateTime)` | `DateTimeParameterType` | `datetime` | `False` | `2026-10-29T08:30:00` (`yyyy-MM-ddTHH:mm:ss.FFFFFFF`, a UTC instant without an offset) |
| `LiteralUuid(Guid)` | `UuidParameter(IGuid)` | `UuidParameterType` | `uuid` | `False` | `3f2504e0-4f89-11d3-9a0c-0305e82c3301` (lower case, `D` format) |
| `LiteralIntegerNullable()` | `NullableIntegerParameter()` | `NullableIntegerParameterType` | `integer` | `True` | empty |
| `LiteralDecimalNullable()` | `NullableDecimalParameter()` | `NullableDecimalParameterType` | `decimal` | `True` | empty |
| `LiteralStringNullable()` | `NullableStringParameter()` | `NullableStringParameterType` | `string` | `True` | empty |
| `LiteralBooleanNullable()` | `NullableBooleanParameter()` | `NullableBooleanParameterType` | `boolean` | `True` | empty |
| `LiteralDateNullable()` | `NullableDateParameter()` | `NullableDateParameterType` | `date` | `True` | empty |
| `LiteralTimeNullable()` | `NullableTimeParameter()` | `NullableTimeParameterType` | `time` | `True` | empty |
| `LiteralDatetimeNullable()` | `NullableDateTimeParameter()` | `NullableDateTimeParameterType` | `datetime` | `True` | empty |
| `LiteralUuidNullable()` | `NullableUuidParameter()` | `NullableUuidParameterType` | `uuid` | `True` | empty |
| `ListLiteralInteger(IEnumerable<long>)` | `IntegerListParameter(IEnumerable<INumber<long>>)` | `IntegerListParameterType` | `integerList` | `False` | `["1","-42"]` |
| `ListLiteralDecimal(IEnumerable<decimal>)` | `DecimalListParameter(IEnumerable<INumber<decimal>>)` | `DecimalListParameterType` | `decimalList` | `False` | `["1","2.5"]` |
| `ListLiteralString(IEnumerable<string>)` | `StringListParameter(IEnumerable<IString>)` | `StringListParameterType` | `stringList` | `False` | `["paid","shipped"]` |
| `ListLiteralBoolean(IEnumerable<bool>)` | `BooleanListParameter(IEnumerable<IBool>)` | `BooleanListParameterType` | `booleanList` | `False` | `["True","False"]` |
| `ListLiteralDate(IEnumerable<DateOnly>)` | `DateListParameter(IEnumerable<IDate>)` | `DateListParameterType` | `dateList` | `False` | `["2026-10-29"]` |
| `ListLiteralTime(IEnumerable<TimeOnly>)` | `TimeListParameter(IEnumerable<ITime>)` | `TimeListParameterType` | `timeList` | `False` | `["08:30:00","08:30:00.25"]` |
| `ListLiteralDatetime(IEnumerable<DateTimeOffset>)` | `DateTimeListParameter(IEnumerable<IDateTime>)` | `DateTimeListParameterType` | `datetimeList` | `False` | `["2026-10-29T08:30:00"]` |
| `ListLiteralUuid(IEnumerable<Guid>)` | `UuidListParameter(IEnumerable<IGuid>)` | `UuidListParameterType` | `uuidList` | `False` | `["3f2504e0-4f89-11d3-9a0c-0305e82c3301"]` |

The type names are those of `PureQL.CSharp.Model.Types`. A nullable type has the name of its non-null type: `TypeIntegerNullable.Name` is `integer`, and only `Nullable` tells them apart. For a `string` parameter, `Nullable` also tells a typed null from the empty string. A list type is never nullable.

### Current and offset dates and times

A report for the last ten days needs a date relative to when the query runs, not to when the parameter is built. These parameters hold it as date math, the relative values of Elasticsearch and Grafana, such as `now-10d`, and `Pure.RelationalSchema.Storage.PureQL.Projection` evaluates it against the UTC instant the query runs. This package does not evaluate it.

| Parameter | Type | `Name` | `Value` |
|-----------|------|--------|---------|
| `CurrentDateParameter()` | `DateParameterType` | `date` | `now` |
| `CurrentTimeParameter()` | `TimeParameterType` | `time` | `now` |
| `CurrentDateTimeParameter()` | `DateTimeParameterType` | `datetime` | `now` |
| `DateOffsetParameter(years, months, days)`, `(months, days)`, `(days)` | `DateParameterType` | `date` | `now-10d` for `(-10)`, `now-1M-5d` for `(-1, -5)` |
| `TimeOffsetParameter(hours, minutes, seconds)`, `(minutes, seconds)`, `(seconds)` | `TimeParameterType` | `time` | `now-30m` for `(-30, 0)` |
| `DateTimeOffsetParameter(years, months, days, hours, minutes, seconds)`, `(months, …, seconds)`, `(days, hours, minutes, seconds)`, `(hours, minutes, seconds)`, `(minutes, seconds)`, `(seconds)` | `DateTimeParameterType` | `datetime` | `now-7d` for `(-7, 0, 0, 0)`, `now-2h` for `(-2, 0, 0)` |

Every part is an `INumber<long>`, negative to go back. The date math is `now`, then each part that is not zero as its sign, its invariant number and its unit, in the order of the constructor: `y` (years), `M` (months), `d` (days), `h` (hours), `m` (minutes) and `s` (seconds). It is `now` when every part is zero. The parts must not mix signs: `DateOffsetParameter(new Long(-1), new Long(60))`, which would be `-1M+60d`, throws `ArgumentException` when its `Value` is read. The parts are read then too, not when the parameter is built.

For example, on Thursday 2026-10-29 at 08:30 UTC:

| Parameter | Date math | Value |
|-----------|-----------|-------|
| `CurrentDateParameter()` | `now` | `2026-10-29` |
| `DateOffsetParameter(new Long(-10))` | `now-10d` | `2026-10-19` |
| `DateOffsetParameter(new Long(-1), new Long(-5))` | `now-1M-5d` | `2026-09-24` |
| `TimeOffsetParameter(new Long(-30), new Long(0))` | `now-30m` | `08:00:00`, wrapping around midnight |
| `DateTimeOffsetParameter(new Long(-7), new Long(0), new Long(0), new Long(0))` | `now-7d` | `2026-10-22T08:30:00` |
| `CurrentDateTimeParameter()` | `now` | `2026-10-29T08:30:00` |

A report for the last seven days is `orders.ordered_at >= :from AND orders.ordered_at < :to`, with `from` bound to `now-7d` and `to` to `now`. Each `Type` is the type of the value the date math gives, so a query declares the parameter as a plain `date`, `time` or `datetime`.

**The evaluator's contract.** The projection evaluates `now` as the UTC instant the query runs (a date is its UTC date, a time its UTC time of day) and applies the parts in order: years and months as calendar months, with the day clamped to the end of a shorter month, then days, hours, minutes and seconds. A date takes `y`, `M` and `d`; a time `h`, `m` and `s`, and wraps around midnight; a datetime all six. There is no rounding to the start of a week or month yet.

### The value text

- **Decimals** are exact. The scale a `decimal` carries is dropped (`1.50` → `1.5`, `4.00` → `4`, `-0.0` → `0`), and large or small values are written in full (`1E+20m` → `100000000000000000000`).
- **Times and datetimes** carry fractional seconds only when they are not zero, with up to seven digits, trimmed as the `FFFFFFF` format trims them: `08:30:00`, `08:30:00.25`, `08:30:00.123456`, `23:59:59.9999999`. `ITime` holds milliseconds, microseconds and nanoseconds; the text keeps them to the 100 ns tick, and the nanoseconds below a tick are dropped, as `TimeOnly` and `DateTime` drop them.
- **Datetimes** are read as a UTC instant: the parts of the `IDateTime` are the UTC date and time, and the text has no offset. The projection parses it with `DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal`.
- **Strings** are the given `IString`, returned as it is.
- **Lists** are informational, since the projection reads `Items`: a JSON array of the items' texts, each as a JSON string, in order, and `[]` for an empty list. `Utf8JsonWriter` escapes them: quotes and backslashes as `\"` and `\\`, control characters, characters outside the Basic Multilingual Plane (emoji) and a few invisible ones (U+2028) as `\uXXXX`; other text, non-ASCII included, is written as it is. `System.Text.Json` reads it back to the texts.

### List items

`Items` holds one scalar parameter per value, in order: an `IntegerListParameter`'s items are `IntegerParameter`s, with the same text. A PureQL list holds no `null`, so no item is a typed null. A list may be empty and may repeat a value. The given sequence is not copied: each enumeration of `Items` enumerates it again, and `Value` enumerates it once, on its first read.

The `Pure.Primitives` constructors that turn an `IDate`, `ITime` or `IDateTime` into a `String` write another format (`10/29/2026`, `8:30:0.250.0`), which the projection cannot parse. This package does not use them.

`Equals`, `GetHashCode` and `ToString` of every record are those the compiler generates for a record.

## Usage

```csharp
using Pure.Primitives.Date;
using Pure.Primitives.Number;
using Pure.Primitives.Time;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Decimal = Pure.Primitives.Number.Decimal;
using String = Pure.Primitives.String.String;

IParameter price = new DecimalParameter(new Decimal(9.990m));
IParameter since = new DateParameter(new Date(new DateOnly(2026, 10, 29)));
IParameter opensAt = new TimeParameter(new Time(new TimeOnly(8, 30, 0, 250)));
IParameter shippedBefore = new NullableDateParameter();
IParameter tenDaysAgo = new DateOffsetParameter(new Long(-10));
IParameter monthAgo = new DateOffsetParameter(new Long(-1), new Long(0));
IParameter today = new CurrentDateParameter();
IListParameter statuses = new StringListParameter(
    [new String("paid"), new String("say \"hi\"")]
);
IListParameter userIds = new IntegerListParameter([new Long(7), new Long(-42)]);

Console.WriteLine(price.Type.Name.TextValue); // decimal
Console.WriteLine(price.Value.TextValue); // 9.99
Console.WriteLine(since.Value.TextValue); // 2026-10-29
Console.WriteLine(opensAt.Value.TextValue); // 08:30:00.25
Console.WriteLine(shippedBefore.Type.Nullable.BoolValue); // True
Console.WriteLine(shippedBefore.Value.TextValue.Length); // 0
Console.WriteLine(tenDaysAgo.Type.Name.TextValue); // date
Console.WriteLine(tenDaysAgo.Value.TextValue); // now-10d
Console.WriteLine(monthAgo.Value.TextValue); // now-1M
Console.WriteLine(today.Value.TextValue); // now
Console.WriteLine(statuses.Type.Name.TextValue); // stringList
Console.WriteLine(statuses.Value.TextValue); // ["paid","say \"hi\""]
Console.WriteLine(userIds.Items.First().Value.TextValue); // 7
Console.WriteLine(userIds.Value.TextValue); // ["7","-42"]
```

When the query runs, each parameter becomes the PureQL literal its type names, parsed the way the projection parses a stored cell, a list parameter the list literal of its items, and the literals bind the query's parameters by name. This conversion needs `PureQL.CSharp.Model`, so it lives with the code that runs the query, not in this package. A current or offset parameter is evaluated by the projection first, since `now-10d` is not a date:

```csharp
using System.Globalization;
using PureQL.CSharp.Model.Lists;
using PureQL.CSharp.Model.Literals;

static ILiteral Literal(IParameter parameter)
{
    string text = parameter.Value.TextValue;
    CultureInfo invariant = CultureInfo.InvariantCulture;

    return (parameter.Type.Name.TextValue, parameter.Type.Nullable.BoolValue) switch
    {
        ("integer", false) => new LiteralInteger(
            long.Parse(text, NumberStyles.Integer, invariant)
        ),
        ("decimal", false) => new LiteralDecimal(
            decimal.Parse(text, NumberStyles.Float, invariant)
        ),
        ("string", false) => new LiteralString(text),
        ("boolean", false) => new LiteralBoolean(bool.Parse(text)),
        ("date", false) => new LiteralDate(DateOnly.Parse(text, invariant)),
        ("time", false) => new LiteralTime(TimeOnly.Parse(text, invariant)),
        ("datetime", false) => new LiteralDatetime(
            DateTime.Parse(
                text,
                invariant,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
            )
        ),
        ("uuid", false) => new LiteralUuid(Guid.Parse(text)),
        ("integer", true) => new LiteralIntegerNullable(),
        ("decimal", true) => new LiteralDecimalNullable(),
        ("string", true) => new LiteralStringNullable(),
        ("boolean", true) => new LiteralBooleanNullable(),
        ("date", true) => new LiteralDateNullable(),
        ("time", true) => new LiteralTimeNullable(),
        ("datetime", true) => new LiteralDatetimeNullable(),
        ("uuid", true) => new LiteralUuidNullable(),
        _ => throw new ArgumentException(
            $"{parameter.Type.Name.TextValue} is not a PureQL literal type."
        ),
    };
}

static ILiteral ListLiteral(IListParameter parameter)
{
    IEnumerable<string> texts = parameter.Items.Select(item => item.Value.TextValue);
    CultureInfo invariant = CultureInfo.InvariantCulture;

    return parameter.Type.Name.TextValue switch
    {
        "integerList" => new ListLiteralInteger(
            [.. texts.Select(text => long.Parse(text, NumberStyles.Integer, invariant))]
        ),
        "stringList" => new ListLiteralString([.. texts]),
        // ... the other list types, each item parsed as Literal parses its type
        _ => throw new ArgumentException(
            $"{parameter.Type.Name.TextValue} is not a PureQL list type."
        ),
    };
}

IReadOnlyDictionary<string, ILiteral> literals = new Dictionary<string, ILiteral>
{
    ["price"] = Literal(price),
    ["since"] = Literal(since),
    ["opensAt"] = Literal(opensAt),
    ["shippedBefore"] = Literal(shippedBefore),
    ["statuses"] = ListLiteral(statuses),
    ["userIds"] = ListLiteral(userIds),
};

PureQLProjection projection = new PureQLProjection(datasets, query, literals);
```

The examples were compiled against `Pure.RelationalSchema.Storage.PureQL.Projection` 0.1.0-preview.4.6.0 and `PureQL.CSharp.Model` 0.1.0-preview.13.0.0.

## Known limitations

- **No sub-tick precision.** Nanoseconds below 100 ns are dropped, as in `TimeOnly`, `DateTime` and the projection's stored times.
- **No offset.** A datetime is the UTC instant its parts give; convert a local time to UTC before building the `IDateTime`.
- **Invalid values fail late.** An `IDate`, `ITime` or `IDateTime` whose parts are not a valid date or time (February 30th, minute 75) throws `ArgumentOutOfRangeException` when the text is read, not when the parameter is built. `Pure.Primitives`' `Date` and `Time` refuse such parts themselves.
- **Read once.** The text is computed on its first read and kept, as in `Pure.Primitives.String.String`; a value that changes afterwards, such as `CurrentDateTime`, is not read again. A list's `Items` are not kept: each enumeration enumerates the given sequence again, while its `Value` is the text of the first enumeration.
- **Lone surrogates.** A string that is not valid UTF-16 has U+FFFD in place of a lone surrogate in a list's `Value`; its item keeps the string as given.

## Design Principles

- **One record per literal** — 16 parameters, 8 list parameters and 24 parameter types, each a `sealed record`, and 6 current and offset parameters of the date, time and datetime types
- **Model-agnostic** — depends on `Pure.Primitives` and the parameter abstractions only, never on the PureQL model or the projection
- **Lazy** — values are read when the text is read
- **AOT-compatible** — `IsAotCompatible=true`
- **Documented** — XML doc comments on every public member, shipped in the package

## Dependencies

- [`Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions`](https://github.com/kudima03/Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions) — `IParameter`, `IListParameter` and `IParameterType`
- [`Pure.Primitives.String.Operations`](https://github.com/kudima03/Pure.Primitives.String.Operations/tree/1.7.0) — `ConcatenatedString`; brings in [`Pure.Primitives`](https://github.com/kudima03/Pure.Primitives/tree/3.7.0) 3.7.0 (`String`, `EmptyString`, `True` and `False`) and `Pure.Primitives.Abstractions` 4.3.0

## Target Frameworks

- .NET 8
- .NET 9
- .NET 10

## Related packages

- `Pure.RelationalSchema.Storage.PureQL.Projection` — runs PureQL queries over stored relational data and binds their parameters to PureQL literals
- `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions` — the interfaces these records implement
- `PureQL.CSharp.Model` — the PureQL literals and types these records mirror
