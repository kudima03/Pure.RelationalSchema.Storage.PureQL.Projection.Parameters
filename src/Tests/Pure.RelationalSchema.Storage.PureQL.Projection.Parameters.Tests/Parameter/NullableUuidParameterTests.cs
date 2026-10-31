using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record NullableUuidParameterTests
{
    [Fact]
    public void HasNullableType()
    {
        IParameter parameter = new NullableUuidParameter();

        _ = Assert.IsType<NullableUuidParameterType>(parameter.Type);
    }

    [Fact]
    public void HasNullablePureQLType()
    {
        IParameter parameter = new NullableUuidParameter();

        Assert.Equal("uuid", parameter.Type.Name.TextValue);
        Assert.True(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsEmptyText()
    {
        Assert.Equal(string.Empty, new NullableUuidParameter().Value.TextValue);
    }

    [Fact]
    public void EnumeratesNoCharacters()
    {
        IParameter parameter = new NullableUuidParameter();

        Assert.Equal(string.Empty, Characters.Text(parameter.Value));
        Assert.Equal(string.Empty, Characters.NonGenericText(parameter.Value));
    }
}
