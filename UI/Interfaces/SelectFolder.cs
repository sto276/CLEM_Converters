using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    /// <summary>
    /// A Gtk interface for selecting a folder
    /// </summary>
    class SelectFolder
    {
        public Window window = null;

        public event EventHandler selected;

        private FileChooserWidget chooser = null;
        private Button selectbtn = null;
        private Button cancelbtn = null;

        private Widget caller = null;
        private Entry entry = null;

        public SelectFolder(Widget caller, Entry entry)
        {
            this.caller = caller;
            this.entry = entry;

            if ((caller == null) || (entry == null)) throw new ArgumentNullException();
          
            Builder builder = Tools.ReadGlade("SelectFolder");

            // Window
            window = (Window)builder.GetObject("window");
            window.Title = "";           

            // File chooser
            chooser = (FileChooserWidget)builder.GetObject("chooser");            
            chooser.SetCurrentFolder(entry.Text);

            // Buttons
            selectbtn = (Button)builder.GetObject("selectbtn");
            cancelbtn = (Button)builder.GetObject("cancelbtn");

            // Button events
            selectbtn.Clicked += OnSelectClicked;
            cancelbtn.Clicked += OnCancelClicked;

            builder.Dispose();
        }

        /// <summary>
        /// Handels the select button click event
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        private void OnSelectClicked(object sender, EventArgs e)
        {
            entry.Text = chooser.CurrentFolder;

            if (selected != null) selected.Invoke(this, EventArgs.Empty);
           
            caller.Sensitive = true;
            window.Destroy();
        }

        /// <summary>
        /// Handles the cancel button click event
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        private void OnCancelClicked(object sender, EventArgs e)
        {
            caller.Sensitive = true;
            window.Destroy();            
        }
    }
}
