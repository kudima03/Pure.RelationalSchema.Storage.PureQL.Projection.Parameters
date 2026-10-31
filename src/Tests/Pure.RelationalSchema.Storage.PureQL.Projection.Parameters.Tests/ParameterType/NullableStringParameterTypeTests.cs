using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ParameterType;

public sealed record NullableStringParameterTypeTests
{
    [Fact]
    public void HasPureQLTypeName()
    {
        IParameterType type = new NullableStringParameterType();

        Assert.Equal("string", type.Name.TextValue);
    }

    [Fact]
    public void IsNullable()
    {
        IParameterType type = new NullableStringParameterType();

        Assert.True(type.Nullable.BoolValue);
    }
}
