using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestRP
{
    public static class JsonHelpers
    {
        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.None, // SDL Requirements dictate that we use this value
        };

        public static JsonSerializerSettings SerializerSettingsNoIndent = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None, // SDL Requirements dictate that we use this value
        };

        public static string SerializeObject(object obj, bool indent = true)
        {
            return indent ? JsonConvert.SerializeObject(obj, SerializerSettings) : JsonConvert.SerializeObject(obj, SerializerSettingsNoIndent);
        }

        public static T DeserializeObject<T>(string json, bool indent = true)
        {
            return indent ? JsonConvert.DeserializeObject<T>(json, SerializerSettings) : JsonConvert.DeserializeObject<T>(json, SerializerSettingsNoIndent);
        }

        public static Dictionary<string, string> DeserializeDict(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new Dictionary<string, string>();
            }

            return DeserializeObject<Dictionary<string, string>>(input);
        }

        public static HttpContent ConvertToHttpContent(object value)
        {
            if (value == null)
            {
                return null;
            }

            var json = SerializeObject(value);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
