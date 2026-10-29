using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

// A string that fails every read, so a test can tell when a parameter reads it.
internal sealed record UnreadText : IString
{
    public string TextValue => throw new ValueReadException();

    public IEnumerator<IChar> GetEnumerator()
    {
        throw new ValueReadException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
