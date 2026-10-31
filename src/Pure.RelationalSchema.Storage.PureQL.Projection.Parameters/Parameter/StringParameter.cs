using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A string bound to a query parameter: the counterpart of
/// <c>PureQL.CSharp.Model.Literals.LiteralString</c>.
/// </summary>
public sealed record StringParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The bound value.</param>
    public StringParameter(IString value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the type <see cref="StringParameterType"/>.
    /// </summary>
    public IParameterType Type { get; } = new StringParameterType();

    /// <summary>
    /// Gets the value as invariant cell text: the string itself.
    /// </summary>
    public IString Value { get; }
}
