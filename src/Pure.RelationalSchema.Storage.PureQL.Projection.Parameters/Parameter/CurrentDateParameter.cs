using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// The date a query runs at, the UTC date, written as the date math <c>now</c>: evaluated
/// by <c>Pure.RelationalSchema.Storage.PureQL.Projection</c> when the query runs and
/// bound as a <c>PureQL.CSharp.Model.Literals.LiteralDate</c>.
/// </summary>
public sealed record CurrentDateParameter : IParameter
{
    /// <summary>
    /// Gets the type <see cref="DateParameterType"/>, the type of the value the date math
    /// gives.
    /// </summary>
    public IParameterType Type { get; } = new DateParameterType();

    /// <summary>
    /// Gets the date math <c>now</c>.
    /// </summary>
    public IString Value { get; } = new Now();
}
