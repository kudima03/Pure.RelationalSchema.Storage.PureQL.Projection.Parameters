namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

internal sealed class ValueReadException : Exception
{
    public ValueReadException()
        : base("The value was read.") { }
}
