using System.Globalization;
using System.Text.Json;
using Pure.Primitives.Abstractions.Date;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Date = Pure.Primitives.Date.Date;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ListParameter;

public sealed record DateListParameterTests
{
    public static TheoryData<DateOnly[], string> Texts =>
        new TheoryData<DateOnly[], string>
        {
            { [new DateOnly(2026, 10, 29)], """["2026-10-29"]""" },
            {
                [new DateOnly(2024, 2, 29), new DateOnly(2026, 1, 5)],
                """["2024-02-29","2026-01-05"]"""
            },
            { [DateOnly.MinValue, DateOnly.MaxValue], """["0001-01-01","9999-12-31"]""" },
        };

    public static TheoryData<DateOnly[]> Values =>
        new TheoryData<DateOnly[]>
        {
            { [new DateOnly(2026, 10, 29)] },
            { [DateOnly.MinValue, DateOnly.MaxValue] },
        };

    private static DateOnly[] Sample =>
        [
            new DateOnly(2026, 10, 29),
            new DateOnly(2001, 3, 9),
            new DateOnly(2026, 10, 29),
        ];

    private static string[] SampleTexts => ["2026-10-29", "2001-03-09", "2026-10-29"];

    private static string SampleJson => """["2026-10-29","2001-03-09","2026-10-29"]""";

    [Fact]
    public void HasDateListType()
    {
        IListParameter parameter = List(Sample);

        _ = Assert.IsType<DateListParameterType>(parameter.Type);
        Assert.Equal("dateList", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void ItemsAreDateParameters()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(Sample.Length, parameter.Items.Count());
        Assert.All(
            parameter.Items,
            item =>
            {
                _ = Assert.IsType<DateParameter>(item);
                _ = Assert.IsType<DateParameterType>(item.Type);
                Assert.Equal("date", item.Type.Name.TextValue);
                Assert.False(item.Type.Nullable.BoolValue);
            }
        );
    }

    [Fact]
    public void ItemsHoldValuesInOrder()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(SampleTexts, parameter.Items.Select(item => item.Value.TextValue));
        Assert.Equal(
            Sample
                .Select(value => new DateParameter(new Date(value)))
                .Select(item => item.Value.TextValue),
            parameter.Items.Select(item => item.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsJsonArrayOfItemTexts(DateOnly[] values, string json)
    {
        Assert.Equal(json, List(values).Value.TextValue);
    }

    [Fact]
    public void IsJsonArrayOfItemTextsInOrder()
    {
        Assert.Equal(SampleJson, List(Sample).Value.TextValue);
    }

    [Fact]
    public void IsEmptyJsonArrayForEmptyList()
    {
        IListParameter parameter = List([]);

        Assert.Equal("[]", parameter.Value.TextValue);
        Assert.Empty(parameter.Items);
        Assert.Equal("dateList", parameter.Type.Name.TextValue);
    }

    [Theory]
    [MemberData(nameof(Values))]
    public void ParsesBackWithSystemTextJson(DateOnly[] values)
    {
        IListParameter parameter = List(values);

        using JsonDocument document = JsonDocument.Parse(parameter.Value.TextValue);

        Assert.Equal(JsonValueKind.Array, document.RootElement.ValueKind);
        Assert.All(
            document.RootElement.EnumerateArray(),
            element => Assert.Equal(JsonValueKind.String, element.ValueKind)
        );
        Assert.Equal(
            parameter.Items.Select(item => item.Value.TextValue),
            document.RootElement.EnumerateArray().Select(element => element.GetString()!)
        );
        Assert.Equal(
            values,
            document
                .RootElement.EnumerateArray()
                .Select(element => Parse(element.GetString()!))
        );
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(
            SampleJson,
            Characters.InCulture("th-TH", () => parameter.Value.TextValue)
        );
    }

    [Fact]
    public void EnumeratesValuesOnlyWhenRead()
    {
        IListParameter parameter = new DateListParameter(
            new UnreadValues<IDate>()
        );

        Assert.Equal("dateList", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Items.ToList());
    }

    [Fact]
    public void ReadsValuesOnlyWhenTextIsRead()
    {
        IListParameter parameter = new DateListParameter([new UnreadValue()]);

        IParameter item = Assert.Single(parameter.Items);
        Assert.Equal("date", item.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => item.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsValuesOnceForText()
    {
        CountedValues<IDate> values = new CountedValues<IDate>(
            Pure(Sample)
        );

        IListParameter parameter = new DateListParameter(values);

        Assert.Equal(0, values.Enumerations);
        Assert.Equal(SampleJson, parameter.Value.TextValue);
        Assert.Equal(SampleJson, Characters.Text(parameter.Value));
        Assert.Equal(1, values.Enumerations);
    }

    [Fact]
    public void EnumeratesValuesForEachEnumerationOfItems()
    {
        CountedValues<IDate> values = new CountedValues<IDate>(
            Pure(Sample)
        );

        IListParameter parameter = new DateListParameter(values);

        Assert.Equal(Sample.Length, parameter.Items.Count());
        Assert.Equal(Sample.Length, parameter.Items.Count());
        Assert.Equal(2, values.Enumerations);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(SampleJson, Characters.Text(parameter.Value));
        Assert.Equal(SampleJson, Characters.NonGenericText(parameter.Value));
    }

    private static DateListParameter List(DateOnly[] values)
    {
        return new DateListParameter(Pure(values));
    }

    private static IEnumerable<IDate> Pure(DateOnly[] values)
    {
        return values.Select(value => new Date(value));
    }

    private static DateOnly Parse(string text)
    {
        return DateOnly.Parse(text, CultureInfo.InvariantCulture);
    }
}
