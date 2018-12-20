namespace IAT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using DocumentFormat.OpenXml.Spreadsheet;

    /// <summary>
    /// Provides utility for the user to pick and choose
    /// which IAT files to convert, through a command terminal
    /// </summary>
    static class Terminal 
	{
        /// <summary>
        /// Entry point of the program
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
		{
            Console.WriteLine("IAT to CLEM file conversion.");
            Console.WriteLine("Missing/Incorrect IAT data may produce incomplete .apsimx files.\n");

            // Defaults to using the 'Simulations' directory if the input directory was invalid
            if (!Directory.Exists(Toolbox.InDir))
            {
                Console.WriteLine("Input directory not found.");
                if (!Directory.Exists("Simulations")) Directory.CreateDirectory("Simulations");
            }

            // Obtain all .xlsx files in the given directory, ignoring temporary files that might exist
            // .xlsx files in the input directory are assumed to be IAT files
            string[] dir = Directory.GetFiles(Toolbox.InDir, "*.xlsx").Where(file => !file.Contains("~$")).ToArray();                       

            // Check that files exist to convert
            if (dir.Length == 0)
            {
                Console.WriteLine("No files found in input directory.");
                return;
            }            

            // This loops until the user opts out (given a choice to do so each iteration)
            do
            {
                // Choose a file/s to convert
                int choice = PickFile(dir);
                
                // Write one or all simulations based on user choice
                if (choice == 0)
                {
                    bool same_file = YesNo("save all the simulations to the same .apsimx file");
                    bool same_sim = YesNo("place parameter sets from the same IAT file into the same simulation");

                    IAT iat = null;                    
                    XElement apsims = new XElement("irrelevant");

                    foreach (string file in dir)
                    {
                        // Load the IAT object
                        iat = PrepareIAT(file);

                        // Find all the parameter sheets in the IAT
                        List<string> sheets = new List<string>();
                        foreach (Sheet sheet in iat.book.Workbook.Sheets)
                        {
                            string name = sheet.Name.ToString();
                            if (name.ToLower().Contains("params")) sheets.Add(name);
                        }

                        // Generates a collection of all the CLEM objects
                        XElement clems = new XElement("irrelevant");
                        foreach (string sheet in sheets)
                        {
                            iat.SetSheet(sheet);
                            clems.Add(Simulation.GetCLEM(iat, sheet));
                        }

                        //
                        XElement sims = new XElement(iat.name.Replace(" ", ""));

                        // Add 1 simulation which contains the CLEM model for each sheet
                        if (same_sim) sims.Add(Simulation.GetSimulation(iat, clems));

                        // Alternatively, add 1 simulation per sheet, each containing 1 CLEM model
                        else
                        {
                            foreach (var clem in clems.Elements())
                            {
                                // Note: GetSimulation acts on the child elements,
                                // so we set up an element with the child we want to pass
                                XElement temp = new XElement("irrelevant", clem);
                                sims.Add(Simulation.GetSimulation(iat, temp));
                            }
                        }

                        // Each element in this object is a collection of all the simulations 
                        // generated from a single IAT file
                        apsims.Add(sims);

                        Toolbox.CloseErrorLog();
                    }

                    string path = "";
                    XmlTextWriter xtw = null;
                    if (same_file)
                    {
                        // Create a new object which has every simulation as sibling elements
                        XElement all = new XElement("irrelevant");
                        foreach (var apsim in apsims.Elements()) all.Add(apsim.Elements());

                        // Wrap all the simulations together
                        var apsimx = Simulation.GetApsimx(all);

                        xtw = Toolbox.MakeApsimX("CLEM_Simulations");

                        apsimx.WriteTo(xtw);
                        xtw.WriteEndDocument();
                        xtw.Close();
                    }
                    else
                    {
                        // Create a separate file for each simulation
                        foreach(var apsim in apsims.Elements())
                        {
                            // Create a separate folder for each base IAT
                            path = apsim.Elements().First().Name.ToString();
                            Directory.CreateDirectory(path);

                            //
                            foreach(var sim in apsim.Elements())
                            {
                                var zone = sim.Descendants("ZoneCLEM").First();
                                string name = zone.Descendants("Name").First().Value;
                                xtw = Toolbox.MakeApsimX($"{path}/{name}");

                                var apsimx = Simulation.GetApsimx(sim);

                                apsimx.WriteTo(xtw);
                                xtw.WriteEndDocument();
                                xtw.Close();

                            }
                        }
                    }
                    // If every file is converted, don't have to ask the user if more files need converting
                    break;
                }
                else
                {
                    bool same_sim = YesNo("place separate parameter sets into the same simulation");

                    // Prepare a single IAT, giving the user the option to choose which parameter sheets
                    IAT iat = PrepareIAT(dir[choice - 1]);

                    List<string> sheets = new List<string>();
                    foreach (Sheet sheet in iat.book.Workbook.Sheets)
                    {
                        string name = sheet.Name.ToString();
                        if (!name.ToLower().Contains("input")) sheets.Add(name);
                    }

                    WriteSim(iat, sheets, true);
                }
            }
            while (YesNo("attempt another conversion"));
            
            // Wait for user input before closing window
            Console.ReadKey();
            return;
        }

        /// <summary>
        /// Ask the user to choose which IAT files to convert
        /// '0' for all files, or list position of the specific file to convert.
        /// Repeat the process until the user gives valid input or opts out.
        /// </summary>
        /// <param name="dir">Collection of files in the directory</param>
        private static int PickFile(string[] dir)
        {
            do
            {
                // Display IAT files found in the directory
                Console.WriteLine("The following IAT files are available:\n");
                for (int i = 0; i < dir.Length; i++)
                {
                    Console.WriteLine($"  {i + 1}. {dir[i].Substring(Toolbox.InDir.Length + 1)}");
                }
                // Take user input
                Console.WriteLine("\nEnter the number corresponding to the file you wish to convert, or '0' for all files.");
                string input = Console.ReadLine();

                // If the user provides a valid choice, move to the next stage
                if (int.TryParse(input, out int choice)) return choice;
                else Console.WriteLine("Invalid input.");
            }
            while (YesNo("choose another IAT"));

            Console.WriteLine("No selection provided. Selecting first available file.");
            return 1;
        }

        private static IAT PrepareIAT(string name)
        {
            // Create the object
            IAT iat = new IAT(name);

            // Prepare the directory
            Directory.CreateDirectory($"{Toolbox.OutDir}\\{iat.name}");

            // Prepares the error log
            Toolbox.OpenErrorLog($"{Toolbox.OutDir}\\{iat.name}\\ErrorLog.txt");

            // Write the PRN files
            Grains.FileCrop(iat);
            Grains.FileResidue(iat);
            Forages.FileForage(iat);

            return iat;
        }

        /// <summary>
        /// Attempts to write a complete .apsimx, choosing parameter sheets from the desired IAT file
        /// </summary>
        /// <param name="IATname">The name of the IAT file.</param>
        /// <param name="choose">If the user gets to pick which parameter sheet to use</param>
        private static void WriteSim(IAT iat, List<string> sheets, bool choose = false)
        {           
            // Ask the user to choose one or all parameter sheets (if the option is available)
            int choice = 0;
            if (choose && (sheets.Count() > 1)) choice = PickSheet(sheets);            

            XElement clems = new XElement("");
            // Convert a single parameter sheet
            if (choice != 0)
            {
                string sheet = sheets[choice - 1];
                iat.SetSheet(sheet);
                clems.Add(Simulation.GetCLEM(iat, sheet));
                
            }
            // Covert all the parameter sheets
            else
            {
                foreach (string sheet in sheets)
                {
                    iat.SetSheet(sheet);
                    clems.Add(Simulation.GetCLEM(iat, sheet));
                }
            }

            

            Toolbox.CloseErrorLog();
            return;
        }

        private static int PickSheet(List<string> sheets)
        {
            // Repeat until valid input is provided or the user opts out
            do
            {
                // Display available parameter sheets
                Console.WriteLine("\nFound the following parameter sheets:\n");
                int n = 0;
                foreach (string sheet in sheets)
                {
                    n++;
                    Console.WriteLine($"  {n}. {sheet}");
                }

                // Asks the user to pick from the displayed sheets
                Console.WriteLine("\nEnter the number corresponding to the parameter sheet you wish to use, or '0' for all.");
                string input = Console.ReadLine();

                // Move to the next stage of conversion if valid input is provided
                if (int.TryParse(input, out int choice) && choice <= n) return choice;
                Console.WriteLine("Invalid selection.");
            }
            while (YesNo("choose another sheet"));

            Console.WriteLine("No selection provided. Converting first available sheet.");
            return 1;
        }

        /// <summary>
        /// Gives the user a yes/no choice
        /// </summary>
        /// <param name="choice">The yes/no choice given to the user</param>
        /// <returns>true if yes, false if else</returns>
        private static bool YesNo(string choice = "try again")
        {
            Console.WriteLine($"\nWould you like to {choice}? (y/n)");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();
            if (key.KeyChar == 'y') return true;
            else return false;
        }
    }
}