using Pure.Primitives.Number;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record RelativeDateParameterTests
{
    [Fact]
    public void HasDateType()
    {
        IParameter parameter = new RelativeDateParameter(new Long(-10));

        _ = Assert.IsType<DateParameterType>(parameter.Type);
        Assert.Equal("date", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void HasDateTypeForExpression()
    {
        IParameter parameter = new RelativeDateParameter(new String("now-1w/w"));

        _ = Assert.IsType<DateParameterType>(parameter.Type);
    }

    [Theory]
    [InlineData(-10, "now-10d")]
    [InlineData(10, "now+10d")]
    [InlineData(-1, "now-1d")]
    [InlineData(0, "now+0d")]
    [InlineData(long.MaxValue, "now+9223372036854775807d")]
    [InlineData(long.MinValue, "now-9223372036854775808d")]
    public void IsDateMathOfDays(long days, string text)
    {
        Assert.Equal(text, new RelativeDateParameter(new Long(days)).Value.TextValue);
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IParameter parameter = new RelativeDateParameter(new Long(-10));

        Assert.Equal(
            "now-10d",
            Characters.InCulture("fa-IR", () => parameter.Value.TextValue)
        );
    }

    [Fact]
    public void IsTheGivenExpression()
    {
        String expression = new String("now-1w/w");

        Assert.Same(expression, new RelativeDateParameter(expression).Value);
    }

    [Fact]
    public void ReadsDaysOnlyWhenTextIsRead()
    {
        IParameter parameter = new RelativeDateParameter(new UnreadValue());

        Assert.Equal("date", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new RelativeDateParameter(new Long(-10));

        Assert.Equal("now-10d", Characters.Text(parameter.Value));
        Assert.Equal("now-10d", Characters.NonGenericText(parameter.Value));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new RelativeDateParameter(new Long(-10)).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new RelativeDateParameter(new Long(-10)).ToString()
        );
    }
}
