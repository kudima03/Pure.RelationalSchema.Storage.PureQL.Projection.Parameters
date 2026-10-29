using System.Globalization;
using System.Text.Json;
using Pure.Primitives.Abstractions.Time;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Time = Pure.Primitives.Time.Time;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ListParameter;

public sealed record TimeListParameterTests
{
    public static TheoryData<TimeOnly[], string> Texts =>
        new TheoryData<TimeOnly[], string>
        {
            { [new TimeOnly(8, 30)], """["08:30:00"]""" },
            {
                [new TimeOnly(0, 0), new TimeOnly(13, 45, 30)],
                """["00:00:00","13:45:30"]"""
            },
            { [new TimeOnly(8, 30, 0, 123, 456)], """["08:30:00.123456"]""" },
            { [TimeOnly.MaxValue], """["23:59:59.9999999"]""" },
        };

    public static TheoryData<TimeOnly[]> Values =>
        new TheoryData<TimeOnly[]>
        {
            { [new TimeOnly(8, 30)] },
            { [TimeOnly.MinValue, TimeOnly.MaxValue] },
            { [new TimeOnly(8, 30, 0, 250), new TimeOnly(8, 30, 0, 250)] },
        };

    private static TimeOnly[] Sample =>
        [
            new TimeOnly(8, 30),
            new TimeOnly(8, 30, 0, 250),
            new TimeOnly(8, 30),
        ];

    private static string[] SampleTexts => ["08:30:00", "08:30:00.25", "08:30:00"];

    private static string SampleJson => """["08:30:00","08:30:00.25","08:30:00"]""";

    [Fact]
    public void HasTimeListType()
    {
        IListParameter parameter = List(Sample);

        _ = Assert.IsType<TimeListParameterType>(parameter.Type);
        Assert.Equal("timeList", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void ItemsAreTimeParameters()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(Sample.Length, parameter.Items.Count());
        Assert.All(
            parameter.Items,
            item =>
            {
                _ = Assert.IsType<TimeParameter>(item);
                _ = Assert.IsType<TimeParameterType>(item.Type);
                Assert.Equal("time", item.Type.Name.TextValue);
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
                .Select(value => new TimeParameter(new Time(value)))
                .Select(item => item.Value.TextValue),
            parameter.Items.Select(item => item.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsJsonArrayOfItemTexts(TimeOnly[] values, string json)
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
        Assert.Equal("timeList", parameter.Type.Name.TextValue);
    }

    [Theory]
    [MemberData(nameof(Values))]
    public void ParsesBackWithSystemTextJson(TimeOnly[] values)
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
            Characters.InCulture("ar-SA", () => parameter.Value.TextValue)
        );
    }

    [Fact]
    public void EnumeratesValuesOnlyWhenRead()
    {
        IListParameter parameter = new TimeListParameter(
            new UnreadValues<ITime>()
        );

        Assert.Equal("timeList", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Items.ToList());
    }

    [Fact]
    public void ReadsValuesOnlyWhenTextIsRead()
    {
        IListParameter parameter = new TimeListParameter([new UnreadValue()]);

        IParameter item = Assert.Single(parameter.Items);
        Assert.Equal("time", item.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => item.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsValuesOnceForText()
    {
        CountedValues<ITime> values = new CountedValues<ITime>(
            Pure(Sample)
        );

        IListParameter parameter = new TimeListParameter(values);

        Assert.Equal(0, values.Enumerations);
        Assert.Equal(SampleJson, parameter.Value.TextValue);
        Assert.Equal(SampleJson, Characters.Text(parameter.Value));
        Assert.Equal(1, values.Enumerations);
    }

    [Fact]
    public void EnumeratesValuesForEachEnumerationOfItems()
    {
        CountedValues<ITime> values = new CountedValues<ITime>(
            Pure(Sample)
        );

        IListParameter parameter = new TimeListParameter(values);

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

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() => List(Sample).GetHashCode());
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() => List(Sample).ToString());
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCodeOfValue()
    {
        _ = Assert.Throws<NotSupportedException>(() => List(Sample).Value.GetHashCode());
    }

    [Fact]
    public void ThrowsExceptionOnToStringOfValue()
    {
        _ = Assert.Throws<NotSupportedException>(() => List(Sample).Value.ToString());
    }

    private static TimeListParameter List(TimeOnly[] values)
    {
        return new TimeListParameter(Pure(values));
    }

    private static IEnumerable<ITime> Pure(TimeOnly[] values)
    {
        return values.Select(value => new Time(value));
    }

    private static TimeOnly Parse(string text)
    {
        return TimeOnly.Parse(text, CultureInfo.InvariantCulture);
    }
}
