using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record CurrentDateTimeParameterTests
{
    [Fact]
    public void HasDateTimeType()
    {
        IParameter parameter = new CurrentDateTimeParameter();

        _ = Assert.IsType<DateTimeParameterType>(parameter.Type);
        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsNow()
    {
        Assert.Equal("now", new CurrentDateTimeParameter().Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new CurrentDateTimeParameter();

        Assert.Equal("now", Characters.Text(parameter.Value));
        Assert.Equal("now", Characters.NonGenericText(parameter.Value));
    }
}
