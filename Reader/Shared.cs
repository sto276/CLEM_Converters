using Models;
using Models.Core;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;

namespace Reader
{
    public static class Shared
    {
        public static BackgroundWorker Worker = null;

        public static string InDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public static string OutDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

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

        /// <summary>
        /// Creates an error log to write to
        /// </summary>
        /// <param name="filename">The name of the error log</param>
        public static void OpenLog(string filename)
        {
            ErrorStream = new FileStream($"{OutDir}/{filename}.csv", FileMode.Append);
            ErrorWriter = new StreamWriter(ErrorStream);
            ErrorWriter.WriteLine("Error #, File name, Message, Severity, Table, Sheet, Date");
            ErrorCount = 0;            
        }

        /// <summary>
        /// Writes an error to the log
        /// </summary>
        /// <param name="message">The message to be written</param>
        public static void Write(ConversionError CE)
        {            
            ErrorWriter.WriteLine($"{ErrorCount}, {CE.FileName}, {CE.Message}, {CE.Severity}, {CE.Table}, {CE.Sheet}, {DateTime.Now}\n");
            ErrorCount++;
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

    }

    public struct ConversionError
    {
        public string FileName;

        public string FileType;

        public string Message;

        public string Severity;

        public string Table;

        public string Sheet;
    }
}
