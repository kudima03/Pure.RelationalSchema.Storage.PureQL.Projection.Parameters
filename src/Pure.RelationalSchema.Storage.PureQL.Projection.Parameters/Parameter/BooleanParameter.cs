using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A boolean bound to a query parameter: the counterpart of
/// <c>PureQL.CSharp.Model.Literals.LiteralBoolean</c>.
/// </summary>
public sealed record BooleanParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The bound value.</param>
    public BooleanParameter(IBool value)
    {
        Value = new CellText(value);
    }

    /// <summary>
    /// Gets the type <see cref="BooleanParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new BooleanParameterType();

    /// <summary>
    /// Gets the value as invariant cell text: <c>True</c> or <c>False</c>. The
    /// bound value is read when the text is first read, not before.
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
