using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record StringParameterTests
{
    [Fact]
    public void HasStringType()
    {
        IParameter parameter = new StringParameter(new String("Ada"));

        _ = Assert.IsType<StringParameterType>(parameter.Type);
        Assert.Equal("string", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Theory]
    [InlineData("Ada")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  padded  ")]
    [InlineData("True")]
    [InlineData("1.50")]
    [InlineData("line\nbreak\ttab")]
    [InlineData("'; DROP TABLE users; --")]
    [InlineData("Ünïcödé")]
    [InlineData("Добрый день")]
    [InlineData("日本語のテキスト")]
    [InlineData("emoji 🎉 and 𝄞")]
    public void IsTheStringItself(string text)
    {
        Assert.Equal(text, new StringParameter(new String(text)).Value.TextValue);
    }

    [Fact]
    public void IsTheGivenString()
    {
        IString value = new String("Ada");

        Assert.Same(value, new StringParameter(value).Value);
    }

    [Fact]
    public void EmptyStringIsNotTypedNull()
    {
        IParameter parameter = new StringParameter(new String(string.Empty));

        Assert.Equal(string.Empty, parameter.Value.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new StringParameter(new String("Ünï 🎉"));

        Assert.Equal("Ünï 🎉", Characters.Text(parameter.Value));
        Assert.Equal("Ünï 🎉", Characters.NonGenericText(parameter.Value));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new StringParameter(new String("Ada")).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new StringParameter(new String("Ada")).ToString()
        );
    }
}
