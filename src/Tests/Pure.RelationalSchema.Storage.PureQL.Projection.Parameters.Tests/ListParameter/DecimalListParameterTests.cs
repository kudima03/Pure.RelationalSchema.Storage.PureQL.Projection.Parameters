using System.Globalization;
using System.Text.Json;
using Pure.Primitives.Abstractions.Number;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ListParameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Decimal = Pure.Primitives.Number.Decimal;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ListParameter;

public sealed record DecimalListParameterTests
{
    public static TheoryData<decimal[], string> Texts =>
        new TheoryData<decimal[], string>
        {
            { [1.50m, 4.00m, -0.0m], """["1.5","4","0"]""" },
            { [-1234.50m], """["-1234.5"]""" },
            {
                [1E+20m, 0.0000000000000000000000000001m],
                """["100000000000000000000","0.0000000000000000000000000001"]"""
            },
            {
                [decimal.MaxValue, decimal.MinValue],
                """["79228162514264337593543950335","-79228162514264337593543950335"]"""
            },
        };

    public static TheoryData<decimal[]> Values =>
        new TheoryData<decimal[]>
        {
            { [1.5m, -0.0m] },
            { [decimal.MaxValue, decimal.MinValue] },
            { [0.0000000000000000000000000001m, 2.5m, 2.5m] },
        };

    private static decimal[] Sample => [1m, 2.50m, 1m];

    private static string[] SampleTexts => ["1", "2.5", "1"];

    private static string SampleJson => """["1","2.5","1"]""";

    [Fact]
    public void HasDecimalListType()
    {
        IListParameter parameter = List(Sample);

        _ = Assert.IsType<DecimalListParameterType>(parameter.Type);
        Assert.Equal("decimalList", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void ItemsAreDecimalParameters()
    {
        IListParameter parameter = List(Sample);

        Assert.Equal(Sample.Length, parameter.Items.Count());
        Assert.All(
            parameter.Items,
            item =>
            {
                _ = Assert.IsType<DecimalParameter>(item);
                _ = Assert.IsType<DecimalParameterType>(item.Type);
                Assert.Equal("decimal", item.Type.Name.TextValue);
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
                .Select(value => new DecimalParameter(new Decimal(value)))
                .Select(item => item.Value.TextValue),
            parameter.Items.Select(item => item.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsJsonArrayOfItemTexts(decimal[] values, string json)
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
        Assert.Equal("decimalList", parameter.Type.Name.TextValue);
    }

    [Theory]
    [MemberData(nameof(Values))]
    public void ParsesBackWithSystemTextJson(decimal[] values)
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
            Characters.InCulture("de-DE", () => parameter.Value.TextValue)
        );
    }

    [Fact]
    public void EnumeratesValuesOnlyWhenRead()
    {
        IListParameter parameter = new DecimalListParameter(
            new UnreadValues<INumber<decimal>>()
        );

        Assert.Equal("decimalList", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Items.ToList());
    }

    [Fact]
    public void ReadsValuesOnlyWhenTextIsRead()
    {
        IListParameter parameter = new DecimalListParameter([new UnreadValue()]);

        IParameter item = Assert.Single(parameter.Items);
        Assert.Equal("decimal", item.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => item.Value.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ReadsValuesOnceForText()
    {
        CountedValues<INumber<decimal>> values = new CountedValues<INumber<decimal>>(
            Pure(Sample)
        );

        IListParameter parameter = new DecimalListParameter(values);

        Assert.Equal(0, values.Enumerations);
        Assert.Equal(SampleJson, parameter.Value.TextValue);
        Assert.Equal(SampleJson, Characters.Text(parameter.Value));
        Assert.Equal(1, values.Enumerations);
    }

    [Fact]
    public void EnumeratesValuesForEachEnumerationOfItems()
    {
        CountedValues<INumber<decimal>> values = new CountedValues<INumber<decimal>>(
            Pure(Sample)
        );

        IListParameter parameter = new DecimalListParameter(values);

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

    private static DecimalListParameter List(decimal[] values)
    {
        return new DecimalListParameter(Pure(values));
    }

    private static IEnumerable<INumber<decimal>> Pure(decimal[] values)
    {
        return values.Select(value => new Decimal(value));
    }

    private static decimal Parse(string text)
    {
        return decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
    }
}
