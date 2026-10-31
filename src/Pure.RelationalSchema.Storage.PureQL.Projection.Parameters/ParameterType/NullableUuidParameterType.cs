using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

/// <summary>
/// The nullable PureQL type <c>uuid</c>, the type of a typed null: the counterpart of
/// <c>PureQL.CSharp.Model.Types.TypeUuidNullable</c>.
/// </summary>
public sealed record NullableUuidParameterType : IParameterType
{
    /// <summary>
    /// Gets the PureQL type name, <c>uuid</c>.
    /// </summary>
    public IString Name { get; } = new String("uuid");

    /// <summary>
    /// Gets true: a parameter of this type is a typed null.
    /// </summary>
    public IBool Nullable { get; } = new True();
}
