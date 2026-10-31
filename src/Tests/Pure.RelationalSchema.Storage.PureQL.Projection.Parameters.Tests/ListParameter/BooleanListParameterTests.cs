using System.Text.Json;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ListParameter;

public sealed record BooleanListParameterTests
{
    public static TheoryData<bool[], string> Texts =>
        new TheoryData<bool[], string>
        {
            { [true], """["True"]""" },
            { [false], """["False"]""" },
            { [false, true], """["False","True"]""" },
        };

    public static TheoryData<bool[]> Values =>
        new TheoryData<bool[]>
        {
            { [true] },
            { [false, true, false] },
        };

    private static bool[] Sample => [true, false, true];

    private static string[] SampleTexts => ["True", "False", "True"];

    private static string SampleJson => """["True","False","True"]""";

    [Fact]
    public void HasBooleanListType()
    {
        IListParameter parameter = List(Sample);

        _ = Assert.IsType<BooleanListParameterType>(parameter.Type);
        Assert.Equal("booleanList", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void ItemsAreBooleanParameters()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(Sample.Length, parameter.Items.Count());
        Assert.All(
            parameter.Items,
            item =>
            {
                _ = Assert.IsType<BooleanParameter>(item);
                _ = Assert.IsType<BooleanParameterType>(item.Type);
                Assert.Equal("boolean", item.Type.Name.TextValue);
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
                .Select(value => new BooleanParameter(Bool(value)))
                .Select(item => item.Value.TextValue),
            parameter.Items.Select(item => item.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsJsonArrayOfItemTexts(bool[] values, string json)
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
        Assert.Equal("booleanList", parameter.Type.Name.TextValue);
    }

    [Theory]
    [MemberData(nameof(Values))]
    public void ParsesBackWithSystemTextJson(bool[] values)
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
            Characters.InCulture("tr-TR", () => parameter.Value.TextValue)
        );
    }

    [Fact]
    public void EnumeratesValuesOnlyWhenRead()
    {
        IListParameter parameter = new BooleanListParameter(
            new UnreadValues<IBool>()
        );

        Assert.Equal("booleanList", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Items.ToList());
    }

    [Fact]
    public void ReadsValuesOnlyWhenTextIsRead()
    {
        IListParameter parameter = new BooleanListParameter([new UnreadValue()]);

        IParameter item = Assert.Single(parameter.Items);
        Assert.Equal("boolean", item.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => item.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsValuesOnceForText()
    {
        CountedValues<IBool> values = new CountedValues<IBool>(
            Pure(Sample)
        );

        IListParameter parameter = new BooleanListParameter(values);

        Assert.Equal(0, values.Enumerations);
        Assert.Equal(SampleJson, parameter.Value.TextValue);
        Assert.Equal(SampleJson, Characters.Text(parameter.Value));
        Assert.Equal(1, values.Enumerations);
    }

    [Fact]
    public void EnumeratesValuesForEachEnumerationOfItems()
    {
        CountedValues<IBool> values = new CountedValues<IBool>(
            Pure(Sample)
        );

        IListParameter parameter = new BooleanListParameter(values);

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

    private static BooleanListParameter List(bool[] values)
    {
        return new BooleanListParameter(Pure(values));
    }

    private static IEnumerable<IBool> Pure(bool[] values)
    {
        return values.Select(value => Bool(value));
    }

    private static bool Parse(string text)
    {
        return bool.Parse(text);
    }

    private static IBool Bool(bool value)
    {
        return value ? new True() : new False();
    }
}
