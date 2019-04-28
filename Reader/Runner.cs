using Models;
using Models.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Xml.Linq;
using System;

namespace Reader
{
    public static class Runner
    {
        public static void Run(IEnumerable<string> files, bool join, bool split)
        {
            IAT.Error.OpenLog("Simulations/log.txt");

            Simulations simulations = new Simulations(null);

            foreach (string file in files)
            {
                IAT iat = new IAT(file);

                Folder folder = new Folder(simulations) { Name = iat.Name };                                                

                // Find all the parameter sheets in the IAT
                List<string> sheets = new List<string>();
                foreach (Sheet sheet in iat.Book.Workbook.Sheets)
                {
                    // Ensure a parameter sheet is selected
                    string name = sheet.Name.ToString();
                    if (!name.ToLower().Contains("param")) continue;
                    iat.SetSheet(name);                    

                    // Write the resulting simulation to its own .apsimx file
                    if (split)
                    {
                        AttachIAT(simulations, iat);
                        WriteApsimX(simulations, iat.Name);

                        // Reset the simulations node after writing out
                        simulations = new Simulations(null);
                    }
                    // Or collect all the simulations in the IAT
                    else
                    {
                        if (join) AttachIAT(folder, iat);                        
                        else AttachIAT(simulations, iat);
                    }
                }

                // Files will already be written if split is true
                if (split) continue;

                // Collect all the IAT files in the same .apsimx file
                if (join) simulations.Children.Add(folder);          
                // Only gather parameter sets into the same .apsimx file
                else
                {
                    WriteApsimX(simulations, iat.Name);
                    simulations = new Simulations(null);
                }                
            }

            if (join) WriteApsimX(simulations, "simulations");

            IAT.Error.CloseLog();
        }

        private static void WriteApsimX(Simulations simulations, string name)
        {
            StreamWriter stream = new StreamWriter(IAT.OutDir + "/" + name + ".apsimx");
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

        private static void AttachIAT(Node node, IAT iat)
        {
            node.Source = iat;
            Simulation simulation = new Simulation(node);
            simulation.Name = iat.ParameterSheet.Name;

            node.Children.Add(simulation);
        }
    }
}
