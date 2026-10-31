using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A date offset from the day a query runs, written as date math such as <c>now-10d</c>:
/// evaluated by <c>Pure.RelationalSchema.Storage.PureQL.Projection</c> when the query
/// runs and bound as a <c>PureQL.CSharp.Model.Literals.LiteralDate</c>.
/// </summary>
public sealed record DateOffsetParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to the date <paramref name="years"/>, <paramref
    /// name="months"/> and <paramref name="days"/> from the day the query runs:
    /// <c>now-1M-5d</c> for 0, -1 and -5.
    /// </summary>
    /// <param name="years">The whole years to add, negative to go back.</param>
    /// <param name="months">The whole months to add, negative to go back.</param>
    /// <param name="days">The whole days to add, negative to go back.</param>
    public DateOffsetParameter(
        INumber<long> years,
        INumber<long> months,
        INumber<long> days
    )
        : this(
            new Offset(
                [years, months, days],
                [new String("y"), new String("M"), new String("d")]
            )
        )
    { }

    /// <summary>
    /// Initializes a parameter bound to the date <paramref name="months"/> and <paramref
    /// name="days"/> from the day the query runs: <c>now-1M-5d</c> for -1 and -5.
    /// </summary>
    /// <param name="months">The whole months to add, negative to go back.</param>
    /// <param name="days">The whole days to add, negative to go back.</param>
    public DateOffsetParameter(
        INumber<long> months,
        INumber<long> days
    )
        : this(
            new Offset(
                [months, days],
                [new String("M"), new String("d")]
            )
        )
    { }

    /// <summary>
    /// Initializes a parameter bound to the date <paramref name="days"/> from the day the
    /// query runs: <c>now-10d</c> for -10.
    /// </summary>
    /// <param name="days">The whole days to add, negative to go back.</param>
    public DateOffsetParameter(INumber<long> days)
        : this(
            new Offset(
                [days],
                [new String("d")]
            )
        )
    { }

    private DateOffsetParameter(IString expression)
    {
        Value = expression;
    }

    /// <summary>
    /// Gets the type <see cref="DateParameterType"/>, the type of the value the date math
    /// gives.
    /// </summary>
    public IParameterType Type { get; } = new DateParameterType();

    /// <summary>
    /// Gets the date math: <c>now</c>, then each part that is not zero as its sign,
    /// amount and unit (<c>y</c>, <c>M</c> and <c>d</c>), such as <c>now-1M-5d</c>, and
    /// <c>now</c> when every part is zero. The parts are read when the text is first
    /// read; reading it throws <see cref="ArgumentException"/> when they mix signs, as
    /// <c>-1M+60d</c> does.
    /// </summary>
    public IString Value { get; }
}
