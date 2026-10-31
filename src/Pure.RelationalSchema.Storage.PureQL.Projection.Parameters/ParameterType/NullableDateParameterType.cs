using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

/// <summary>
/// The nullable PureQL type <c>date</c>, the type of a typed null: the counterpart of
/// <c>PureQL.CSharp.Model.Types.TypeDateNullable</c>.
/// </summary>
public sealed record NullableDateParameterType : IParameterType
{
    /// <summary>
    /// Gets the PureQL type name, <c>date</c>.
    /// </summary>
    public IString Name { get; } = new String("date");

    /// <summary>
    /// Gets true: a parameter of this type is a typed null.
    /// </summary>
    public IBool Nullable { get; } = new True();
}
