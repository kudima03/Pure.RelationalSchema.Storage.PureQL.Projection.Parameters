using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters;

// The anchor of date math, now: the instant a query runs, which the projection
// reads when it evaluates the date math.
internal sealed record Now : IString
{
    private readonly IString _text = new String("now");

    public string TextValue => _text.TextValue;

    public IEnumerator<IChar> GetEnumerator()
    {
        return _text.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
