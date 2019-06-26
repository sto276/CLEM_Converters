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
        public static void OpenErrorLog()
        {
            ErrorStream = new FileStream(
                $"{OutDir}/ErrorLog.csv", 
                FileMode.OpenOrCreate, 
                FileAccess.ReadWrite,
                FileShare.None);

            ErrorWriter = new StreamWriter(ErrorStream);
            ErrorReader = new StreamReader(ErrorStream);

            ErrorCount = 0;
            if (ErrorReader.Peek() == -1)
            {                
                ErrorWriter.WriteLine("Error #, File name, Sheet, Table, Cause, Severity, Date");                   
            }
            else
            {                
                while (ErrorReader.ReadLine() != null)
                {
                    ErrorCount++;
                }
                ErrorCount--;
            }         
        }

        /// <summary>
        /// Writes an error to the log
        /// </summary>
        /// <param name="message">The message to be written</param>
        public static void WriteError(ErrorData ED)
        {
            ErrorCount++;
            ErrorWriter.WriteLine($"{ErrorCount}, {ED.FileName}, {ED.Sheet}, {ED.Table}, {ED.Message}, {ED.Severity}, {DateTime.Now}");
        }

        /// <summary>
        /// Closes the error log
        /// </summary>
        public static void CloseErrorLog()
        {
            ErrorWriter.Close();
            return;
        }

    }

    public struct ErrorData
    {
        public string FileName;

        public string FileType;

        public string Message;

        public string Severity;

        public string Table;

        public string Sheet;
    }

    public class ConversionException : Exception
    {
        public ConversionException() : base()
        { }
    }
}

