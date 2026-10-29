using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record BooleanParameterTests
{
    [Fact]
    public void HasBooleanType()
    {
        IParameter parameter = new BooleanParameter(new True());

        _ = Assert.IsType<BooleanParameterType>(parameter.Type);
        Assert.Equal("boolean", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void TrueIsTrueText()
    {
        Assert.Equal("True", new BooleanParameter(new True()).Value.TextValue);
    }

    [Fact]
    public void FalseIsFalseText()
    {
        Assert.Equal("False", new BooleanParameter(new False()).Value.TextValue);
    }

    [Fact]
    public void IsTheSameTextInAnyCulture()
    {
        IParameter parameter = new BooleanParameter(new True());

        Assert.Equal(
            "True",
            Characters.InCulture("tr-TR", () => parameter.Value.TextValue)
        );
    }

    [Fact]
    public void ParsesBackToValue()
    {
        IBool[] values = [new True(), new False()];

        Assert.All(
            values,
            value =>
                Assert.Equal(
                    value.BoolValue,
                    bool.Parse(new BooleanParameter(value).Value.TextValue)
                )
        );
    }

    [Fact]
    public void ReadsValueOnlyWhenTextIsRead()
    {
        IParameter parameter = new BooleanParameter(new UnreadValue());

        Assert.Equal("boolean", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new BooleanParameter(new False());

        Assert.Equal("False", Characters.Text(parameter.Value));
        Assert.Equal("False", Characters.NonGenericText(parameter.Value));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new BooleanParameter(new True()).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new BooleanParameter(new True()).ToString()
        );
    }
}
