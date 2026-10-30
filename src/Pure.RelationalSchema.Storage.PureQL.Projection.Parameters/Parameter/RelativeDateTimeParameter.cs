using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A datetime relative to the UTC instant a query runs, written as date math
/// such as <c>now-7d</c>: evaluated by
/// <c>Pure.RelationalSchema.Storage.PureQL.Projection</c> when the query runs
/// and bound as a <c>PureQL.CSharp.Model.Literals.LiteralDatetime</c>.
/// </summary>
public sealed record RelativeDateTimeParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to the instant
    /// <paramref name="expression"/> gives at the instant the query runs.
    /// </summary>
    /// <param name="expression">
    /// Date math: <c>now</c>, then <c>+N</c> or <c>-N</c> of a unit, or
    /// <c>/</c> and a unit to round down to its start, applied left to right,
    /// with the units <c>y</c>, <c>M</c>, <c>w</c> (from Monday), <c>d</c>,
    /// <c>h</c>, <c>m</c> and <c>s</c>, such as <c>now-7d</c> or
    /// <c>now-1w/w</c>. It is checked when the query runs.
    /// </param>
    public RelativeDateTimeParameter(IString expression)
    {
        Value = expression;
    }

    /// <summary>
    /// Gets the type <see cref="DateTimeParameterType"/>, the type of the value
    /// the date math gives.
    /// </summary>
    public IParameterType Type { get; } = new DateTimeParameterType();

    /// <summary>
    /// Gets the date math, such as <c>now-7d</c>: the given expression itself.
    /// </summary>
    public IString Value { get; }

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
