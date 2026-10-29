using System.Globalization;
using Pure.Primitives.Number;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Date = Pure.Primitives.Date.Date;
using DateTime = Pure.Primitives.DateTime.DateTime;
using Time = Pure.Primitives.Time.Time;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record DateTimeParameterTests
{
    public static TheoryData<System.DateTime, string> Texts =>
        new TheoryData<System.DateTime, string>
        {
            { Utc(2026, 10, 29, 8, 30, 0), "2026-10-29T08:30:00" },
            { Utc(2026, 1, 5, 0, 0, 0), "2026-01-05T00:00:00" },
            { Utc(2024, 2, 29, 23, 59, 59), "2024-02-29T23:59:59" },
            {
                Utc(2026, 10, 29, 8, 30, 0).AddMilliseconds(250),
                "2026-10-29T08:30:00.25"
            },
            {
                Utc(2026, 10, 29, 8, 30, 0).AddMicroseconds(123456),
                "2026-10-29T08:30:00.123456"
            },
            { Utc(2026, 10, 29, 8, 30, 0).AddTicks(1), "2026-10-29T08:30:00.0000001" },
            { System.DateTime.UnixEpoch, "1970-01-01T00:00:00" },
            {
                System.DateTime.SpecifyKind(System.DateTime.MinValue, DateTimeKind.Utc),
                "0001-01-01T00:00:00"
            },
            {
                System.DateTime.SpecifyKind(System.DateTime.MaxValue, DateTimeKind.Utc),
                "9999-12-31T23:59:59.9999999"
            },
        };

    [Fact]
    public void HasDateTimeType()
    {
        IParameter parameter = new DateTimeParameter(
            Instant(Utc(2026, 10, 29, 8, 30, 0))
        );

        _ = Assert.IsType<DateTimeParameterType>(parameter.Type);
        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsTextWithFractionOnlyWhenPresent(System.DateTime value, string text)
    {
        Assert.Equal(text, new DateTimeParameter(Instant(value)).Value.TextValue);
    }

    [Theory]
    [InlineData(0, 0, 0, "2026-10-29T01:02:03")]
    [InlineData(4, 5, 600, "2026-10-29T01:02:03.0040056")]
    [InlineData(0, 0, 150, "2026-10-29T01:02:03.0000001")]
    [InlineData(0, 0, 99, "2026-10-29T01:02:03")]
    [InlineData(999, 999, 999, "2026-10-29T01:02:03.9999999")]
    public void IsTextOfDateTimePartsToTheTick(
        ushort millisecond,
        ushort microsecond,
        ushort nanosecond,
        string text
    )
    {
        IParameter parameter = new DateTimeParameter(
            new DateTime(
                new Date(new UShort(29), new UShort(10), new UShort(2026)),
                new Time(
                    new UShort(1),
                    new UShort(2),
                    new UShort(3),
                    new UShort(millisecond),
                    new UShort(microsecond),
                    new UShort(nanosecond)
                )
            )
        );

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IParameter parameter = new DateTimeParameter(
            Instant(Utc(2026, 10, 29, 18, 5, 7).AddMilliseconds(250))
        );

        Assert.Equal(
            "2026-10-29T18:05:07.25",
            Characters.InCulture("th-TH", () => parameter.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void ParsesBackToTheUtcInstant(System.DateTime value, string text)
    {
        string parsed = new DateTimeParameter(Instant(value)).Value.TextValue;

        System.DateTime instant = System.DateTime.Parse(
            parsed,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
        );

        Assert.Equal(text, parsed);
        Assert.Equal(value, instant);
        Assert.Equal(DateTimeKind.Utc, instant.Kind);
    }

    [Fact]
    public void ReadsValueOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateTimeParameter(new UnreadValue());

        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new DateTimeParameter(
            Instant(Utc(2026, 10, 29, 8, 30, 0))
        );

        Assert.Equal("2026-10-29T08:30:00", Characters.Text(parameter.Value));
        Assert.Equal("2026-10-29T08:30:00", Characters.NonGenericText(parameter.Value));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new DateTimeParameter(Instant(System.DateTime.UnixEpoch)).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new DateTimeParameter(Instant(System.DateTime.UnixEpoch)).ToString()
        );
    }

    private static System.DateTime Utc(
        int year,
        int month,
        int day,
        int hour,
        int minute,
        int second
    )
    {
        return new System.DateTime(
            year,
            month,
            day,
            hour,
            minute,
            second,
            DateTimeKind.Utc
        );
    }

    private static DateTime Instant(System.DateTime value)
    {
        return new DateTime(
            new Date(DateOnly.FromDateTime(value)),
            new Time(TimeOnly.FromDateTime(value))
        );
    }
}
