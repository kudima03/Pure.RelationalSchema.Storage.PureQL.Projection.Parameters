using System.Buffers;
using System.Collections;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.PureQL.Projection.Parameters.Abstractions.Parameter;
using Char = Pure.Primitives.Char.Char;

namespace Pure.RelationalSchema.Storage.PureQL.Projection.Parameters;

// The informational text of a list parameter: a JSON array of its items' cell
// texts, each as a JSON string, in order (["paid","shipped"], [] for an empty
// list). Utf8JsonWriter escapes them, so no reflection is needed. The relaxed
// encoder writes quotes and backslashes as \" and \\ and leaves most non-ASCII
// text as it is; control characters, characters outside the Basic
// Multilingual Plane and a few invisible ones are written as \uXXXX, and a lone
// surrogate becomes U+FFFD. The items are read when the text is first read.
internal sealed record ListText : IString
{
    private static JsonWriterOptions Options =>
        new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    private readonly Lazy<string> _text;

    public ListText(IEnumerable<IParameter> items)
    {
        _text = new Lazy<string>(() => Json(items));
    }

    public string TextValue => _text.Value;

    public IEnumerator<IChar> GetEnumerator()
    {
        return _text.Value.Select(symbol => new Char(symbol)).GetEnumerator();
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static string Json(IEnumerable<IParameter> items)
    {
        ArrayBufferWriter<byte> buffer = new ArrayBufferWriter<byte>();
        Write(buffer, items);
        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    // Disposing the writer flushes it into the buffer.
    private static void Write(IBufferWriter<byte> buffer, IEnumerable<IParameter> items)
    {
        using Utf8JsonWriter writer = new Utf8JsonWriter(buffer, Options);
        writer.WriteStartArray();
        foreach (IParameter item in items)
        {
            writer.WriteStringValue(item.Value.TextValue);
        }

        writer.WriteEndArray();
    }
}
