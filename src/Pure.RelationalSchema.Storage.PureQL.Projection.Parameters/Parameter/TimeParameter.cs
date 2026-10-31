using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Abstractions.Time;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A time bound to a query parameter: the counterpart of
/// <c>PureQL.CSharp.Model.Literals.LiteralTime</c>.
/// </summary>
public sealed record TimeParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The bound value.</param>
    public TimeParameter(ITime value)
    {
        Value = new CellText(value);
    }

    /// <summary>
    /// Gets the type <see cref="TimeParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new TimeParameterType();

    /// <summary>
    /// Gets the value as invariant cell text: a <c>HH:mm:ss</c> time with up to
    /// seven digits of fractional seconds, written only when present, such as
    /// <c>08:30:00</c> or <c>08:30:00.25</c>; nanoseconds below 100 ns are
    /// dropped. The bound value is read when the text is first read, not before.
    /// </summary>
    public IString Value { get; }
}
