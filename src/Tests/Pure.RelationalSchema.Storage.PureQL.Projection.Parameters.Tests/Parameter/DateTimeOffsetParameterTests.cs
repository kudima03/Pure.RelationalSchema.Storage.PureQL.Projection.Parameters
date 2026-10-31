using Pure.Primitives.Number;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record DateTimeOffsetParameterTests
{
    [Theory]
    [InlineData(-1, -2, -3, -4, -5, -6, "now-1y-2M-3d-4h-5m-6s")]
    [InlineData(1, 2, 3, 4, 5, 6, "now+1y+2M+3d+4h+5m+6s")]
    [InlineData(0, 0, 0, 0, 0, 0, "now")]
    [InlineData(-10, 0, 0, 0, 0, 0, "now-10y")]
    [InlineData(60, 0, 0, 0, 0, 0, "now+60y")]
    [InlineData(0, -10, 0, 0, 0, 0, "now-10M")]
    [InlineData(0, 60, 0, 0, 0, 0, "now+60M")]
    [InlineData(0, 0, -10, 0, 0, 0, "now-10d")]
    [InlineData(0, 0, 60, 0, 0, 0, "now+60d")]
    [InlineData(0, 0, 0, -10, 0, 0, "now-10h")]
    [InlineData(0, 0, 0, 60, 0, 0, "now+60h")]
    [InlineData(0, 0, 0, 0, -10, 0, "now-10m")]
    [InlineData(0, 0, 0, 0, 60, 0, "now+60m")]
    [InlineData(0, 0, 0, 0, 0, -10, "now-10s")]
    [InlineData(0, 0, 0, 0, 0, 60, "now+60s")]
    [InlineData(-1, 0, 0, 0, 0, -5, "now-1y-5s")]
    [InlineData(long.MaxValue, 0, 0, 0, 0, 0, "now+9223372036854775807y")]
    [InlineData(0, 0, 0, 0, 0, long.MinValue, "now-9223372036854775808s")]
    public void IsDateMathOfYearsMonthsDaysHoursMinutesSeconds(
        long years,
        long months,
        long days,
        long hours,
        long minutes,
        long seconds,
        string text
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(years),
            new Long(months),
            new Long(days),
            new Long(hours),
            new Long(minutes),
            new Long(seconds)
        );

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, 0, 0, 0, 0, 60)]
    [InlineData(1, 0, 0, 0, 0, -1)]
    [InlineData(-1, -1, -1, 1, -1, -1)]
    public void ThrowsOnMixedSignsOfYearsMonthsDaysHoursMinutesSecondsWhenTextIsRead(
        long years,
        long months,
        long days,
        long hours,
        long minutes,
        long seconds
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(years),
            new Long(months),
            new Long(days),
            new Long(hours),
            new Long(minutes),
            new Long(seconds)
        );

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            parameter.Value.TextValue
        );
        Assert.Equal(
            "The parts of an offset must not mix signs, as -1M+60d does.",
            exception.Message
        );
    }

    [Fact]
    public void ReadsYearsMonthsDaysHoursMinutesSecondsOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(-1),
            new Long(-1),
            new Long(-1),
            new Long(-1),
            new Long(-1),
            new UnreadValue()
        );

        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, -2, -3, -4, -5, "now-1M-2d-3h-4m-5s")]
    [InlineData(1, 2, 3, 4, 5, "now+1M+2d+3h+4m+5s")]
    [InlineData(0, 0, 0, 0, 0, "now")]
    [InlineData(-10, 0, 0, 0, 0, "now-10M")]
    [InlineData(60, 0, 0, 0, 0, "now+60M")]
    [InlineData(0, -10, 0, 0, 0, "now-10d")]
    [InlineData(0, 60, 0, 0, 0, "now+60d")]
    [InlineData(0, 0, -10, 0, 0, "now-10h")]
    [InlineData(0, 0, 60, 0, 0, "now+60h")]
    [InlineData(0, 0, 0, -10, 0, "now-10m")]
    [InlineData(0, 0, 0, 60, 0, "now+60m")]
    [InlineData(0, 0, 0, 0, -10, "now-10s")]
    [InlineData(0, 0, 0, 0, 60, "now+60s")]
    [InlineData(-1, 0, 0, 0, -5, "now-1M-5s")]
    [InlineData(long.MaxValue, 0, 0, 0, 0, "now+9223372036854775807M")]
    [InlineData(0, 0, 0, 0, long.MinValue, "now-9223372036854775808s")]
    public void IsDateMathOfMonthsDaysHoursMinutesSeconds(
        long months,
        long days,
        long hours,
        long minutes,
        long seconds,
        string text
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(months),
            new Long(days),
            new Long(hours),
            new Long(minutes),
            new Long(seconds)
        );

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, 0, 0, 0, 60)]
    [InlineData(1, 0, 0, 0, -1)]
    [InlineData(-1, -1, 1, -1, -1)]
    public void ThrowsOnMixedSignsOfMonthsDaysHoursMinutesSecondsWhenTextIsRead(
        long months,
        long days,
        long hours,
        long minutes,
        long seconds
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(months),
            new Long(days),
            new Long(hours),
            new Long(minutes),
            new Long(seconds)
        );

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            parameter.Value.TextValue
        );
        Assert.Equal(
            "The parts of an offset must not mix signs, as -1M+60d does.",
            exception.Message
        );
    }

    [Fact]
    public void ReadsMonthsDaysHoursMinutesSecondsOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(-1),
            new Long(-1),
            new Long(-1),
            new Long(-1),
            new UnreadValue()
        );

        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, -2, -3, -4, "now-1d-2h-3m-4s")]
    [InlineData(1, 2, 3, 4, "now+1d+2h+3m+4s")]
    [InlineData(0, 0, 0, 0, "now")]
    [InlineData(-10, 0, 0, 0, "now-10d")]
    [InlineData(60, 0, 0, 0, "now+60d")]
    [InlineData(0, -10, 0, 0, "now-10h")]
    [InlineData(0, 60, 0, 0, "now+60h")]
    [InlineData(0, 0, -10, 0, "now-10m")]
    [InlineData(0, 0, 60, 0, "now+60m")]
    [InlineData(0, 0, 0, -10, "now-10s")]
    [InlineData(0, 0, 0, 60, "now+60s")]
    [InlineData(-1, 0, 0, -5, "now-1d-5s")]
    [InlineData(long.MaxValue, 0, 0, 0, "now+9223372036854775807d")]
    [InlineData(0, 0, 0, long.MinValue, "now-9223372036854775808s")]
    public void IsDateMathOfDaysHoursMinutesSeconds(
        long days,
        long hours,
        long minutes,
        long seconds,
        string text
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(days),
            new Long(hours),
            new Long(minutes),
            new Long(seconds)
        );

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, 0, 0, 60)]
    [InlineData(1, 0, 0, -1)]
    [InlineData(-1, -1, 1, -1)]
    public void ThrowsOnMixedSignsOfDaysHoursMinutesSecondsWhenTextIsRead(
        long days,
        long hours,
        long minutes,
        long seconds
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(days),
            new Long(hours),
            new Long(minutes),
            new Long(seconds)
        );

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            parameter.Value.TextValue
        );
        Assert.Equal(
            "The parts of an offset must not mix signs, as -1M+60d does.",
            exception.Message
        );
    }

    [Fact]
    public void ReadsDaysHoursMinutesSecondsOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(-1),
            new Long(-1),
            new Long(-1),
            new UnreadValue()
        );

        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, -2, -3, "now-1h-2m-3s")]
    [InlineData(1, 2, 3, "now+1h+2m+3s")]
    [InlineData(0, 0, 0, "now")]
    [InlineData(-10, 0, 0, "now-10h")]
    [InlineData(60, 0, 0, "now+60h")]
    [InlineData(0, -10, 0, "now-10m")]
    [InlineData(0, 60, 0, "now+60m")]
    [InlineData(0, 0, -10, "now-10s")]
    [InlineData(0, 0, 60, "now+60s")]
    [InlineData(-1, 0, -5, "now-1h-5s")]
    [InlineData(long.MaxValue, 0, 0, "now+9223372036854775807h")]
    [InlineData(0, 0, long.MinValue, "now-9223372036854775808s")]
    public void IsDateMathOfHoursMinutesSeconds(
        long hours,
        long minutes,
        long seconds,
        string text
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(hours),
            new Long(minutes),
            new Long(seconds)
        );

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, 0, 60)]
    [InlineData(1, 0, -1)]
    [InlineData(-1, 1, -1)]
    public void ThrowsOnMixedSignsOfHoursMinutesSecondsWhenTextIsRead(
        long hours,
        long minutes,
        long seconds
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(hours),
            new Long(minutes),
            new Long(seconds)
        );

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            parameter.Value.TextValue
        );
        Assert.Equal(
            "The parts of an offset must not mix signs, as -1M+60d does.",
            exception.Message
        );
    }

    [Fact]
    public void ReadsHoursMinutesSecondsOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(-1),
            new Long(-1),
            new UnreadValue()
        );

        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, -2, "now-1m-2s")]
    [InlineData(1, 2, "now+1m+2s")]
    [InlineData(0, 0, "now")]
    [InlineData(-10, 0, "now-10m")]
    [InlineData(60, 0, "now+60m")]
    [InlineData(0, -10, "now-10s")]
    [InlineData(0, 60, "now+60s")]
    [InlineData(-1, -5, "now-1m-5s")]
    [InlineData(long.MaxValue, 0, "now+9223372036854775807m")]
    [InlineData(0, long.MinValue, "now-9223372036854775808s")]
    public void IsDateMathOfMinutesSeconds(
        long minutes,
        long seconds,
        string text
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(minutes),
            new Long(seconds)
        );

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, 60)]
    [InlineData(1, -1)]
    [InlineData(-1, 1)]
    public void ThrowsOnMixedSignsOfMinutesSecondsWhenTextIsRead(
        long minutes,
        long seconds
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(minutes),
            new Long(seconds)
        );

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            parameter.Value.TextValue
        );
        Assert.Equal(
            "The parts of an offset must not mix signs, as -1M+60d does.",
            exception.Message
        );
    }

    [Fact]
    public void ReadsMinutesSecondsOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateTimeOffsetParameter(
            new Long(-1),
            new UnreadValue()
        );

        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, "now-1s")]
    [InlineData(1, "now+1s")]
    [InlineData(0, "now")]
    [InlineData(-10, "now-10s")]
    [InlineData(60, "now+60s")]
    [InlineData(long.MaxValue, "now+9223372036854775807s")]
    [InlineData(long.MinValue, "now-9223372036854775808s")]
    public void IsDateMathOfSeconds(
        long seconds,
        string text
    )
    {
        IParameter parameter = new DateTimeOffsetParameter(new Long(seconds));

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsSecondsOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateTimeOffsetParameter(new UnreadValue());

        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void HasDateTimeType()
    {
        IParameter parameter = new DateTimeOffsetParameter(new Long(-10));

        _ = Assert.IsType<DateTimeParameterType>(parameter.Type);
        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IParameter parameter = new DateTimeOffsetParameter(new Long(-10));

        Assert.Equal(
            "now-10s",
            Characters.InCulture("th-TH", () => parameter.Value.TextValue)
        );
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new DateTimeOffsetParameter(new Long(-10));

        Assert.Equal("now-10s", Characters.Text(parameter.Value));
        Assert.Equal("now-10s", Characters.NonGenericText(parameter.Value));
    }
}
