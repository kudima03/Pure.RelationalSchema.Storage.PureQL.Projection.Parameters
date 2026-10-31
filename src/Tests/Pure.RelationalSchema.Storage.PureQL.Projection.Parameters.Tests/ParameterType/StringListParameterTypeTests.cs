using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ParameterType;

public sealed record StringListParameterTypeTests
{
    [Fact]
    public void HasPureQLTypeName()
    {
        IParameterType type = new StringListParameterType();

        Assert.Equal("stringList", type.Name.TextValue);
    }

    [Fact]
    public void IsNotNullable()
    {
        IParameterType type = new StringListParameterType();

        Assert.False(type.Nullable.BoolValue);
    }
}
