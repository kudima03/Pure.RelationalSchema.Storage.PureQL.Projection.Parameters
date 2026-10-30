using System.Collections;
using System.Globalization;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.Date;
using Pure.Primitives.Abstractions.DateTime;
using Pure.Primitives.Abstractions.Guid;
using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Abstractions.Time;
using Char = Pure.Primitives.Char.Char;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters;

// The invariant text a stored cell holds, in the format
// Pure.RelationalSchema.Storage.PureQL.Projection writes (CellText) and parses
// (StoredColumn): invariant integers, exact decimals without an exponent or
// trailing zeros in the fraction, True/False, yyyy-MM-dd dates, HH:mm:ss times
// and yyyy-MM-ddTHH:mm:ss datetimes with up to seven digits of fractional
// seconds written only when present, and lower case D-format uuids. The
// Pure.Primitives String conversions of dates and times write another format
// (10/29/2026, 8:30:0.0.0). The value is read when the text is first read.
// A relative parameter gives its own Lazy text.
internal sealed record CellText : IString
{
    private const string DateFormat = "yyyy-MM-dd";

    private const string TimeFormat = "HH:mm:ss.FFFFFFF";

    private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFF";

    // Dividing by one with the most decimal places gives the same value with
    // the fewest: the scale a decimal carries (1.50, 4.00) is dropped.
    private static decimal One => 1.0000000000000000000000000000m;

    private readonly Lazy<string> _text;

    public CellText(INumber<long> value)
        : this(
            new Lazy<string>(() =>
                value.NumberValue.ToString(CultureInfo.InvariantCulture)
            )
        )
    { }

    public CellText(INumber<decimal> value)
        : this(
            new Lazy<string>(() =>
                (value.NumberValue / One).ToString(CultureInfo.InvariantCulture)
            )
        )
    { }

    public CellText(IBool value)
        : this(
            new Lazy<string>(() =>
                value.BoolValue ? bool.TrueString : bool.FalseString
            )
        )
    { }

    public CellText(IDate value)
        : this(
            new Lazy<string>(() =>
                new DateOnly(
                    value.Year.NumberValue,
                    value.Month.NumberValue,
                    value.Day.NumberValue
                ).ToString(DateFormat, CultureInfo.InvariantCulture)
            )
        )
    { }

    public CellText(ITime value)
        : this(
            new Lazy<string>(() =>
                TimeOnly
                    .FromDateTime(Instant(new DateOnly(), value))
                    .ToString(TimeFormat, CultureInfo.InvariantCulture)
            )
        )
    { }

    public CellText(IDateTime value)
        : this(
            new Lazy<string>(() =>
                Instant(
                        new DateOnly(
                            value.Year.NumberValue,
                            value.Month.NumberValue,
                            value.Day.NumberValue
                        ),
                        value
                    )
                    .ToString(DateTimeFormat, CultureInfo.InvariantCulture)
            )
        )
    { }

    public CellText(IGuid value)
        : this(
            // Stryker disable once String : the empty format is the D format
            new Lazy<string>(() => value.GuidValue.ToString("D"))
        )
    { }

    public CellText(Lazy<string> text)
    {
        _text = text;
    }

    public string TextValue => _text.Value;

    public IEnumerator<IChar> GetEnumerator()
    {
        return _text.Value.Select(symbol => new Char(symbol)).GetEnumerator();
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // The time of day on the date, in UTC, to the 100 ns tick: the
    // nanoseconds below a tick are dropped, as a TimeOnly or DateTime drops
    // them.
    private static DateTime Instant(DateOnly date, ITime time)
    {
        return new DateTime(
            date.Year,
            date.Month,
            date.Day,
            time.Hour.NumberValue,
            time.Minute.NumberValue,
            time.Second.NumberValue,
            time.Millisecond.NumberValue,
            time.Microsecond.NumberValue,
            DateTimeKind.Utc
        ).AddTicks(time.Nanosecond.NumberValue / TimeSpan.NanosecondsPerTick);
    }
}
