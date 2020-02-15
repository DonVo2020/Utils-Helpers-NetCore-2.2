using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CryptoTests.TestUtils
{
    internal class JsonFormatter : IFormatter
    {
        private readonly JsonSerializer serializer;

        public SerializationBinder Binder { get; set; }
        public StreamingContext Context { get; set; }
        public ISurrogateSelector SurrogateSelector { get; set; }

        public JsonFormatter()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver
                {
                    SerializeCompilerGeneratedMembers = true
                },
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            serializer = JsonSerializer.Create(settings);
        }

        public object Deserialize(Stream serializationStream)
        {
            JsonTextReader reader = new JsonTextReader(new StreamReader(serializationStream));
            return serializer.Deserialize(reader);
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            JsonTextWriter writer = new JsonTextWriter(new StreamWriter(serializationStream));
            serializer.Serialize(writer, graph);
            writer.Flush();
        }
    }
}
