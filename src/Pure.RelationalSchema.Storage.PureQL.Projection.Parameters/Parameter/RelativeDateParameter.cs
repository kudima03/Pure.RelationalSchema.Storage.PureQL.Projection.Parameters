using System.Globalization;
using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A date relative to the day a query runs, written as date math such as
/// <c>now-10d</c>: evaluated by
/// <c>Pure.RelationalSchema.Storage.PureQL.Projection</c> when the query runs
/// and bound as a <c>PureQL.CSharp.Model.Literals.LiteralDate</c>.
/// </summary>
public sealed record RelativeDateParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to the date <paramref name="days"/> days
    /// from the day the query runs.
    /// </summary>
    /// <param name="days">
    /// The whole days to add, negative to go back: -10 is <c>now-10d</c>.
    /// </param>
    public RelativeDateParameter(INumber<long> days)
        : this(
            new CellText(
                new Lazy<string>(() =>
                    Now
                    + (days.NumberValue < 0 ? string.Empty : "+")
                    + days.NumberValue.ToString(CultureInfo.InvariantCulture)
                    + "d"
                )
            )
        )
    { }

    /// <summary>
    /// Initializes a parameter bound to the date
    /// <paramref name="expression"/> gives on the day the query runs.
    /// </summary>
    /// <param name="expression">
    /// Date math: <c>now</c>, then <c>+N</c> or <c>-N</c> of a unit, or
    /// <c>/</c> and a unit to round down to its start, applied left to right,
    /// with the units <c>y</c>, <c>M</c>, <c>w</c> (from Monday) and <c>d</c>,
    /// such as <c>now-10d</c> or <c>now-1w/w</c>. It is checked when the query
    /// runs.
    /// </param>
    public RelativeDateParameter(IString expression)
    {
        Value = expression;
    }

    /// <summary>
    /// Gets the type <see cref="DateParameterType"/>, the type of the value the
    /// date math gives.
    /// </summary>
    public IParameterType Type { get; } = new DateParameterType();

    /// <summary>
    /// Gets the date math, such as <c>now-10d</c>: the given expression itself,
    /// or the text of the days, read when the text is first read.
    /// </summary>
    public IString Value { get; }

    private static string Now => "now";

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
