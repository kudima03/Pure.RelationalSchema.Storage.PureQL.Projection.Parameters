using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// An integer bound to a query parameter: the counterpart of
/// <c>PureQL.CSharp.Model.Literals.LiteralInteger</c>.
/// </summary>
public sealed record IntegerParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The bound value.</param>
    public IntegerParameter(INumber<long> value)
    {
        Value = new CellText(value);
    }

    /// <summary>
    /// Gets the type <see cref="IntegerParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new IntegerParameterType();

    /// <summary>
    /// Gets the value as invariant cell text: an invariant integer, such as
    /// <c>-42</c>. The bound value is read when the text is first read, not
    /// before.
    /// </summary>
    public IString Value { get; }
}
