using Gtk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Resources
{
    public static class Tools
    {
        /// <summary>
        /// Creates a builder from a glade file
        /// </summary>
        /// <param name="name">Path to the glade file</param>
        public static Builder ReadGlade(string name)
        {
            Builder builder = new Builder();

            string path = $"{Directory.GetCurrentDirectory()}/Resources/glade/{name}.glade";

            StreamReader reader = new StreamReader(path);
            string glade = reader.ReadToEnd();

            builder.AddFromString(glade);
            return builder;
        }

        /// <summary>
        /// Creates a new text list store for a combo box
        /// </summary>
        /// <param name="combo">The box to create a store for</param>
        public static void AddStoreToCombo(ComboBox combo)
        {
            // Remove any existing list
            combo.Clear();

            // Create a new renderer for the text in the box
            CellRendererText renderer = new CellRendererText();
            combo.PackStart(renderer, false);
            combo.AddAttribute(renderer, "text", 0);

            // Add a ListStore to the box
            ListStore store = new ListStore(typeof(string));
            combo.Model = store;
        }

        public static void SetProjectDirectory()
        {
            string location = Directory.GetCurrentDirectory();

            while(!File.Exists("CLEM_Converters.sln"))
            {
                location = Directory.GetParent(location).FullName;
                Directory.SetCurrentDirectory(location);
            }            
        }
    }
}
