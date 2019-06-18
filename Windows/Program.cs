using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Windows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Converter converter = new Converter();

            Application.Run(converter);            
        }

        
    }

}
