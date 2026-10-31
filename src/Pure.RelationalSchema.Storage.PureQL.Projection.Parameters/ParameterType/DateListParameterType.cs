using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

/// <summary>
/// The PureQL list type <c>dateList</c> of a list parameter: the counterpart of
/// <c>PureQL.CSharp.Model.Types.TypeDateList</c>.
/// </summary>
public sealed record DateListParameterType : IParameterType
{
    /// <summary>
    /// Gets the PureQL type name, <c>dateList</c>.
    /// </summary>
    public IString Name { get; } = new String("dateList");

    /// <summary>
    /// Gets false: a PureQL list type is never nullable.
    /// </summary>
    public IBool Nullable { get; } = new False();
}
