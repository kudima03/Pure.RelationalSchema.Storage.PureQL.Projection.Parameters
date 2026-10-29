using Pure.Primitives.Abstractions.Date;
using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Number;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

// February 30th, which Pure.Primitives' Date would refuse to give.
internal sealed record InvalidDate : IDate
{
    public INumber<ushort> Day { get; } = new UShort(30);

    public INumber<ushort> Month { get; } = new UShort(2);

    public INumber<ushort> Year { get; } = new UShort(2026);
}
