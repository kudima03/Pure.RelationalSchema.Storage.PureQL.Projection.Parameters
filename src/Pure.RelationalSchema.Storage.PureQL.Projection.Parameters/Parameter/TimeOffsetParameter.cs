using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;

/// <summary>
/// A time of day offset from the instant a query runs, written as date math such as
/// <c>now-90s</c>: evaluated by <c>Pure.RelationalSchema.Storage.PureQL.Projection</c>
/// when the query runs and bound as a <c>PureQL.CSharp.Model.Literals.LiteralTime</c>.
/// </summary>
public sealed record TimeOffsetParameter : IParameter
{
    /// <summary>
    /// Initializes a parameter bound to the time of day <paramref name="hours"/>,
    /// <paramref name="minutes"/> and <paramref name="seconds"/> from the instant the
    /// query runs: <c>now-1h-30m</c> for -1, -30 and 0.
    /// </summary>
    /// <param name="hours">The whole hours to add, negative to go back.</param>
    /// <param name="minutes">The whole minutes to add, negative to go back.</param>
    /// <param name="seconds">The whole seconds to add, negative to go back.</param>
    public TimeOffsetParameter(
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
    /// Initializes a parameter bound to the time of day <paramref name="minutes"/> and
    /// <paramref name="seconds"/> from the instant the query runs: <c>now-30m</c> for -30
    /// and 0.
    /// </summary>
    /// <param name="minutes">The whole minutes to add, negative to go back.</param>
    /// <param name="seconds">The whole seconds to add, negative to go back.</param>
    public TimeOffsetParameter(
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
    /// Initializes a parameter bound to the time of day <paramref name="seconds"/> from
    /// the instant the query runs: <c>now-90s</c> for -90.
    /// </summary>
    /// <param name="seconds">The whole seconds to add, negative to go back.</param>
    public TimeOffsetParameter(INumber<long> seconds)
        : this(
            new Offset(
                [seconds],
                [new String("s")]
            )
        )
    { }

    private TimeOffsetParameter(IString expression)
    {
        Value = expression;
    }

    /// <summary>
    /// Gets the type <see cref="TimeParameterType"/>, the type of the value the date math
    /// gives.
    /// </summary>
    public IParameterType Type { get; } = new TimeParameterType();

    /// <summary>
    /// Gets the date math: <c>now</c>, then each part that is not zero as its sign,
    /// amount and unit (<c>h</c>, <c>m</c> and <c>s</c>), such as <c>now-1h-30m</c>, and
    /// <c>now</c> when every part is zero. The parts are read when the text is first
    /// read; reading it throws <see cref="ArgumentException"/> when they mix signs, as
    /// <c>-1M+60d</c> does.
    /// </summary>
    public IString Value { get; }
}
