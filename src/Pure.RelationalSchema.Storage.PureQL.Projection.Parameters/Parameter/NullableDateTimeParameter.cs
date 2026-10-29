using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A typed null of the PureQL type <c>datetime</c> bound to a query parameter: the
/// counterpart of <c>PureQL.CSharp.Model.Literals.LiteralDatetimeNullable</c>.
/// </summary>
public sealed record NullableDateTimeParameter : IParameter
{
    /// <summary>
    /// Gets the nullable type <see cref="NullableDateTimeParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new NullableDateTimeParameterType();

    /// <summary>
    /// Gets the empty string, the storage layer's NULL.
    /// </summary>
    public IString Value { get; } = new EmptyString();

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
