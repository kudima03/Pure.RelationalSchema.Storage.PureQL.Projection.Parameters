using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A typed null of the PureQL type <c>date</c> bound to a query parameter: the
/// counterpart of <c>PureQL.CSharp.Model.Literals.LiteralDateNullable</c>.
/// </summary>
public sealed record NullableDateParameter : IParameter
{
    /// <summary>
    /// Gets the nullable type <see cref="NullableDateParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new NullableDateParameterType();

    /// <summary>
    /// Gets the empty string, the storage layer's NULL.
    /// </summary>
    public IString Value { get; } = new EmptyString();
}
