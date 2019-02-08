namespace IAT
{
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using DocumentFormat.OpenXml.Spreadsheet;

    /// <summary>
    /// Provides utility for the user to pick and choose
    /// which IAT files to convert, through a command terminal
    /// </summary>
    public static class Terminal 
	{
        public static void Main(string[] args)
        {
            Tools.SetProjectDirectory();

            Console.WriteLine("IAT to CLEM file conversion.");
            Console.WriteLine("Missing/Incorrect IAT data may produce incomplete .apsimx files.\n");

            ManageDirectories();

            // Obtain all .xlsx files in the given directory, ignoring temporary files that might exist
            // .xlsx files in the input directory are assumed to be IAT files
            string[] dir = Directory.GetFiles(Toolbox.InDir, "*.xlsx").Where(file => !file.Contains("~$")).ToArray();

            // Check that files exist to convert
            if (dir.Length == 0)
            {
                Console.WriteLine("No files found in input directory.");
                return;
            }

            Start(dir);
        }

        /// <summary>
        /// Entry point of the program
        /// </summary>
        /// <param name="args"></param>
        public static void Start(string[] dir)
		{                      
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
                                       
                    RunConverter(dir, same_sim, same_file);                   
                    
                    // If every file is converted, don't have to ask the user if more files need converting
                    break;
                }
                else
                {
                    bool same_sim = YesNo("place separate parameter sets into the same simulation");

                    // Prepare a single IAT, giving the user the option to choose which parameter sheets
                    IAT iat = PrepareIAT(dir[choice - 1], Toolbox.OutDir);

                    List<string> sheets = new List<string>();
                    foreach (Sheet sheet in iat.book.Workbook.Sheets)
                    {
                        string name = sheet.Name.ToString();
                        if (name.ToLower().Contains("param")) sheets.Add(name);
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
        /// Runs the conversion process for the given IAT files and options
        /// </summary>
        /// <param name="files">Files to convert</param>
        /// <param name="same_file">Shared .apsimx file option</param>
        /// <param name="same_sim">Shared simulation option</param>
        public static void RunConverter(IEnumerable<string> files, bool same_file, bool same_sim)
        {
            IAT iat = null;
            XElement simulations = new XElement("name_is_ignored");

            foreach (string file in files)
            {
                // Load the IAT object
                iat = PrepareIAT(file, Toolbox.OutDir);

                // Find all the parameter sheets in the IAT
                List<string> sheets = new List<string>();
                foreach (Sheet sheet in iat.book.Workbook.Sheets)
                {
                    string name = sheet.Name.ToString();
                    if (name.ToLower().Contains("params")) sheets.Add(name);
                }

                // Generates a collection of all the CLEM objects
                XElement clems = new XElement("name_is_ignored");
                foreach (string sheet in sheets)
                {
                    iat.SetSheet(sheet);
                    clems.Add(Simulation.GetCLEM(iat, sheet));
                }

                // Sanitise the simulation name
                string xname = Toolbox.SanitiseXName(iat.name);
                XElement simulation = new XElement(xname);

                // Add one simulation containing a CLEM model per sheet
                if (same_sim)
                {
                    simulation.Add(Simulation.GetSimulation(iat, clems));
                }
                // Add one simulation per sheet, containing a CLEM model each
                else
                {
                    foreach (var clem in clems.Elements())
                    {
                        // Note: GetSimulation acts on the child elements,
                        // so we set up an element with the child we want to pass
                        XElement temp = new XElement("name_is_ignored", clem);
                        simulation.Add(Simulation.GetSimulation(iat, temp));
                    }
                }

                // Each element in this object is a collection of all the simulations 
                // generated from a single IAT file
                simulations.Add(simulation);

                Toolbox.CloseErrorLog();                
            }

            if (same_file)
            {
                WriteApsimx(simulations, "Simulations", "All_sims");
            }
            else
            {
                WriteApsimxs(simulations);
            }
        }

        /// <summary>
        /// Write all the simulations to individual .apsimx files
        /// </summary>
        /// <param name="simulations"></param>
        public static void WriteApsimxs(XElement simulations)
        {
            string path = "";
            string name = "";

            // Create a separate file for each simulation
            foreach (var simulation in simulations.Elements())
            {
                // Refers directly to the location of the file name in the XML
                path = simulation.Elements().First().Elements().First().Value;
                name = simulation.Descendants("Name").First().Value;
                WriteApsimx(simulation, path, name);                
            }
        }

        /// <summary>
        /// Creates an .apsimx file using the given simulations
        /// </summary>
        /// <param name="simulations">The simulations in the file</param>
        /// <param name="path">The path to the file</param>
        /// <param name="name">The name of the .apsimx file</param>
        private static void WriteApsimx(XElement simulations, string path, string name)
        {
            XmlTextWriter xtw = null;

            var apsimx = Simulation.GetApsimx(simulations);
            xtw = Toolbox.MakeApsimX(path, name);

            apsimx.WriteTo(xtw);
            xtw.WriteEndDocument();
            xtw.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IAT PrepareIAT(string path, string folder)
        {
            // Create the object
            IAT iat = new IAT(path);

            // Prepare the directory
            Directory.CreateDirectory($"{folder}/{iat.name}");

            // Prepares the error log
            Toolbox.OpenErrorLog($"{folder}/{iat.name}/ErrorLog.txt");

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

            XElement clems = new XElement("clems");
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

        /// <summary>
        /// Provides the user the choice to select new input/output directories
        /// </summary>
        private static void ManageDirectories()
        {
            // Create the simulations directory
            if (!Directory.Exists("Simulations"))
            {
                Directory.CreateDirectory("Simulations");
            }

            // Select a new input directory
            if (YesNo("select a new input directory"))
            {
                Console.WriteLine("Enter the full directory path: \n");
                Toolbox.InDir = Console.ReadLine();
            }

            // Validate the new input directory, return to default if invalid
            if (!Directory.Exists(Toolbox.InDir))
            {
                Console.WriteLine("Input directory not found. Switching to default directory.");
                Toolbox.InDir = "Simulations";
            }

            // Select a new output directory
            if (YesNo("select a new output directory"))
            {
                Console.WriteLine("Enter the full directory path: \n");
                Toolbox.OutDir = Console.ReadLine();
            }

            // Validate the new output directory, return to default if invalid
            if (!Directory.Exists(Toolbox.InDir))
            {
                Console.WriteLine("Input directory not found. Switching to default directory.");
                if (!Directory.Exists("Simulations"))
                {
                    Directory.CreateDirectory("Simulations");
                }
                Toolbox.OutDir = "Simulations";
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheets"></param>
        /// <returns></returns>
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