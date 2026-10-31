using System.Globalization;
using System.Text.Json;
using Pure.Primitives.Abstractions.DateTime;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Date = Pure.Primitives.Date.Date;
using DateTime = Pure.Primitives.DateTime.DateTime;
using Time = Pure.Primitives.Time.Time;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ListParameter;

public sealed record DateTimeListParameterTests
{
    public static TheoryData<System.DateTime[], string> Texts =>
        new TheoryData<System.DateTime[], string>
        {
            { [Utc(2026, 10, 29, 8, 30, 0)], """["2026-10-29T08:30:00"]""" },
            {
                [System.DateTime.UnixEpoch, Utc(2024, 2, 29, 23, 59, 59)],
                """["1970-01-01T00:00:00","2024-02-29T23:59:59"]"""
            },
            {
                [Utc(2026, 10, 29, 8, 30, 0).AddTicks(1)],
                """["2026-10-29T08:30:00.0000001"]"""
            },
            {
                [System.DateTime.SpecifyKind(System.DateTime.MaxValue, DateTimeKind.Utc)],
                """["9999-12-31T23:59:59.9999999"]"""
            },
        };

    public static TheoryData<System.DateTime[]> Values =>
        new TheoryData<System.DateTime[]>
        {
            { [Utc(2026, 10, 29, 8, 30, 0)] },
            {
                [
                    System.DateTime.UnixEpoch,
                    Utc(2026, 10, 29, 8, 30, 0).AddMicroseconds(123456),
                ]
            },
        };

    private static System.DateTime[] Sample =>
        [
            Utc(2026, 10, 29, 8, 30, 0),
            Utc(2026, 10, 29, 8, 30, 0).AddMilliseconds(250),
            Utc(2026, 10, 29, 8, 30, 0),
        ];

    private static string[] SampleTexts =>
        [
            "2026-10-29T08:30:00",
            "2026-10-29T08:30:00.25",
            "2026-10-29T08:30:00",
        ];

    private static string SampleJson =>
        "[\"2026-10-29T08:30:00\","
        + "\"2026-10-29T08:30:00.25\","
        + "\"2026-10-29T08:30:00\"]";

    [Fact]
    public void HasDateTimeListType()
    {
        IListParameter parameter = List(Sample);

        _ = Assert.IsType<DateTimeListParameterType>(parameter.Type);
        Assert.Equal("datetimeList", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void ItemsAreDateTimeParameters()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(Sample.Length, parameter.Items.Count());
        Assert.All(
            parameter.Items,
            item =>
            {
                _ = Assert.IsType<DateTimeParameter>(item);
                _ = Assert.IsType<DateTimeParameterType>(item.Type);
                Assert.Equal("datetime", item.Type.Name.TextValue);
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
                .Select(value => new DateTimeParameter(Instant(value)))
                .Select(item => item.Value.TextValue),
            parameter.Items.Select(item => item.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsJsonArrayOfItemTexts(System.DateTime[] values, string json)
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
        Assert.Equal("datetimeList", parameter.Type.Name.TextValue);
    }

    [Theory]
    [MemberData(nameof(Values))]
    public void ParsesBackWithSystemTextJson(System.DateTime[] values)
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
        IListParameter parameter = new DateTimeListParameter(
            new UnreadValues<IDateTime>()
        );

        Assert.Equal("datetimeList", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Items.ToList());
    }

    [Fact]
    public void ReadsValuesOnlyWhenTextIsRead()
    {
        IListParameter parameter = new DateTimeListParameter([new UnreadValue()]);

        IParameter item = Assert.Single(parameter.Items);
        Assert.Equal("datetime", item.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => item.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsValuesOnceForText()
    {
        CountedValues<IDateTime> values = new CountedValues<IDateTime>(
            Pure(Sample)
        );

        IListParameter parameter = new DateTimeListParameter(values);

        Assert.Equal(0, values.Enumerations);
        Assert.Equal(SampleJson, parameter.Value.TextValue);
        Assert.Equal(SampleJson, Characters.Text(parameter.Value));
        Assert.Equal(1, values.Enumerations);
    }

    [Fact]
    public void EnumeratesValuesForEachEnumerationOfItems()
    {
        CountedValues<IDateTime> values = new CountedValues<IDateTime>(
            Pure(Sample)
        );

        IListParameter parameter = new DateTimeListParameter(values);

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

    private static DateTimeListParameter List(System.DateTime[] values)
    {
        return new DateTimeListParameter(Pure(values));
    }

    private static IEnumerable<IDateTime> Pure(System.DateTime[] values)
    {
        return values.Select(value => Instant(value));
    }

    private static System.DateTime Parse(string text)
    {
        return System.DateTime.Parse(
            text,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
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
