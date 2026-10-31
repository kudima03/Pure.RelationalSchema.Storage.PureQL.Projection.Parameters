using System.Globalization;
using Pure.Primitives.Number;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record IntegerParameterTests
{
    [Fact]
    public void HasIntegerType()
    {
        IParameter parameter = new IntegerParameter(new Long(1));

        _ = Assert.IsType<IntegerParameterType>(parameter.Type);
        Assert.Equal("integer", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(42, "42")]
    [InlineData(-42, "-42")]
    [InlineData(1000000, "1000000")]
    [InlineData(long.MaxValue, "9223372036854775807")]
    [InlineData(long.MinValue, "-9223372036854775808")]
    public void IsInvariantText(long value, string text)
    {
        Assert.Equal(text, new IntegerParameter(new Long(value)).Value.TextValue);
    }

    [Theory]
    [InlineData(-42)]
    [InlineData(long.MinValue)]
    public void IsTheSameTextInAnyCulture(long value)
    {
        IParameter parameter = new IntegerParameter(new Long(value));

        Assert.Equal(
            value.ToString(CultureInfo.InvariantCulture),
            Characters.InCulture("sv-SE", () => parameter.Value.TextValue)
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-7)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void ParsesBackToValue(long value)
    {
        string text = new IntegerParameter(new Long(value)).Value.TextValue;

        Assert.Equal(
            value,
            long.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture)
        );
    }

    [Fact]
    public void ReadsValueOnlyWhenTextIsRead()
    {
        IParameter parameter = new IntegerParameter(new UnreadValue());

        Assert.Equal("integer", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new IntegerParameter(new Long(-42));

        Assert.Equal("-42", Characters.Text(parameter.Value));
        Assert.Equal("-42", Characters.NonGenericText(parameter.Value));
    }
}
