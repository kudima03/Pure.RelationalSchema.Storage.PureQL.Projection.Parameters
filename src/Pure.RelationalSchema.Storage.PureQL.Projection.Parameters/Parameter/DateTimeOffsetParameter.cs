using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A UTC instant offset from the instant a query runs, written as date math such as
/// <c>now-90s</c>: evaluated by <c>Pure.RelationalSchema.Storage.PureQL.Projection</c>
/// when the query runs and bound as a
/// <c>PureQL.CSharp.Model.Literals.LiteralDatetime</c>.
/// </summary>
public sealed record DateTimeOffsetParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to the UTC instant <paramref name="years"/>,
    /// <paramref name="months"/>, <paramref name="days"/>, <paramref name="hours"/>,
    /// <paramref name="minutes"/> and <paramref name="seconds"/> from the instant the
    /// query runs: <c>now-7d</c> for 0, 0, -7, 0, 0 and 0.
    /// </summary>
    /// <param name="years">The whole years to add, negative to go back.</param>
    /// <param name="months">The whole months to add, negative to go back.</param>
    /// <param name="days">The whole days to add, negative to go back.</param>
    /// <param name="hours">The whole hours to add, negative to go back.</param>
    /// <param name="minutes">The whole minutes to add, negative to go back.</param>
    /// <param name="seconds">The whole seconds to add, negative to go back.</param>
    public DateTimeOffsetParameter(
        INumber<long> years,
        INumber<long> months,
        INumber<long> days,
        INumber<long> hours,
        INumber<long> minutes,
        INumber<long> seconds
    )
        : this(
            new Offset(
                [years, months, days, hours, minutes, seconds],
                [
                    new String("y"),
                    new String("M"),
                    new String("d"),
                    new String("h"),
                    new String("m"),
                    new String("s"),
                ]
            )
        )
    { }

    /// <summary>
    /// Initializes a parameter bound to the UTC instant <paramref name="months"/>,
    /// <paramref name="days"/>, <paramref name="hours"/>, <paramref name="minutes"/> and
    /// <paramref name="seconds"/> from the instant the query runs: <c>now-1M</c> for -1,
    /// 0, 0, 0 and 0.
    /// </summary>
    /// <param name="months">The whole months to add, negative to go back.</param>
    /// <param name="days">The whole days to add, negative to go back.</param>
    /// <param name="hours">The whole hours to add, negative to go back.</param>
    /// <param name="minutes">The whole minutes to add, negative to go back.</param>
    /// <param name="seconds">The whole seconds to add, negative to go back.</param>
    public DateTimeOffsetParameter(
        INumber<long> months,
        INumber<long> days,
        INumber<long> hours,
        INumber<long> minutes,
        INumber<long> seconds
    )
        : this(
            new Offset(
                [months, days, hours, minutes, seconds],
                [
                    new String("M"),
                    new String("d"),
                    new String("h"),
                    new String("m"),
                    new String("s"),
                ]
            )
        )
    { }

    /// <summary>
    /// Initializes a parameter bound to the UTC instant <paramref name="days"/>,
    /// <paramref name="hours"/>, <paramref name="minutes"/> and <paramref
    /// name="seconds"/> from the instant the query runs: <c>now-1d-12h</c> for -1, -12, 0
    /// and 0.
    /// </summary>
    /// <param name="days">The whole days to add, negative to go back.</param>
    /// <param name="hours">The whole hours to add, negative to go back.</param>
    /// <param name="minutes">The whole minutes to add, negative to go back.</param>
    /// <param name="seconds">The whole seconds to add, negative to go back.</param>
    public DateTimeOffsetParameter(
        INumber<long> days,
        INumber<long> hours,
        INumber<long> minutes,
        INumber<long> seconds
    )
        : this(
            new Offset(
                [days, hours, minutes, seconds],
                [new String("d"), new String("h"), new String("m"), new String("s")]
            )
        )
    { }

    /// <summary>
    /// Initializes a parameter bound to the UTC instant <paramref name="hours"/>,
    /// <paramref name="minutes"/> and <paramref name="seconds"/> from the instant the
    /// query runs: <c>now-2h</c> for -2, 0 and 0.
    /// </summary>
    /// <param name="hours">The whole hours to add, negative to go back.</param>
    /// <param name="minutes">The whole minutes to add, negative to go back.</param>
    /// <param name="seconds">The whole seconds to add, negative to go back.</param>
    public DateTimeOffsetParameter(
        INumber<long> hours,
        INumber<long> minutes,
        INumber<long> seconds
    )
        : this(
            new Offset(
                [hours, minutes, seconds],
                [new String("h"), new String("m"), new String("s")]
            )
        )
    { }

    /// <summary>
    /// Initializes a parameter bound to the UTC instant <paramref name="minutes"/> and
    /// <paramref name="seconds"/> from the instant the query runs: <c>now-30m</c> for -30
    /// and 0.
    /// </summary>
    /// <param name="minutes">The whole minutes to add, negative to go back.</param>
    /// <param name="seconds">The whole seconds to add, negative to go back.</param>
    public DateTimeOffsetParameter(
        INumber<long> minutes,
        INumber<long> seconds
    )
        : this(
            new Offset(
                [minutes, seconds],
                [new String("m"), new String("s")]
            )
        )
    { }

    /// <summary>
    /// Initializes a parameter bound to the UTC instant <paramref name="seconds"/> from
    /// the instant the query runs: <c>now-90s</c> for -90.
    /// </summary>
    /// <param name="seconds">The whole seconds to add, negative to go back.</param>
    public DateTimeOffsetParameter(INumber<long> seconds)
        : this(
            new Offset(
                [seconds],
                [new String("s")]
            )
        )
    { }

    private DateTimeOffsetParameter(IString expression)
    {
        Value = expression;
    }

    /// <summary>
    /// Gets the type <see cref="DateTimeParameterType"/>, the type of the value the date
    /// math gives.
    /// </summary>
    public IParameterType Type { get; } = new DateTimeParameterType();

    /// <summary>
    /// Gets the date math: <c>now</c>, then each part that is not zero as its sign,
    /// amount and unit (<c>y</c>, <c>M</c>, <c>d</c>, <c>h</c>, <c>m</c> and <c>s</c>),
    /// such as <c>now-7d</c>, and <c>now</c> when every part is zero. The parts are read
    /// when the text is first read; reading it throws <see cref="ArgumentException"/>
    /// when they mix signs, as <c>-1M+60d</c> does.
    /// </summary>
    public IString Value { get; }
}
