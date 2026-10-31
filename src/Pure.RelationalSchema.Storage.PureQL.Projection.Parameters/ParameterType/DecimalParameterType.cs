using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

/// <summary>
/// The PureQL type <c>decimal</c> of a parameter that holds a value: the counterpart
/// of <c>PureQL.CSharp.Model.Types.TypeDecimal</c>.
/// </summary>
public sealed record DecimalParameterType : IParameterType
{
    /// <summary>
    /// Gets the PureQL type name, <c>decimal</c>.
    /// </summary>
    public IString Name { get; } = new String("decimal");

    /// <summary>
    /// Gets false: a parameter of this type holds a value.
    /// </summary>
    public IBool Nullable { get; } = new False();
}
