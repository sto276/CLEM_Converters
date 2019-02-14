using Gtk;
using IAT;
using NABSA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UI
{
    /// <summary>
    /// A Gtk interface for the main screen of the converter
    /// </summary>
    class MainScreen
    {
        public Window window = null;

        private Builder builder = null;

        private Entry inentry = null;
        private Entry outentry = null;

        private Button aboutbtn = null;
        private Button convertbtn = null;
        private Button quitbtn = null;
        private Button inbtn = null;
        private Button outbtn = null;

        private CheckButton allcheck = null;
        private CheckButton joincheck = null;
        private CheckButton splitcheck = null;

        private ComboBox combobox = null;

        private VBox listbox = null;

        private string path = null;
       
        public MainScreen()
        {
            builder = Tools.ReadGlade("MainMenu");

            // Main window
            window = (Window)builder.GetObject("window");
            window.Title = "CLEM File Converter";
            window.Icon = new Gdk.Pixbuf($"{Directory.GetCurrentDirectory()}/UI/Resources/Maize.png");
            window.Destroyed += OnQuitClicked;

            // Entry boxes
            inentry = (Entry)builder.GetObject("inentry");
            outentry = (Entry)builder.GetObject("outentry");

            path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            outentry.Text = path + "\\IAT_Stuff\\ExampleOutputs";
            path += "\\IAT_Stuff\\ExampleInputs";
            inentry.Text = path;            

            inentry.Changed += OnInentryChanged;

            // Buttons
            aboutbtn = (Button)builder.GetObject("aboutbtn");
            convertbtn = (Button)builder.GetObject("convertbtn");
            quitbtn = (Button)builder.GetObject("quitbtn");
            inbtn = (Button)builder.GetObject("inbtn");
            outbtn = (Button)builder.GetObject("outbtn");

            // Button events
            aboutbtn.Clicked += OnAboutClicked;
            convertbtn.Clicked += OnConvertClicked;
            quitbtn.Clicked += OnQuitClicked;
            inbtn.Clicked += OnInClicked;
            outbtn.Clicked += OnOutClicked;

            // Check buttons
            allcheck = (CheckButton)builder.GetObject("allcheck");
            joincheck = (CheckButton)builder.GetObject("joincheck");
            splitcheck = (CheckButton)builder.GetObject("splitcheck");

            allcheck.Toggled += OnAllToggled;
            joincheck.Toggled += OnJoinToggled;

            // Combo boxes
            combobox = (ComboBox)builder.GetObject("combobox");
            Tools.AddStoreToCombo(combobox);
            combobox.AppendText("IAT");
            combobox.AppendText("NABSA");
            combobox.Active = 0;

            combobox.Changed += OnComboChanged;

            // VBoxes
            listbox = (VBox)builder.GetObject("listbox");
            SetListItems(null, null);            
        }

        private void OnJoinToggled(object sender, EventArgs e)
        {
            if (joincheck.Active)
            {
                splitcheck.Active = false;
                splitcheck.Sensitive = false;
            }
            else
            {
                splitcheck.Sensitive = true;
            }
                
        }

        private void OnComboChanged(object sender, EventArgs e)
        {
            SetListItems(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInentryChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(inentry.Text))
            {
                path = inentry.Text;               

                SetListItems(null, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAllToggled(object sender, EventArgs e)
        {
            foreach (CheckButton child in listbox.AllChildren)
            {
                child.Active = allcheck.Active;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetListItems(object sender, EventArgs e)
        {
            string pattern;
            if (combobox.ActiveText == "IAT") pattern = "*.xlsx";
            else pattern = "*.nabsa";

            // Clear existing children                
            foreach (CheckButton child in listbox.AllChildren)
            {
                listbox.Remove(child);
            }

            string[] items = Directory.GetFiles(inentry.Text, pattern, SearchOption.TopDirectoryOnly);
            foreach (string item in items)
            {
                string label = Path.GetFileName(item);

                CheckButton check = new CheckButton()
                {
                    Sensitive = true,
                    Visible = true,
                    Label = label
                };

                listbox.PackStart(check, false, false, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAboutClicked(object sender, EventArgs e)
        {
            var icon = new Gdk.Pixbuf($"{Directory.GetCurrentDirectory()}/UI/Resources/Maize.png");

            AboutDialog about = new AboutDialog()
            {
                ProgramName = "CLEM File Converter",
                Version = "1.0",
                Copyright = "(c) CSIRO",
                Comments = "A tool for converting IAT & NABSA files to CLEM files (run through ApsimX)",
                Logo = icon,
                Icon = icon
            };

            about.Run();
            about.Destroy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConvertClicked(object sender, EventArgs e)
        {
            // Ensure the output directory exists
            if (!Directory.Exists(outentry.Text))
            {
                try
                {
                    Directory.CreateDirectory(outentry.Text);
                }
                catch
                {
                    new DialogBox("Invalid output directory.");
                    return;
                }
            }

            List<string> files = new List<string>();
            foreach (CheckButton child in listbox.AllChildren)
            {
                if (child.Active)
                {
                    files.Add(path + "/" + child.Label);
                }                
            }           

            switch (combobox.ActiveText)
            {                
                case "IAT":
                    Toolbox.OutDir = outentry.Text;
                    IAT.Converter.Run(files, joincheck.Active, splitcheck.Active);
                    break;

                case "NABSA":
                    NABSA.Converter.Run(files);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQuitClicked(object sender, EventArgs e)
        {
            Detach();
            Application.Quit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOutClicked(object sender, EventArgs e)
        {
            outbtn.Sensitive = false;

            SelectFolder selecter = new SelectFolder(outbtn, outentry);
            selecter.window.ShowAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInClicked(object sender, EventArgs e)
        {
            inbtn.Sensitive = false;

            SelectFolder selecter = new SelectFolder(inbtn, inentry);
            selecter.selected += SetListItems;

            selecter.window.ShowAll();
        }

        private void Detach()
        {
            window.Destroyed -= OnQuitClicked;
            inentry.Changed -= OnInentryChanged;
            aboutbtn.Clicked -= OnAboutClicked;
            convertbtn.Clicked -= OnConvertClicked;
            quitbtn.Clicked -= OnQuitClicked;
            inbtn.Clicked -= OnInClicked;
            outbtn.Clicked -= OnOutClicked;
            allcheck.Toggled -= OnAllToggled;
        }
    }
}
