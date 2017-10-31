using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicObject
{
    public class ObjectXJsonConverter : JsonConverter<ObjectX>
    {
        private static ObjectXJsonConverter instance;

        public static ObjectXJsonConverter Instance
        {
            get { return instance ?? (instance = new ObjectXJsonConverter()); }
        }

        public override void WriteJson(JsonWriter writer, ObjectX value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value.Values, serializer);
            token.WriteTo(writer);
        }

        public override ObjectX ReadJson(JsonReader reader, ObjectX existingValue, JsonSerializer serializer)
        {
            var values = serializer.Deserialize<IDictionary<string, object>>(reader);
            return ObjectX.From(values);
        }
    }
}