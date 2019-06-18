using System;
using System.ComponentModel;
using System.IO;

namespace Reader
{
    public static partial class Shared
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

    }
}
