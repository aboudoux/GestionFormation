using GestionFormation.Kernel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GestionFormation.EventStore
{
    public class DomainEventJsonEventSerializer : IEventSerializer, IRuntimeDependency
    {
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        public DomainEventJsonEventSerializer() : this(null)
        {
            
        }

        public DomainEventJsonEventSerializer(ISerializationBinder binder = null)
        {
            _jsonSettings.SerializationBinder = binder ?? new DomainEventTypeBinder();
        }


        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, _jsonSettings);
        }

        public T Deserialize<T>(string data)
        {            
            var deserializedObject = JsonConvert.DeserializeObject(data, _jsonSettings);
            return (T)deserializedObject;
        }
    }
}