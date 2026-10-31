using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record CurrentTimeParameterTests
{
    [Fact]
    public void HasTimeType()
    {
        IParameter parameter = new CurrentTimeParameter();

        _ = Assert.IsType<TimeParameterType>(parameter.Type);
        Assert.Equal("time", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsNow()
    {
        Assert.Equal("now", new CurrentTimeParameter().Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new CurrentTimeParameter();

        Assert.Equal("now", Characters.Text(parameter.Value));
        Assert.Equal("now", Characters.NonGenericText(parameter.Value));
    }
}
