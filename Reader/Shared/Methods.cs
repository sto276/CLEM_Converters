using Models.Core;
using Newtonsoft.Json;
using System.IO;

namespace Reader
{
    public static partial class Shared
    {
        public static void WriteApsimX(Simulations simulations, string name)
        {
            using (StreamWriter stream = new StreamWriter($"{OutDir}\\{name}.apsimx"))
            using (JsonWriter writer = new JsonTextWriter(stream))
            {
                writer.CloseOutput = true;
                writer.AutoCompleteOnClose = true;                

                JsonSerializer serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Objects                    
                };
                serializer.Serialize(writer, simulations);

                serializer = null;
                simulations.Dispose();
            }
        }
    }
}
