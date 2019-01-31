using Gtk;
using Resources;
using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    class Program
    {              
        static void Main(string[] args)
        {
            Tools.SetProjectDirectory();

            Application.Init();

            MainScreen screen = new MainScreen();
            screen.window.ShowAll();
            
            Application.Run();
        }

    }
}
