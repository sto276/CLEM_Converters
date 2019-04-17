using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace IATReader
{
    public partial class IAT
    {
        /// <summary>
        /// Contains methods for error tracking in an IAT file read
        /// </summary>
        private static class Error
        {
            /// <summary>
            /// The error log file stream
            /// </summary>
            private static FileStream stream;

            /// <summary>
            /// The error log stream writer
            /// </summary>
            private static StreamWriter writer;

            /// <summary>
            /// The number of errors made
            /// </summary>
            private static int count = 0;

            /// <summary>
            /// Creates an error log to write to
            /// </summary>
            /// <param name="filename">The name of the error log</param>
            public static void OpenLog(string filename)
            {
                stream = new FileStream(filename, FileMode.Create);
                writer = new StreamWriter(stream);
                count = 0;
            }

            /// <summary>
            /// Writes an error to the log
            /// </summary>
            /// <param name="msg">The message to be written</param>
            public static void Write(string msg, IAT iat)
            {
                count++;
                writer.WriteLine($"Error {count}:");
                writer.WriteLine("\t" + msg);
                writer.WriteLine($"\tin {iat.name}: {iat.sheet.Name}\n");
            }

            /// <summary>
            /// Closes the error log
            /// </summary>
            public static void CloseLog()
            {
                if (count == 0)
                {
                    writer.WriteLine("No conversion errors detected. It is still advised to inspect the converted file before attempting to run the model.");
                }
                writer.Close();
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
}