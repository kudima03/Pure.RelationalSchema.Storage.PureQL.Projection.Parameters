using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

/// <summary>
/// The PureQL list type <c>booleanList</c> of a list parameter: the counterpart of
/// <c>PureQL.CSharp.Model.Types.TypeBooleanList</c>.
/// </summary>
public sealed record BooleanListParameterType : IParameterType
{
    /// <summary>
    /// Gets the PureQL type name, <c>booleanList</c>.
    /// </summary>
    public IString Name { get; } = new String("booleanList");

    /// <summary>
    /// Gets false: a PureQL list type is never nullable.
    /// </summary>
    public IBool Nullable { get; } = new False();
}
