using Pure.Primitives.Abstractions.DateTime;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;

/// <summary>
/// A list of datetimes bound to a list parameter, which only <c>in</c> reads: the
/// counterpart of <c>PureQL.CSharp.Model.Lists.ListLiteralDatetime</c>.
/// </summary>
public sealed record DateTimeListParameter : IListParameter
{
    /// <summary>
    /// Initializes a list parameter bound to <paramref name="values"/>.
    /// </summary>
    /// <param name="values">
    /// The values of the list, in order. The sequence is neither enumerated nor copied
    /// here.
    /// </param>
    public DateTimeListParameter(IEnumerable<IDateTime> values)
    {
        Items = values.Select(value => new DateTimeParameter(value));
        Value = new ListText(Items);
    }

    /// <summary>
    /// Gets the list type <see cref="DateTimeListParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new DateTimeListParameterType();

    /// <summary>
    /// Gets each value as a <see cref="DateTimeParameter"/>, in order. Each enumeration
    /// enumerates the given values again.
    /// </summary>
    public IEnumerable<IParameter> Items { get; }

    /// <summary>
    /// Gets a JSON array of the items' values, each as a JSON string, in order, such as
    /// <c>["2026-10-29T08:30:00"]</c>, and <c>[]</c> for an empty list. It is
    /// informational: the projection reads <see cref="Items"/>. The items are read when
    /// the text is first read, not before, and the text is kept.
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
