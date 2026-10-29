using System.Text.Json;
using Pure.Primitives.Abstractions.Guid;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Guid = Pure.Primitives.Guid.Guid;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ListParameter;

public sealed record UuidListParameterTests
{
    public static TheoryData<string[], string> Texts =>
        new TheoryData<string[], string>
        {
            {
                ["3F2504E0-4F89-11D3-9A0C-0305E82C3301"],
                """["3f2504e0-4f89-11d3-9a0c-0305e82c3301"]"""
            },
            {
                [
                    "{FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF}",
                    "00000000-0000-0000-0000-000000000000",
                ],
                "[\"ffffffff-ffff-ffff-ffff-ffffffffffff\","
                + "\"00000000-0000-0000-0000-000000000000\"]"
            },
        };

    public static TheoryData<string[]> Values =>
        new TheoryData<string[]>
        {
            { ["3F2504E0-4F89-11D3-9A0C-0305E82C3301"] },
            {
                [
                    "00000000-0000-0000-0000-000000000000",
                    "ffffffff-ffff-ffff-ffff-ffffffffffff",
                ]
            },
        };

    private static string[] Sample =>
        [
            "3F2504E0-4F89-11D3-9A0C-0305E82C3301",
            "00000000-0000-0000-0000-000000000000",
            "3F2504E0-4F89-11D3-9A0C-0305E82C3301",
        ];

    private static string[] SampleTexts =>
        [
            "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            "00000000-0000-0000-0000-000000000000",
            "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        ];

    private static string SampleJson =>
        "[\"3f2504e0-4f89-11d3-9a0c-0305e82c3301\","
        + "\"00000000-0000-0000-0000-000000000000\","
        + "\"3f2504e0-4f89-11d3-9a0c-0305e82c3301\"]";

    [Fact]
    public void HasUuidListType()
    {
        IListParameter parameter = List(Sample);

        _ = Assert.IsType<UuidListParameterType>(parameter.Type);
        Assert.Equal("uuidList", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void ItemsAreUuidParameters()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(Sample.Length, parameter.Items.Count());
        Assert.All(
            parameter.Items,
            item =>
            {
                _ = Assert.IsType<UuidParameter>(item);
                _ = Assert.IsType<UuidParameterType>(item.Type);
                Assert.Equal("uuid", item.Type.Name.TextValue);
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
                .Select(value => new UuidParameter(new Guid(System.Guid.Parse(value))))
                .Select(item => item.Value.TextValue),
            parameter.Items.Select(item => item.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsJsonArrayOfItemTexts(string[] values, string json)
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
        Assert.Equal("uuidList", parameter.Type.Name.TextValue);
    }

    [Theory]
    [MemberData(nameof(Values))]
    public void ParsesBackWithSystemTextJson(string[] values)
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
            values.Select(System.Guid.Parse),
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
        IListParameter parameter = new UuidListParameter(
            new UnreadValues<IGuid>()
        );

        Assert.Equal("uuidList", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Items.ToList());
    }

    [Fact]
    public void ReadsValuesOnlyWhenTextIsRead()
    {
        IListParameter parameter = new UuidListParameter([new UnreadValue()]);

        IParameter item = Assert.Single(parameter.Items);
        Assert.Equal("uuid", item.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => item.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsValuesOnceForText()
    {
        CountedValues<IGuid> values = new CountedValues<IGuid>(
            Pure(Sample)
        );

        IListParameter parameter = new UuidListParameter(values);

        Assert.Equal(0, values.Enumerations);
        Assert.Equal(SampleJson, parameter.Value.TextValue);
        Assert.Equal(SampleJson, Characters.Text(parameter.Value));
        Assert.Equal(1, values.Enumerations);
    }

    [Fact]
    public void EnumeratesValuesForEachEnumerationOfItems()
    {
        CountedValues<IGuid> values = new CountedValues<IGuid>(
            Pure(Sample)
        );

        IListParameter parameter = new UuidListParameter(values);

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

    private static UuidListParameter List(string[] values)
    {
        return new UuidListParameter(Pure(values));
    }

    private static IEnumerable<IGuid> Pure(string[] values)
    {
        return values.Select(value => new Guid(System.Guid.Parse(value)));
    }

    private static System.Guid Parse(string text)
    {
        return System.Guid.Parse(text);
    }
}
