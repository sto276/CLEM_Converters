using Models.Core;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Xml;
using System.Xml.Linq;

namespace ReadIAT
{
    public static class Runner
    {
        public static void Run(IEnumerable<string> files)
        {
            Simulations simulations = new Simulations(null);

            foreach (string file in files)
            {
                IAT iat = new IAT(file);

                Simulation simulation = new Simulation(simulations)
                {
                    Name = iat.Name,
                    Source = iat
                };
                simulations.Children.Add(simulation);

                // Find all the parameter sheets in the IAT
                List<string> sheets = new List<string>();
                foreach (Sheet sheet in iat.Book.Workbook.Sheets)
                {
                    // Ensure a parameter sheet is selected
                    string name = sheet.Name.ToString();
                    if (!name.ToLower().Contains("param")) continue;
                    
                    // Write the resulting simulation to its own .apsimx file
                    if (split) WriteApsimx(simulation, iat.name, name);
                    // Or collect all the simulations in the IAT
                    else
                    {
                        if (join) folder.Add(simulation);
                        else simulations.Add(simulation);
                    }
                }

                // Files will already be written if split is true
                if (split) continue;

                // Write the .apsimx containing all simulations from an IAT
                // and reset the simulations container
                if (join)
                {
                    simulations.Add(folder);
                }
                else
                {
                    WriteApsimx(simulations, null, iat.name);
                    simulations = new XElement("name_is_ignored");
                }
            }

            if (join) WriteApsimx(simulations, null, "Simulations");

            Toolbox.CloseErrorLog();
        }
        }
    }
}
