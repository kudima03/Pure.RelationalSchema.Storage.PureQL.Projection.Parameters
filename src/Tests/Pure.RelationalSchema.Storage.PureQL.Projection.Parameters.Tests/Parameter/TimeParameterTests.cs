using System.Globalization;
using Pure.Primitives.Number;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Time = Pure.Primitives.Time.Time;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record TimeParameterTests
{
    public static TheoryData<TimeOnly, string> Texts =>
        new()
        {
            { TimeOnly.MinValue, "00:00:00" },
            { new TimeOnly(8, 30), "08:30:00" },
            { new TimeOnly(8, 5, 9), "08:05:09" },
            { new TimeOnly(13, 45, 30), "13:45:30" },
            { new TimeOnly(8, 30, 0, 250), "08:30:00.25" },
            { new TimeOnly(12, 34, 56, 789), "12:34:56.789" },
            { new TimeOnly(12, 34, 56, 789, 12), "12:34:56.789012" },
            { new TimeOnly(0, 0, 0, 0, 1), "00:00:00.000001" },
            { new TimeOnly(0, 0, 0, 100), "00:00:00.1" },
            { new TimeOnly(1), "00:00:00.0000001" },
            { new TimeOnly(new TimeOnly(10, 0).Ticks + 1234567), "10:00:00.1234567" },
            { TimeOnly.MaxValue, "23:59:59.9999999" },
        };

    [Fact]
    public void HasTimeType()
    {
        IParameter parameter = new TimeParameter(new Time(new TimeOnly(8, 30)));

        _ = Assert.IsType<TimeParameterType>(parameter.Type);
        Assert.Equal("time", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsTextWithFractionOnlyWhenPresent(TimeOnly value, string text)
    {
        Assert.Equal(text, new TimeParameter(new Time(value)).Value.TextValue);
    }

    [Theory]
    [InlineData(0, 0, 0, "01:02:03")]
    [InlineData(4, 5, 600, "01:02:03.0040056")]
    [InlineData(0, 0, 900, "01:02:03.0000009")]
    [InlineData(0, 0, 150, "01:02:03.0000001")]
    [InlineData(0, 0, 99, "01:02:03")]
    [InlineData(999, 999, 999, "01:02:03.9999999")]
    [InlineData(0, 10, 0, "01:02:03.00001")]
    public void IsTextOfTimePartsToTheTick(
        ushort millisecond,
        ushort microsecond,
        ushort nanosecond,
        string text
    )
    {
        IParameter parameter = new TimeParameter(
            new Time(
                new UShort(1),
                new UShort(2),
                new UShort(3),
                new UShort(millisecond),
                new UShort(microsecond),
                new UShort(nanosecond)
            )
        );

        Assert.Equal(text, parameter.Value.TextValue);
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IParameter parameter = new TimeParameter(new Time(new TimeOnly(18, 5, 7, 250)));

        Assert.Equal(
            "18:05:07.25",
            Characters.InCulture("ar-SA", () => parameter.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void ParsesBackToValue(TimeOnly value, string text)
    {
        string parsed = new TimeParameter(new Time(value)).Value.TextValue;

        Assert.Equal(text, parsed);
        Assert.Equal(value, TimeOnly.Parse(parsed, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ReadsValueOnlyWhenTextIsRead()
    {
        IParameter parameter = new TimeParameter(new UnreadValue());

        Assert.Equal("time", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new TimeParameter(new Time(new TimeOnly(8, 30, 0, 250)));

        Assert.Equal("08:30:00.25", Characters.Text(parameter.Value));
        Assert.Equal("08:30:00.25", Characters.NonGenericText(parameter.Value));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new TimeParameter(new Time(TimeOnly.MinValue)).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new TimeParameter(new Time(TimeOnly.MinValue)).ToString()
        );
    }
}
