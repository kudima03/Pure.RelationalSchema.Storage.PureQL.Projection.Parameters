using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record RelativeDateTimeParameterTests
{
    [Fact]
    public void HasDateTimeType()
    {
        IParameter parameter = new RelativeDateTimeParameter(new String("now-7d"));

        _ = Assert.IsType<DateTimeParameterType>(parameter.Type);
        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Fact]
    public void IsTheGivenExpression()
    {
        String expression = new String("now-7d");

        Assert.Same(expression, new RelativeDateTimeParameter(expression).Value);
    }

    [Fact]
    public void DoesNotReadExpression()
    {
        IParameter parameter = new RelativeDateTimeParameter(new UnreadText());

        Assert.Equal("datetime", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new RelativeDateTimeParameter(new String("now-7d")).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new RelativeDateTimeParameter(new String("now-7d")).ToString()
        );
    }
}
