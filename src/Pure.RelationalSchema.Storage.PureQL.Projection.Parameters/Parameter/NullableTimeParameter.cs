using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A typed null of the PureQL type <c>time</c> bound to a query parameter: the
/// counterpart of <c>PureQL.CSharp.Model.Literals.LiteralTimeNullable</c>.
/// </summary>
public sealed record NullableTimeParameter : IParameter
{
    /// <summary>
    /// Gets the nullable type <see cref="NullableTimeParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new NullableTimeParameterType();

    /// <summary>
    /// Gets the empty string, the storage layer's NULL.
    /// </summary>
    public IString Value { get; } = new EmptyString();
}
