using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.ParameterType;

public sealed record IntegerParameterTypeTests
{
    [Fact]
    public void HasPureQLTypeName()
    {
        IParameterType type = new IntegerParameterType();

        Assert.Equal("integer", type.Name.TextValue);
    }

    [Fact]
    public void IsNotNullable()
    {
        IParameterType type = new IntegerParameterType();

        Assert.False(type.Nullable.BoolValue);
    }
}
