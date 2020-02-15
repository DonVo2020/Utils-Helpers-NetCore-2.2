using MongoDB.Bson;
using Newtonsoft.Json;
using System;

namespace MongoHelperJsonConvert
{
    public class ObjectIdConverter : JsonConverter
    {
        public static ObjectIdConverter Default { get; }

        static ObjectIdConverter()
        {
            Default = new ObjectIdConverter();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            ObjectId.TryParse(reader.Value as string, out var result);
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectId).IsAssignableFrom(objectType);
        }
    }
}
