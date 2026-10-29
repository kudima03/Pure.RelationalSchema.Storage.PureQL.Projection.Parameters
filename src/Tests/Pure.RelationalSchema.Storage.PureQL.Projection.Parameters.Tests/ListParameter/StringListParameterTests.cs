using System.Text.Json;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ListParameter;

public sealed record StringListParameterTests
{
    public static TheoryData<string[], string> Texts =>
        new TheoryData<string[], string>
        {
            { ["paid", "shipped"], """["paid","shipped"]""" },
            { [""], """[""]""" },
            { ["", " ", "  padded  "], """[""," ","  padded  "]""" },
            { ["1", "2.5", "True"], """["1","2.5","True"]""" },
            { ["say \"hi\""], """["say \"hi\""]""" },
            { ["C:\\temp\\"], """["C:\\temp\\"]""" },
            { ["\"\\\""], """["\"\\\""]""" },
            { ["line\nbreak\ttab\r"], """["line\nbreak\ttab\r"]""" },
            { ["\b\f"], """["\b\f"]""" },
            { ["\u0000\u0001\u001f\u007f"], """["\u0000\u0001\u001F\u007F"]""" },
            {
                ["Ünïcödé", "Добрый день", "日本語のテキスト"],
                """["Ünïcödé","Добрый день","日本語のテキスト"]"""
            },
            {
                ["emoji \ud83c\udf89 and \ud834\udd1e"],
                """["emoji \uD83C\uDF89 and \uD834\uDD1E"]"""
            },
            { ["\u2028\u2029"], """["\u2028\u2029"]""" },
            { ["</script>&'+`/"], """["</script>&'+`/"]""" },
            { ["'; DROP TABLE users; --"], """["'; DROP TABLE users; --"]""" },
            { ["[\"a\",\"b\"]"], """["[\"a\",\"b\"]"]""" },
        };

    public static TheoryData<string[]> Values =>
        new TheoryData<string[]>
        {
            { ["paid", "shipped", "paid"] },
            { ["", " "] },
            { ["say \"hi\"", "C:\\temp\\", "\"\\\""] },
            { ["line\nbreak\ttab\r\b\f", "\u0000\u0001\u001f\u007f"] },
            { ["Ünïcödé", "Добрый день", "日本語のテキスト"] },
            { ["emoji \ud83c\udf89 and \ud834\udd1e", "\u2028\u2029"] },
            { ["</script>&'+`/", "[\"a\",\"b\"]"] },
        };

    private static string[] Sample => ["paid", "shipped", "paid"];

    private static string[] SampleTexts => ["paid", "shipped", "paid"];

    private static string SampleJson => """["paid","shipped","paid"]""";

    [Fact]
    public void HasStringListType()
    {
        IListParameter parameter = List(Sample);

        _ = Assert.IsType<StringListParameterType>(parameter.Type);
        Assert.Equal("stringList", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void ItemsAreStringParameters()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(Sample.Length, parameter.Items.Count());
        Assert.All(
            parameter.Items,
            item =>
            {
                _ = Assert.IsType<StringParameter>(item);
                _ = Assert.IsType<StringParameterType>(item.Type);
                Assert.Equal("string", item.Type.Name.TextValue);
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
                .Select(value => new StringParameter(new String(value)))
                .Select(item => item.Value.TextValue),
            parameter.Items.Select(item => item.Value.TextValue)
        );
    }

    [Fact]
    public void ItemsAreTheGivenStrings()
    {
        IString[] values = [new String("paid"), new String("shipped")];

        IListParameter parameter = new StringListParameter(values);

        Assert.Equal(2, parameter.Items.Count());
        Assert.All(
            parameter.Items.Zip(values),
            pair => Assert.Same(pair.Second, pair.First.Value)
        );
    }

    [Fact]
    public void WritesLoneSurrogateAsReplacementCharacter()
    {
        IListParameter parameter = List(["a\ud800b"]);

        Assert.Equal("""["a\uFFFDb"]""", parameter.Value.TextValue);
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
        Assert.Equal("stringList", parameter.Type.Name.TextValue);
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
            values,
            document
                .RootElement.EnumerateArray()
                .Select(element => Parse(element.GetString()!))
        );
    }

    [Fact]
    public void EnumeratesValuesOnlyWhenRead()
    {
        IListParameter parameter = new StringListParameter(
            new UnreadValues<IString>()
        );

        Assert.Equal("stringList", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Items.ToList());
    }

    [Fact]
    public void ReadsValuesOnlyWhenTextIsRead()
    {
        IListParameter parameter = new StringListParameter([new UnreadText()]);

        IParameter item = Assert.Single(parameter.Items);
        Assert.Equal("string", item.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => item.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsValuesOnceForText()
    {
        CountedValues<IString> values = new CountedValues<IString>(
            Pure(Sample)
        );

        IListParameter parameter = new StringListParameter(values);

        Assert.Equal(0, values.Enumerations);
        Assert.Equal(SampleJson, parameter.Value.TextValue);
        Assert.Equal(SampleJson, Characters.Text(parameter.Value));
        Assert.Equal(1, values.Enumerations);
    }

    [Fact]
    public void EnumeratesValuesForEachEnumerationOfItems()
    {
        CountedValues<IString> values = new CountedValues<IString>(
            Pure(Sample)
        );

        IListParameter parameter = new StringListParameter(values);

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

    private static StringListParameter List(string[] values)
    {
        return new StringListParameter(Pure(values));
    }

    private static IEnumerable<IString> Pure(string[] values)
    {
        return values.Select(value => new String(value));
    }

    private static string Parse(string text)
    {
        return text;
    }
}
