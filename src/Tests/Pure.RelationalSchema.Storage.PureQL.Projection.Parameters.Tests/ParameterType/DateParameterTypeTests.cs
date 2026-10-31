using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ParameterType;

public sealed record DateParameterTypeTests
{
    [Fact]
    public void HasPureQLTypeName()
    {
        IParameterType type = new DateParameterType();

        Assert.Equal("date", type.Name.TextValue);
    }

    [Fact]
    public void IsNotNullable()
    {
        IParameterType type = new DateParameterType();

        Assert.False(type.Nullable.BoolValue);
    }
}
