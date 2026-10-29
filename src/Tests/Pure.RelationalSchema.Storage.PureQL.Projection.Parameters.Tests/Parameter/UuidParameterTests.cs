using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Parameter;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.ParameterType;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;
using Guid = Pure.Primitives.Guid.Guid;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Parameter;

public sealed record UuidParameterTests
{
    [Fact]
    public void HasUuidType()
    {
        IParameter parameter = new UuidParameter(new Guid(System.Guid.Empty));

        _ = Assert.IsType<UuidParameterType>(parameter.Type);
        Assert.Equal("uuid", parameter.Type.Name.TextValue);
        Assert.False(parameter.Type.Nullable.BoolValue);
    }

    [Theory]
    [InlineData(
        "3F2504E0-4F89-11D3-9A0C-0305E82C3301",
        "3f2504e0-4f89-11d3-9a0c-0305e82c3301"
    )]
    [InlineData(
        "{3F2504E0-4F89-11D3-9A0C-0305E82C3301}",
        "3f2504e0-4f89-11d3-9a0c-0305e82c3301"
    )]
    [InlineData(
        "3f2504e04f8911d39a0c0305e82c3301",
        "3f2504e0-4f89-11d3-9a0c-0305e82c3301"
    )]
    [InlineData(
        "00000000-0000-0000-0000-000000000000",
        "00000000-0000-0000-0000-000000000000"
    )]
    [InlineData(
        "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF",
        "ffffffff-ffff-ffff-ffff-ffffffffffff"
    )]
    public void IsLowerCaseHyphenatedText(string uuid, string text)
    {
        Assert.Equal(
            text,
            new UuidParameter(new Guid(System.Guid.Parse(uuid))).Value.TextValue
        );
    }

    [Fact]
    public void ParsesBackToValue()
    {
        System.Guid value = System.Guid.NewGuid();

        Assert.Equal(
            value,
            System.Guid.Parse(new UuidParameter(new Guid(value)).Value.TextValue)
        );
    }

    [Fact]
    public void ReadsValueOnlyWhenTextIsRead()
    {
        IParameter parameter = new UuidParameter(new UnreadValue());

        Assert.Equal("uuid", parameter.Type.Name.TextValue);
        _ = Assert.Throws<ValueReadException>(() => parameter.Value.TextValue);
    }

    [Fact]
    public void EnumeratesCharactersOfText()
    {
        IParameter parameter = new UuidParameter(new Guid(System.Guid.Empty));

        Assert.Equal(
            "00000000-0000-0000-0000-000000000000",
            Characters.Text(parameter.Value)
        );
        Assert.Equal(
            "00000000-0000-0000-0000-000000000000",
            Characters.NonGenericText(parameter.Value)
        );
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new UuidParameter(new Guid(System.Guid.Empty)).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new UuidParameter(new Guid(System.Guid.Empty)).ToString()
        );
    }
}
