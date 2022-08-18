using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CellLib
{
    public class CellLocationConverter : JsonConverter<CellLocation>
    {
        public override CellLocation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string[] src = reader.GetString()!.Split(',');
            return new(int.Parse(src[0].Split(':')[1]), int.Parse(src[1].Split(':')[1]));
        }

        public override void Write(Utf8JsonWriter writer, CellLocation value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{{\"Column\":{value.Column},\"Row\":{value.Row}}}");
        }
    }
}
