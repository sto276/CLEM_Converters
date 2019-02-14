using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace IAT
{   
    /// <summary>
    /// Contains various miscellaneous methods for
    /// aiding in IAT to CLEM conversion.
    /// </summary>
    public static class Toolbox
    {
        /// <summary>
        /// The directory to look for IAT files in
        /// </summary>
        public static string InDir { get; set; } = "Simulations";

        /// <summary>
        /// The directory to write CLEM simulations to
        /// </summary>
        public static string OutDir { get; set; } = "Simulations";

        /// <summary>
        /// The error log file stream
        /// </summary>
        private static FileStream error_fs;

        /// <summary>
        /// The error log stream writer
        /// </summary>
        private static StreamWriter error_sw;

        /// <summary>
        /// The number of errors made
        /// </summary>
        private static int error_count = 0;

        /// <summary>
        /// Creates and formats the xml document (.apsimx) for a simulation.
        /// </summary>
        /// <param name="folder">The sub-directory containing all simulations produced from an IAT file</param>
        /// <param name="name">The name of the .apsimx file</param>
        /// <returns>The XmlTextWriter that the .apsimx is written to</returns>
        public static XmlTextWriter MakeApsimX(string folder, string name)
        {
            // Ensure the file has a location to be saved
            Directory.CreateDirectory($"{OutDir}\\{folder}");

            // Create the .apsimx file in the directory
            FileStream fs = new FileStream($"{OutDir}\\{folder}\\{name}.apsimx", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            // Format the document
            XmlTextWriter xtw = new XmlTextWriter(sw)
            {
                Formatting = Formatting.Indented,
                Indentation = 4
            };

            xtw.WriteStartDocument();
            return xtw;
        }

        /// <summary>
        /// Creates an error log to write to
        /// </summary>
        /// <param name="filename">The name of the error log</param>
        public static void OpenErrorLog(string filename)
        {
            error_fs = new FileStream(filename, FileMode.Create);
            error_sw = new StreamWriter(error_fs);
            error_count = 0;
            return;
        }

        /// <summary>
        /// Writes an error to the log
        /// </summary>
        /// <param name="msg">The message to be written</param>
        public static void WriteError(string msg, IAT iat)
        {
            error_count++;
            error_sw.WriteLine($"Error {error_count}:");
            error_sw.WriteLine("\t" + msg);
            error_sw.WriteLine($"\tin {iat.name}: {iat.sheet.Name}\n");
            return;
        }

        /// <summary>
        /// Closes the error log
        /// </summary>
        public static void CloseErrorLog()
        {
            if (error_count == 0)
            {
                error_sw.WriteLine("No conversion errors detected. It is still advised to inspect the converted file before attempting to run the model.");
                Console.WriteLine("Conversions completed without error.");
            }
            else Console.WriteLine($"Conversions completed with {error_count} errors. See the error log for details.");
            Console.WriteLine();

            error_sw.Close();
            return;
        }

        /// <summary>
        /// Shifts invalid starting characters to the end of the XName
        /// </summary>
        /// <param name="name">The name to sanitise</param>
        public static string SanitiseXName(string name)
        {
            // Replace all whitespace, brackets, and underscores
            name = Regex.Replace(name, @"[\(\)\{\}\[\]_\s+]", "");

            // Matches non letter characters
            Regex reg = new Regex("[^a-zA-Z]");

            // Move invalid starting characters (like numbers) to the tail of the name
            // (preserving order to keep information where possible)
            string suffix = "";
            while (reg.IsMatch(name[0].ToString()))
            {
                suffix += name[0];
                name = name.Remove(0, 1);
            }            
            name += suffix;

            return name;
        }

    }
}