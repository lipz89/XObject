using System;

using Newtonsoft.Json;

namespace DynamicObject
{
    public abstract class JsonConverter<T> : JsonConverter
    {
        public sealed override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            WriteJson(writer, (T)value, serializer);
        }

        public sealed override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ReadJson(reader, (T)existingValue, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public abstract void WriteJson(JsonWriter writer, T value, JsonSerializer serializer);

        public abstract T ReadJson(JsonReader reader, T existingValue, JsonSerializer serializer);
    }
}