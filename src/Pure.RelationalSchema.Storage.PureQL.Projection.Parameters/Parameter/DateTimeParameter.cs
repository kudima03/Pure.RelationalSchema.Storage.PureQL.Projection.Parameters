using Pure.Primitives.Abstractions.DateTime;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A datetime bound to a query parameter: the counterpart of
/// <c>PureQL.CSharp.Model.Literals.LiteralDatetime</c>.
/// </summary>
public sealed record DateTimeParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The bound value.</param>
    public DateTimeParameter(IDateTime value)
    {
        Value = new CellText(value);
    }

    /// <summary>
    /// Gets the type <see cref="DateTimeParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new DateTimeParameterType();

    /// <summary>
    /// Gets the value as invariant cell text: a <c>yyyy-MM-ddTHH:mm:ss</c>
    /// datetime without an offset, with up to seven digits of fractional seconds,
    /// written only when present, such as <c>2026-10-29T08:30:00</c>; nanoseconds
    /// below 100 ns are dropped. The bound value is read when the text is first
    /// read, not before.
    /// </summary>
    public IString Value { get; }
}
