using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.DateTime;
using Pure.Primitives.Abstractions.Guid;
using Pure.Primitives.Abstractions.Number;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

// A value that fails every read, so a test can tell when a parameter reads it.
internal sealed record UnreadValue
    : INumber<long>,
        INumber<decimal>,
        IBool,
        IDateTime,
        IGuid
{
    long INumber<long>.NumberValue => throw new ValueReadException();

    decimal INumber<decimal>.NumberValue => throw new ValueReadException();

    public bool BoolValue => throw new ValueReadException();

    public INumber<ushort> Day => throw new ValueReadException();

    public INumber<ushort> Month => throw new ValueReadException();

    public INumber<ushort> Year => throw new ValueReadException();

    public INumber<ushort> Hour => throw new ValueReadException();

    public INumber<ushort> Minute => throw new ValueReadException();

    public INumber<ushort> Second => throw new ValueReadException();

    public INumber<ushort> Millisecond => throw new ValueReadException();

    public INumber<ushort> Microsecond => throw new ValueReadException();

    public INumber<ushort> Nanosecond => throw new ValueReadException();

    public Guid GuidValue => throw new ValueReadException();
}
