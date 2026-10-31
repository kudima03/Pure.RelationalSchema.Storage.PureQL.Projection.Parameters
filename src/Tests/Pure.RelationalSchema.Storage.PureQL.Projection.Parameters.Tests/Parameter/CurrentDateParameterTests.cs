using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record CurrentDateParameterTests
{
    [Fact]
    public void HasDateType()
    {
        IParameter parameter = new CurrentDateParameter();

        _ = Assert.IsType<DateParameterType>(parameter.Type);
        Assert.Equal("date", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsNow()
    {
        Assert.Equal("now", new CurrentDateParameter().Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new CurrentDateParameter();

        Assert.Equal("now", Characters.Text(parameter.Value));
        Assert.Equal("now", Characters.NonGenericText(parameter.Value));
    }
}
