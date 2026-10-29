using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;

/// <summary>
/// A list of decimals bound to a list parameter, which only <c>in</c> reads: the
/// counterpart of <c>PureQL.CSharp.Model.Lists.ListLiteralDecimal</c>.
/// </summary>
public sealed record DecimalListParameter : IListParameter
{
    /// <summary>
    /// Initializes a list parameter bound to <paramref name="values"/>.
    /// </summary>
    /// <param name="values">
    /// The values of the list, in order. The sequence is neither enumerated nor copied
    /// here.
    /// </param>
    public DecimalListParameter(IEnumerable<INumber<decimal>> values)
    {
        Items = values.Select(value => new DecimalParameter(value));
        Value = new ListText(Items);
    }

    /// <summary>
    /// Gets the list type <see cref="DecimalListParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new DecimalListParameterType();

    /// <summary>
    /// Gets each value as a <see cref="DecimalParameter"/>, in order. Each enumeration
    /// enumerates the given values again.
    /// </summary>
    public IEnumerable<IParameter> Items { get; }

    /// <summary>
    /// Gets a JSON array of the items' values, each as a JSON string, in order, such as
    /// <c>["1","2.5"]</c>, and <c>[]</c> for an empty list. It is informational: the
    /// projection reads <see cref="Items"/>. The items are read when the text is first
    /// read, not before, and the text is kept.
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
