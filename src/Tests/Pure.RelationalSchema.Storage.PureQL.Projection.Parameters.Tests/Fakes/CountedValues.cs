using System.Collections;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

// A sequence that counts its enumerations, so a test can tell how many times a
// list parameter enumerates it.
internal sealed record CountedValues<T> : IEnumerable<T>
{
    private readonly IEnumerable<T> _values;

    public CountedValues(IEnumerable<T> values)
    {
        _values = values;
    }

    public int Enumerations { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        Enumerations++;
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
