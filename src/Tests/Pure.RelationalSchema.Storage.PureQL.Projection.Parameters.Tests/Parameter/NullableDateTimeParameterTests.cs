using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record NullableDateTimeParameterTests
{
    [Fact]
    public void HasNullableType()
    {
        IParameter parameter = new NullableDateTimeParameter();

        _ = Assert.IsType<NullableDateTimeParameterType>(parameter.Type);
    }

    [Fact]
    public void HasNullablePureQLType()
    {
        IParameter parameter = new NullableDateTimeParameter();

        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        Assert.True(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsEmptyText()
    {
        Assert.Equal(string.Empty, new NullableDateTimeParameter().Value.TextValue);
    }

    [Fact]
    public void EnumeratesNoCharacters()
    {
        IParameter parameter = new NullableDateTimeParameter();

        Assert.Equal(string.Empty, Characters.Text(parameter.Value));
        Assert.Equal(string.Empty, Characters.NonGenericText(parameter.Value));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new NullableDateTimeParameter().GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new NullableDateTimeParameter().ToString()
        );
    }
}
