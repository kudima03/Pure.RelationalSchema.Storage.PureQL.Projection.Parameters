using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ParameterType;

public sealed record NullableTimeParameterTypeTests
{
    [Fact]
    public void HasPureQLTypeName()
    {
        IParameterType type = new NullableTimeParameterType();

        Assert.Equal("time", type.Name.TextValue);
    }

    [Fact]
    public void IsNullable()
    {
        IParameterType type = new NullableTimeParameterType();

        Assert.True(type.Nullable.BoolValue);
    }
}
