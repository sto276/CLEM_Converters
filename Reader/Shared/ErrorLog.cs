using System;
using System.IO;

namespace Reader
{
    public static partial class Shared
    {
        /// <summary>
        /// Creates an error log to write to
        /// </summary>
        /// <param name="filename">The name of the error log</param>
        public static void OpenLog()
        {
            ErrorStream = new FileStream($"{OutDir}/ErrorLog.csv", FileMode.Append);
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

