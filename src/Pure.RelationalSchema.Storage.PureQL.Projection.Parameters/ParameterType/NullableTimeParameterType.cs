using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

/// <summary>
/// The nullable PureQL type <c>time</c>, the type of a typed null: the counterpart of
/// <c>PureQL.CSharp.Model.Types.TypeTimeNullable</c>.
/// </summary>
public sealed record NullableTimeParameterType : IParameterType
{
    /// <summary>
    /// Gets the PureQL type name, <c>time</c>.
    /// </summary>
    public IString Name { get; } = new String("time");

    /// <summary>
    /// Gets true: a parameter of this type is a typed null.
    /// </summary>
    public IBool Nullable { get; } = new True();

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
