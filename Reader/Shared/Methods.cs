using Models.Core;
using Newtonsoft.Json;
using System.IO;


namespace Reader
{    public static partial class Shared
    {
        public static void WriteApsimX(Simulations simulations, string name)
        {
            StreamWriter stream = new StreamWriter($"{OutDir}\\{name}.apsimx");
            JsonWriter writer = new JsonTextWriter(stream)
            {
                CloseOutput = true,
                AutoCompleteOnClose = true
            };

            JsonSerializer serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects
            };
            serializer.Serialize(writer, simulations);

            writer.Close();
        }
    }
}
