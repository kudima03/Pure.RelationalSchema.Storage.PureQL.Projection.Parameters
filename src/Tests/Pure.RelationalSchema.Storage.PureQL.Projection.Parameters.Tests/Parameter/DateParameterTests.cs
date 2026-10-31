using System.Globalization;
using Pure.Primitives.Number;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Date = Pure.Primitives.Date.Date;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record DateParameterTests
{
    public static TheoryData<DateOnly, string> Texts =>
        new TheoryData<DateOnly, string>
        {
            { new DateOnly(2026, 10, 29), "2026-10-29" },
            { new DateOnly(2026, 1, 5), "2026-01-05" },
            { new DateOnly(2024, 2, 29), "2024-02-29" },
            { new DateOnly(1970, 1, 1), "1970-01-01" },
            { new DateOnly(999, 12, 31), "0999-12-31" },
            { DateOnly.MinValue, "0001-01-01" },
            { DateOnly.MaxValue, "9999-12-31" },
        };

    [Fact]
    public void HasDateType()
    {
        IParameter parameter = new DateParameter(new Date(new DateOnly(2026, 10, 29)));

        _ = Assert.IsType<DateParameterType>(parameter.Type);
        Assert.Equal("date", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsYearMonthDayText(DateOnly value, string text)
    {
        Assert.Equal(text, new DateParameter(new Date(value)).Value.TextValue);
    }

    [Fact]
    public void IsTextOfDateParts()
    {
        IParameter parameter = new DateParameter(
            new Date(new UShort(9), new UShort(3), new UShort(2001))
        );

        Assert.Equal("2001-03-09", parameter.Value.TextValue);
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IParameter parameter = new DateParameter(new Date(new DateOnly(2026, 10, 29)));

        Assert.Equal(
            "2026-10-29",
            Characters.InCulture("th-TH", () => parameter.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void ParsesBackToValue(DateOnly value, string text)
    {
        string parsed = new DateParameter(new Date(value)).Value.TextValue;

        Assert.Equal(text, parsed);
        Assert.Equal(value, DateOnly.Parse(parsed, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ThrowsOnInvalidDateWhenTextIsRead()
    {
        IParameter parameter = new DateParameter(new InvalidDate());

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsValueOnlyWhenTextIsRead()
    {
        IParameter parameter = new DateParameter(new UnreadValue());

        Assert.Equal("date", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new DateParameter(new Date(new DateOnly(2026, 10, 29)));

        Assert.Equal("2026-10-29", Characters.Text(parameter.Value));
        Assert.Equal("2026-10-29", Characters.NonGenericText(parameter.Value));
    }
}
