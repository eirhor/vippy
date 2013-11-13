using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Geta.VippyWrapper.Helpers
{
    public static class VippyDeserializer
    {
        public const string JsonNamespace = "vippy";

        public static List<T> Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            try
            {
                JObject data = JObject.Parse(json);
                List<T> list = JsonConvert.DeserializeObject<List<T>>(data[JsonNamespace].ToString());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error when parsing json from Vippy. Errormessage:" + ex.Message);
            }
        }

        public static string DeserializeItem<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            JObject data = JObject.Parse(json);

            if (data != null)
            {
                return data[JsonNamespace].ToString();
            }

            return null;
        }
    }
}