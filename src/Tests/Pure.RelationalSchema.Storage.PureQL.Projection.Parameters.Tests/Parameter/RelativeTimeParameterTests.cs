using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record RelativeTimeParameterTests
{
    [Fact]
    public void HasTimeType()
    {
        IParameter parameter = new RelativeTimeParameter(new String("now-30m"));

        _ = Assert.IsType<TimeParameterType>(parameter.Type);
        Assert.Equal("time", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsTheGivenExpression()
    {
        String expression = new String("now-30m");

        Assert.Same(expression, new RelativeTimeParameter(expression).Value);
    }

    [Fact]
    public void DoesNotReadExpression()
    {
        IParameter parameter = new RelativeTimeParameter(new UnreadText());

        Assert.Equal("time", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new RelativeTimeParameter(new String("now-30m")).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new RelativeTimeParameter(new String("now-30m")).ToString()
        );
    }
}
