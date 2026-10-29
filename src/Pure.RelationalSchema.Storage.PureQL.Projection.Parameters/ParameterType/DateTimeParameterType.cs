using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

/// <summary>
/// The PureQL type <c>datetime</c> of a parameter that holds a value: the counterpart
/// of <c>PureQL.CSharp.Model.Types.TypeDatetime</c>.
/// </summary>
public sealed record DateTimeParameterType : IParameterType
{
    /// <summary>
    /// Gets the PureQL type name, <c>datetime</c>.
    /// </summary>
    public IString Name { get; } = new String("datetime");

    /// <summary>
    /// Gets false: a parameter of this type holds a value.
    /// </summary>
    public IBool Nullable { get; } = new False();

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
