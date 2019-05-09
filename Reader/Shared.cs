using Models;
using Models.Core;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;

namespace Reader
{
    public static class Shared
    {
        public static string OutDir { get; set; } = "Simulations";

        /// <summary>
        /// The error log file stream
        /// </summary>
        private static FileStream ErrorStream;

        /// <summary>
        /// The error log stream writer
        /// </summary>
        private static StreamWriter ErrorWriter;

        /// <summary>
        /// The number of errors made
        /// </summary>
        private static int ErrorCount = 0;

        public static void WriteApsimX(Simulations simulations, string name)
        {
            StreamWriter stream = new StreamWriter(OutDir + "/" + name + ".apsimx");
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

        /// <summary>
        /// Creates an error log to write to
        /// </summary>
        /// <param name="filename">The name of the error log</param>
        public static void OpenLog(string filename)
        {
            ErrorStream = new FileStream(OutDir + "/" + filename, FileMode.Create);
            ErrorWriter = new StreamWriter(ErrorStream);
            ErrorCount = 0;
        }

        /// <summary>
        /// Writes an error to the log
        /// </summary>
        /// <param name="msg">The message to be written</param>
        public static void Write(string msg, IAT iat)
        {
            ErrorCount++;
            ErrorWriter.WriteLine($"Error {ErrorCount}:");
            ErrorWriter.WriteLine("\t" + msg);
            ErrorWriter.WriteLine($"\tin {iat.Name}: {iat.ParameterSheet.Name}\n");
        }

        /// <summary>
        /// Closes the error log
        /// </summary>
        public static void CloseLog()
        {
            if (ErrorCount == 0)
            {
                ErrorWriter.WriteLine("No conversion errors detected. It is still advised to inspect the converted file before attempting to run the model.");
            }
            ErrorWriter.Close();
            return;
        }

        /// <summary>
        /// Shifts invalid starting characters to the end of the Name
        /// </summary>
        /// <param name="name">The name to sanitise</param>
        public static string SanitiseName(string name)
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
