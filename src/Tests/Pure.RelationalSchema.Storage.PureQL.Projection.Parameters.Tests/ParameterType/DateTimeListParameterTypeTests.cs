using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ParameterType;

public sealed record DateTimeListParameterTypeTests
{
    [Fact]
    public void HasPureQLTypeName()
    {
        IParameterType type = new DateTimeListParameterType();

        Assert.Equal("datetimeList", type.Name.TextValue);
    }

    [Fact]
    public void IsNotNullable()
    {
        IParameterType type = new DateTimeListParameterType();

        Assert.False(type.Nullable.BoolValue);
    }
}
