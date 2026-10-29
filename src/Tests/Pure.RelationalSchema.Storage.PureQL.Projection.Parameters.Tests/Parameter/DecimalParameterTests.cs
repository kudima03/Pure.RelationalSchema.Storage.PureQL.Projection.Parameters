using System.Globalization;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Decimal = Pure.Primitives.Number.Decimal;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record DecimalParameterTests
{
    public static TheoryData<decimal, string> Texts =>
        new()
        {
            { 0m, "0" },
            { 0.0m, "0" },
            { -0.000m, "0" },
            { 1.5m, "1.5" },
            { 1.50m, "1.5" },
            { 4.00m, "4" },
            { -1.250m, "-1.25" },
            { 1000m, "1000" },
            { 100.000m, "100" },
            { 1E+20m, "100000000000000000000" },
            { 0.001m, "0.001" },
            { 0.0000000000000000000000000001m, "0.0000000000000000000000000001" },
            { -0.0000000000000000000000000001m, "-0.0000000000000000000000000001" },
            { 1.0000000000000000000000000000m, "1" },
            { 3.1415926535897932384626433833m, "3.1415926535897932384626433833" },
            { 123456789012345678901234567.8m, "123456789012345678901234567.8" },
            { decimal.MaxValue, "79228162514264337593543950335" },
            { decimal.MinValue, "-79228162514264337593543950335" },
        };

    [Fact]
    public void HasDecimalType()
    {
        IParameter parameter = new DecimalParameter(new Decimal(1m));

        _ = Assert.IsType<DecimalParameterType>(parameter.Type);
        Assert.Equal("decimal", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void IsExactTextWithoutTrailingZeros(decimal value, string text)
    {
        Assert.Equal(text, new DecimalParameter(new Decimal(value)).Value.TextValue);
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IParameter parameter = new DecimalParameter(new Decimal(-1234.50m));

        Assert.Equal(
            "-1234.5",
            Characters.InCulture("de-DE", () => parameter.Value.TextValue)
        );
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public void ParsesBackToValue(decimal value, string text)
    {
        string parsed = new DecimalParameter(new Decimal(value)).Value.TextValue;

        Assert.Equal(text, parsed);
        Assert.Equal(
            value,
            decimal.Parse(parsed, NumberStyles.Float, CultureInfo.InvariantCulture)
        );
    }

    [Fact]
    public void ReadsValueOnlyWhenTextIsRead()
    {
        IParameter parameter = new DecimalParameter(new UnreadValue());

        Assert.Equal("decimal", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new DecimalParameter(new Decimal(-1.50m));

        Assert.Equal("-1.5", Characters.Text(parameter.Value));
        Assert.Equal("-1.5", Characters.NonGenericText(parameter.Value));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new DecimalParameter(new Decimal(1m)).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new DecimalParameter(new Decimal(1m)).ToString()
        );
    }
}
