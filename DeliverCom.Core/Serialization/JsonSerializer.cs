using Newtonsoft.Json;

namespace DeliverCom.Core.Serialization
{
    public static class JsonSerializer
    {
        private static readonly JsonSerializerSettings _JsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };


        public static string Serialize(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, _JsonSerializerSettings);
            return json;
        }


        public static string Serialize(object obj, JsonSerializerSettings jsonSerializerSettings)
        {
            var json = JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            return json;
        }

        public static T Deserialize<T>(string value)
        {
            var obj = JsonConvert.DeserializeObject<T>(value, _JsonSerializerSettings);
            return obj;
        }

        public static object Deserialize(string value, Type type)
        {
            var obj = JsonConvert.DeserializeObject(value, type, _JsonSerializerSettings);
            return obj;
        }

        public static T Deserialize<T>(string value, JsonSerializerSettings jsonSerializerSettings)
        {
            var obj = JsonConvert.DeserializeObject<T>(value, jsonSerializerSettings);
            return obj;
        }

        public static object Deserialize(string value, Type type, JsonSerializerSettings jsonSerializerSettings)
        {
            var obj = JsonConvert.DeserializeObject(value, type, jsonSerializerSettings);
            return obj;
        }
    }
}