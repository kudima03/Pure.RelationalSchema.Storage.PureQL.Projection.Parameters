using Pure.Primitives.Abstractions.Guid;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A uuid bound to a query parameter: the counterpart of
/// <c>PureQL.CSharp.Model.Literals.LiteralUuid</c>.
/// </summary>
public sealed record UuidParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The bound value.</param>
    public UuidParameter(IGuid value)
    {
        Value = new CellText(value);
    }

    /// <summary>
    /// Gets the type <see cref="UuidParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new UuidParameterType();

    /// <summary>
    /// Gets the value as invariant cell text: a lower case <c>D</c>-format uuid,
    /// such as <c>3f2504e0-4f89-11d3-9a0c-0305e82c3301</c>. The bound value is
    /// read when the text is first read, not before.
    /// </summary>
    public IString Value { get; }
}
