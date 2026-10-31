using Pure.Primitives.Number;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record DateOffsetParameterTests
{
    [Theory]
    [InlineData(-1, -2, -3, "now-1y-2M-3d")]
    [InlineData(1, 2, 3, "now+1y+2M+3d")]
    [InlineData(0, 0, 0, "now")]
    [InlineData(-10, 0, 0, "now-10y")]
    [InlineData(60, 0, 0, "now+60y")]
    [InlineData(0, -10, 0, "now-10M")]
    [InlineData(0, 60, 0, "now+60M")]
    [InlineData(0, 0, -10, "now-10d")]
    [InlineData(0, 0, 60, "now+60d")]
    [InlineData(-1, 0, -5, "now-1y-5d")]
    [InlineData(long.MaxValue, 0, 0, "now+9223372036854775807y")]
    [InlineData(0, 0, long.MinValue, "now-9223372036854775808d")]
    public void IsDateMathOfYearsMonthsDays(
        long years,
        long months,
        long days,
        string text
    )
    {
        IParameter parameter = new DateOffsetParameter(
            new Long(years),
            new Long(months),
            new Long(days)
        );

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, 0, 60)]
    [InlineData(1, 0, -1)]
    [InlineData(-1, 1, -1)]
    public void ThrowsOnMixedSignsOfYearsMonthsDaysWhenTextIsRead(
        long years,
        long months,
        long days
    )
    {
        IParameter parameter = new DateOffsetParameter(
            new Long(years),
            new Long(months),
            new Long(days)
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
    public void ReadsYearsMonthsDaysOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateOffsetParameter(
            new Long(-1),
            new Long(-1),
            new UnreadValue()
        );

        Assert.Equal("date", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, -2, "now-1M-2d")]
    [InlineData(1, 2, "now+1M+2d")]
    [InlineData(0, 0, "now")]
    [InlineData(-10, 0, "now-10M")]
    [InlineData(60, 0, "now+60M")]
    [InlineData(0, -10, "now-10d")]
    [InlineData(0, 60, "now+60d")]
    [InlineData(-1, -5, "now-1M-5d")]
    [InlineData(long.MaxValue, 0, "now+9223372036854775807M")]
    [InlineData(0, long.MinValue, "now-9223372036854775808d")]
    public void IsDateMathOfMonthsDays(
        long months,
        long days,
        string text
    )
    {
        IParameter parameter = new DateOffsetParameter(new Long(months), new Long(days));

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, 60)]
    [InlineData(1, -1)]
    [InlineData(-1, 1)]
    public void ThrowsOnMixedSignsOfMonthsDaysWhenTextIsRead(long months, long days)
    {
        IParameter parameter = new DateOffsetParameter(new Long(months), new Long(days));

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            parameter.Value.TextValue
        );
        Assert.Equal(
            "The parts of an offset must not mix signs, as -1M+60d does.",
            exception.Message
        );
    }

    [Fact]
    public void ReadsMonthsDaysOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateOffsetParameter(new Long(-1), new UnreadValue());

        Assert.Equal("date", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Theory]
    [InlineData(-1, "now-1d")]
    [InlineData(1, "now+1d")]
    [InlineData(0, "now")]
    [InlineData(-10, "now-10d")]
    [InlineData(60, "now+60d")]
    [InlineData(long.MaxValue, "now+9223372036854775807d")]
    [InlineData(long.MinValue, "now-9223372036854775808d")]
    public void IsDateMathOfDays(
        long days,
        string text
    )
    {
        IParameter parameter = new DateOffsetParameter(new Long(days));

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsDaysOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateOffsetParameter(new UnreadValue());

        Assert.Equal("date", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void HasDateType()
    {
        IParameter parameter = new DateOffsetParameter(new Long(-10));

        _ = Assert.IsType<DateParameterType>(parameter.Type);
        Assert.Equal("date", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IParameter parameter = new DateOffsetParameter(new Long(-10));

        Assert.Equal(
            "now-10d",
            Characters.InCulture("fa-IR", () => parameter.Value.TextValue)
        );
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new DateOffsetParameter(new Long(-10));

        Assert.Equal("now-10d", Characters.Text(parameter.Value));
        Assert.Equal("now-10d", Characters.NonGenericText(parameter.Value));
    }
}
