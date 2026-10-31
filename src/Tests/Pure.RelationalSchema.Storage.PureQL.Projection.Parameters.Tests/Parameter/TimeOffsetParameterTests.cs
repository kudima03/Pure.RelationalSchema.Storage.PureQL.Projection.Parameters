using Pure.Primitives.Number;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record TimeOffsetParameterTests
{
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
        IParameter parameter = new TimeOffsetParameter(
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
        IParameter parameter = new TimeOffsetParameter(
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
        IParameter parameter = new TimeOffsetParameter(
            new Long(-1),
            new Long(-1),
            new UnreadValue()
        );

        Assert.Equal("time", parameter.Type.Name.TextValue);
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
        IParameter parameter = new TimeOffsetParameter(
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
        IParameter parameter = new TimeOffsetParameter(
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
        IParameter parameter = new TimeOffsetParameter(new Long(-1), new UnreadValue());

        Assert.Equal("time", parameter.Type.Name.TextValue);
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
        IParameter parameter = new TimeOffsetParameter(new Long(seconds));

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsSecondsOnlyWhenTextIsRead()
    {
        IParameter parameter = new TimeOffsetParameter(new UnreadValue());

        Assert.Equal("time", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void HasTimeType()
    {
        IParameter parameter = new TimeOffsetParameter(new Long(-10));

        _ = Assert.IsType<TimeParameterType>(parameter.Type);
        Assert.Equal("time", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IParameter parameter = new TimeOffsetParameter(new Long(-10));

        Assert.Equal(
            "now-10s",
            Characters.InCulture("ar-SA", () => parameter.Value.TextValue)
        );
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new TimeOffsetParameter(new Long(-10));

        Assert.Equal("now-10s", Characters.Text(parameter.Value));
        Assert.Equal("now-10s", Characters.NonGenericText(parameter.Value));
    }
}
