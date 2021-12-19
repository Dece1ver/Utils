using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoElma.Infrastructure
{
    public static class Util
    {
        public static void CreateConfig(string path, Settings settings)
        {
            JsonSerializer serializer = new();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using StreamWriter sw = new(path);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, settings);
        }
    }
}
