# Changelog

All notable changes to `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters` are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0-preview.0.2.0] - 2026-10-30

### Added

- List parameters: one sealed record per PureQL list literal implementing `IListParameter` (`IntegerListParameter(IEnumerable<INumber<long>>)`, `DecimalListParameter`, `StringListParameter`, `BooleanListParameter`, `DateListParameter`, `TimeListParameter`, `DateTimeListParameter` and `UuidListParameter`) and `IParameterType` (`IntegerListParameterType` … `UuidListParameterType`, named `integerList` … `uuidList`, never nullable). `Items` holds each value as the scalar parameter of its type, in order, never a typed null. `Value` is informational: a JSON array of the items' cell texts, each as a JSON string, such as `["paid","shipped"]`, and `[]` for an empty list, escaped by `Utf8JsonWriter` without reflection. The values are read when the text is first read.

- Current parameters: `CurrentDateParameter()`, `CurrentTimeParameter()` and `CurrentDateTimeParameter()`, whose `Value` is the date math `now`.
- Offset parameters: `DateOffsetParameter` (years, months, days; months, days; days), `TimeOffsetParameter` (hours, minutes, seconds; minutes, seconds; seconds) and `DateTimeOffsetParameter` (each run of years, months, days, hours, minutes, seconds that ends at seconds), from `INumber<long>` parts. `Value` is date math such as `now-10d` or `now-1M-5d`, the relative values of Elasticsearch and Grafana: `now`, then each non-zero part as its sign, number and unit (`y`, `M`, `d`, `h`, `m`, `s`); `now` when every part is zero. Parts that mix signs, such as `-1M+60d`, throw `ArgumentException` when the text is read. `Type` is the plain `date`, `time` or `datetime` type. `Pure.RelationalSchema.Storage.PureQL.Projection` evaluates the date math against the UTC instant the query runs; this package does not evaluate it.

### Changed

- `GetHashCode` and `ToString` no longer throw `NotSupportedException` on any record: they are the ones the compiler generates for a record.
- `Pure.Primitives.String.Operations` 1.7.0 replaces the direct `Pure.Primitives` reference, which it brings in at 3.7.0.
- `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions` 0.1.0-preview.0.2.0, which adds `IListParameter`.

## [0.1.0-preview.0.1.0] - 2026-10-29

### Added

- Query parameters: one sealed record per PureQL literal implementing `IParameter` (`IntegerParameter`, `DecimalParameter`, `StringParameter`, `BooleanParameter`, `DateParameter`, `TimeParameter`, `DateTimeParameter`, `UuidParameter`, and a parameterless `Nullable…Parameter` for each type) and `IParameterType` (`IntegerParameterType` … `NullableUuidParameterType`, with the PureQL type name and `Nullable`). `Value` is the bound value as invariant cell text in the format `Pure.RelationalSchema.Storage.PureQL.Projection` writes and parses: exact decimals without trailing zeros, `True`/`False`, `yyyy-MM-dd` dates, times and UTC datetimes with fractional seconds to the 100 ns tick only when present, lower case uuids, and the empty string for a typed null. The value is read when the text is first read.
