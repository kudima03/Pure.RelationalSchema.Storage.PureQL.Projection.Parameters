using System.Collections;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

// A sequence that fails when it is enumerated, so a test can tell when a list
// parameter enumerates it.
internal sealed record UnreadValues<T> : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        throw new ValueReadException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
