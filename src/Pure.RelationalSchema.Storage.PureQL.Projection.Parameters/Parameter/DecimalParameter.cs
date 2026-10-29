using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A decimal bound to a query parameter: the counterpart of
/// <c>PureQL.CSharp.Model.Literals.LiteralDecimal</c>.
/// </summary>
public sealed record DecimalParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The bound value.</param>
    public DecimalParameter(INumber<decimal> value)
    {
        Value = new CellText(value);
    }

    /// <summary>
    /// Gets the type <see cref="DecimalParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new DecimalParameterType();

    /// <summary>
    /// Gets the value as invariant cell text: an exact decimal without an
    /// exponent or trailing zeros in the fraction, such as <c>1.5</c> for
    /// <c>1.50</c>. The bound value is read when the text is first read, not
    /// before.
    /// </summary>
    public IString Value { get; }

    /// <summary>
    /// Not supported, as in the other Pure primitives.
    /// </summary>
    /// <returns>Never returns.</returns>
    /// <exception cref="NotSupportedException">Always.</exception>
    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Not supported, as in the other Pure primitives.
    /// </summary>
    /// <returns>Never returns.</returns>
    /// <exception cref="NotSupportedException">Always.</exception>
    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
