using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Number;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters;

// Date math of an offset from now: now, then each part that is not zero as its
// sign, its invariant amount and its unit, in the order given, such as
// now-1M-5d; all zero is now. The parts must not mix signs, as -1M+60d does:
// such an offset throws ArgumentException when the text is read. Each amount
// is read once, on the first read of the text.
internal sealed record Offset : IString
{
    private readonly Lazy<string> _text;

    public Offset(IEnumerable<INumber<long>> amounts, IEnumerable<IString> units)
    {
        _text = new Lazy<string>(() =>
            Text([.. amounts.Select(amount => amount.NumberValue)], units)
        );
    }

    public string TextValue => _text.Value;

    public IEnumerator<IChar> GetEnumerator()
    {
        return _text.Value.Select(symbol => new Char(symbol)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static string Text(IReadOnlyList<long> amounts, IEnumerable<IString> units)
    {
        return amounts.Any(amount => amount > 0) && amounts.Any(amount => amount < 0)
            ? throw new ArgumentException(
                "The parts of an offset must not mix signs, as -1M+60d does."
            )
            : new ConcatenatedString(
                [
                    new Now(),
                    .. amounts
                        .Zip(units)
                        .Where(part => part.First != 0)
                        .Select(part => Part(part.First, part.Second)),
                ]
            ).TextValue;
    }

    private static IString Part(long amount, IString unit)
    {
        return new ConcatenatedString(
            // Stryker disable once Equality : a part is never zero, so >= is >
            amount > 0 ? new String("+") : new EmptyString(),
            new String(new Long(amount)),
            unit
        );
    }
}
