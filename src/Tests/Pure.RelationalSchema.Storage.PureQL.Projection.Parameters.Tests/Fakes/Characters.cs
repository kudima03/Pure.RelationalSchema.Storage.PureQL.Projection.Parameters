using System.Collections;
using System.Globalization;
using Pure.Primitives.Abstractions.Char;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Tests.Fakes;

internal static class Characters
{
    public static string Text(IEnumerable<IChar> characters)
    {
        return new string([.. characters.Select(character => character.CharValue)]);
    }

    public static string NonGenericText(IEnumerable characters)
    {
        List<char> symbols = [];
        foreach (object character in characters)
        {
            symbols.Add(((IChar)character).CharValue);
        }

        return new string([.. symbols]);
    }

    public static string InCulture(string name, Func<string> text)
    {
        CultureInfo current = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo(name);
        try
        {
            return text();
        }
        finally
        {
            CultureInfo.CurrentCulture = current;
        }
    }
}
