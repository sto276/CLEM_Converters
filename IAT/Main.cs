namespace IAT
{
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
    public static class Converter 
	{
        /// <summary>
        /// Runs the conversion process for the given IAT files and options
        /// </summary>
        /// <param name="files">Files to convert</param>
        /// <param name="join">Shared .apsimx file option</param>
        /// <param name="same_sim">Shared simulation option</param>
        public static void Run(IEnumerable<string> files, bool join, bool split)
        {
            IAT iat = null;
            XElement simulations = new XElement("name_is_ignored");

            // Prepares the error log
            Toolbox.OpenErrorLog($"{Toolbox.OutDir}/ErrorLog.txt");
            
            foreach (string file in files)
            {
                // Load the IAT object
                iat = PrepareIAT(file, Toolbox.OutDir);

                XElement folder = new XElement(
                    "Folder", 
                    new XElement("Name", iat.name)
                    );

                // Find all the parameter sheets in the IAT
                List<string> sheets = new List<string>();
                foreach (Sheet sheet in iat.book.Workbook.Sheets)
                {
                    // Ensure a parameter sheet is selected
                    string name = sheet.Name.ToString();
                    if (!name.ToLower().Contains("param")) continue;

                    // Prepare the PRN files
                    string path = iat.name;
                    if (split) path = "";
                    XElement prns = Sim.GetFiles(iat, path);

                    // Generate the CLEM model from the parameter sheet
                    iat.SetSheet(name);                    
                    XElement clem = Sim.GetCLEM(iat, prns);

                    // Wrap the CLEM model in a simulation
                    XElement simulation = Sim.GetSimulation(iat, clem);

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

        /// <summary>
        /// Creates an .apsimx file using the given simulations
        /// </summary>
        /// <param name="simulations">The simulations in the file</param>
        /// <param name="path">The path to the file</param>
        /// <param name="name">The name of the .apsimx file</param>
        private static void WriteApsimx(XElement simulations, string path, string name)
        {
            XmlTextWriter xtw = null;

            var apsimx = Sim.GetApsimx(simulations);
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

            // Write the PRN files
            Grains.FileCrop(iat);
            Grains.FileResidue(iat);
            Forages.FileForage(iat);

            return iat;
        }

        ///// <summary>
        ///// Attempts to write a complete .apsimx, choosing parameter sheets from the desired IAT file
        ///// </summary>
        ///// <param name="IATname">The name of the IAT file.</param>
        ///// <param name="choose">If the user gets to pick which parameter sheet to use</param>
        //private static void WriteSim(IAT iat, List<string> sheets, bool choose = false)
        //{           
        //    // Ask the user to choose one or all parameter sheets (if the option is available)
        //    int choice = 0;
        //    if (choose && (sheets.Count() > 1)) choice = PickSheet(sheets);            

        //    XElement clems = new XElement("clems");
        //    // Convert a single parameter sheet
        //    if (choice != 0)
        //    {
        //        string sheet = sheets[choice - 1];
        //        iat.SetSheet(sheet);
        //        clems.Add(Simulation.GetCLEM(iat));
                
        //    }
        //    // Covert all the parameter sheets
        //    else
        //    {
        //        foreach (string sheet in sheets)
        //        {
        //            iat.SetSheet(sheet);
        //            clems.Add(Simulation.GetCLEM(iat));
        //        }
        //    }            

        //    Toolbox.CloseErrorLog();
        //    return;
        //}

        ///// <summary>
        ///// Provides the user the choice to select new input/output directories
        ///// </summary>
        //private static void ManageDirectories()
        //{
        //    // Create the simulations directory
        //    if (!Directory.Exists("Simulations"))
        //    {
        //        Directory.CreateDirectory("Simulations");
        //    }

        //    // Select a new input directory
        //    if (YesNo("select a new input directory"))
        //    {
        //        Console.WriteLine("Enter the full directory path: \n");
        //        Toolbox.InDir = Console.ReadLine();
        //    }

        //    // Validate the new input directory, return to default if invalid
        //    if (!Directory.Exists(Toolbox.InDir))
        //    {
        //        Console.WriteLine("Input directory not found. Switching to default directory.");
        //        Toolbox.InDir = "Simulations";
        //    }

        //    // Select a new output directory
        //    if (YesNo("select a new output directory"))
        //    {
        //        Console.WriteLine("Enter the full directory path: \n");
        //        Toolbox.OutDir = Console.ReadLine();
        //    }

        //    // Validate the new output directory, return to default if invalid
        //    if (!Directory.Exists(Toolbox.InDir))
        //    {
        //        Console.WriteLine("Input directory not found. Switching to default directory.");
        //        if (!Directory.Exists("Simulations"))
        //        {
        //            Directory.CreateDirectory("Simulations");
        //        }
        //        Toolbox.OutDir = "Simulations";
        //    }
        //}

        ///// <summary>
        ///// Ask the user to choose which IAT files to convert
        ///// '0' for all files, or list position of the specific file to convert.
        ///// Repeat the process until the user gives valid input or opts out.
        ///// </summary>
        ///// <param name="dir">Collection of files in the directory</param>
        //private static int PickFile(string[] dir)
        //{
        //    do
        //    {
        //        // Display IAT files found in the directory
        //        Console.WriteLine("The following IAT files are available:\n");
        //        for (int i = 0; i < dir.Length; i++)
        //        {
        //            Console.WriteLine($"  {i + 1}. {dir[i].Substring(Toolbox.InDir.Length + 1)}");
        //        }
        //        // Take user input
        //        Console.WriteLine("\nEnter the number corresponding to the file you wish to convert, or '0' for all files.");
        //        string input = Console.ReadLine();

        //        // If the user provides a valid choice, move to the next stage
        //        if (int.TryParse(input, out int choice)) return choice;
        //        else Console.WriteLine("Invalid input.");
        //    }
        //    while (YesNo("choose another IAT"));

        //    Console.WriteLine("No selection provided. Selecting first available file.");
        //    return 1;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sheets"></param>
        ///// <returns></returns>
        //private static int PickSheet(List<string> sheets)
        //{
        //    // Repeat until valid input is provided or the user opts out
        //    do
        //    {
        //        // Display available parameter sheets
        //        Console.WriteLine("\nFound the following parameter sheets:\n");
        //        int n = 0;
        //        foreach (string sheet in sheets)
        //        {
        //            n++;
        //            Console.WriteLine($"  {n}. {sheet}");
        //        }

        //        // Asks the user to pick from the displayed sheets
        //        Console.WriteLine("\nEnter the number corresponding to the parameter sheet you wish to use, or '0' for all.");
        //        string input = Console.ReadLine();

        //        // Move to the next stage of conversion if valid input is provided
        //        if (int.TryParse(input, out int choice) && choice <= n) return choice;
        //        Console.WriteLine("Invalid selection.");
        //    }
        //    while (YesNo("choose another sheet"));

        //    Console.WriteLine("No selection provided. Converting first available sheet.");
        //    return 1;
        //}

        ///// <summary>
        ///// Gives the user a yes/no choice
        ///// </summary>
        ///// <param name="choice">The yes/no choice given to the user</param>
        ///// <returns>true if yes, false if else</returns>
        //private static bool YesNo(string choice = "try again")
        //{
        //    Console.WriteLine($"\nWould you like to {choice}? (y/n)");
        //    ConsoleKeyInfo key = Console.ReadKey();
        //    Console.WriteLine();
        //    if (key.KeyChar == 'y') return true;
        //    else return false;
        //}

    }
}