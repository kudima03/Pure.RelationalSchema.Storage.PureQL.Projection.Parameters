# Changelog

All notable changes to `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters` are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- List parameters: one sealed record per PureQL list literal implementing `IListParameter` (`IntegerListParameter(IEnumerable<INumber<long>>)`, `DecimalListParameter`, `StringListParameter`, `BooleanListParameter`, `DateListParameter`, `TimeListParameter`, `DateTimeListParameter` and `UuidListParameter`) and `IParameterType` (`IntegerListParameterType` … `UuidListParameterType`, named `integerList` … `uuidList`, never nullable). `Items` holds each value as the scalar parameter of its type, in order, never a typed null. `Value` is informational: a JSON array of the items' cell texts, each as a JSON string, such as `["paid","shipped"]`, and `[]` for an empty list, escaped by `Utf8JsonWriter` without reflection. The values are read when the text is first read.

### Changed

- `Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions` 0.1.0-preview.0.2.0, which adds `IListParameter`.

## [0.1.0-preview.0.1.0] - 2026-10-29

### Added

- Query parameters: one sealed record per PureQL literal implementing `IParameter` (`IntegerParameter`, `DecimalParameter`, `StringParameter`, `BooleanParameter`, `DateParameter`, `TimeParameter`, `DateTimeParameter`, `UuidParameter`, and a parameterless `Nullable…Parameter` for each type) and `IParameterType` (`IntegerParameterType` … `NullableUuidParameterType`, with the PureQL type name and `Nullable`). `Value` is the bound value as invariant cell text in the format `Pure.RelationalSchema.Storage.PureQL.Projection` writes and parses: exact decimals without trailing zeros, `True`/`False`, `yyyy-MM-dd` dates, times and UTC datetimes with fractional seconds to the 100 ns tick only when present, lower case uuids, and the empty string for a typed null. The value is read when the text is first read.
