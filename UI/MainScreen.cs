using Gtk;
using IAT;
using Resources;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace UI
{
    /// <summary>
    /// A Gtk interface for the main screen of the converter
    /// </summary>
    class MainScreen
    {
        public Window window = null;

        private Entry inentry = null;
        private Entry outentry = null;

        private Button aboutbtn = null;
        private Button convertbtn = null;
        private Button quitbtn = null;
        private Button inbtn = null;
        private Button outbtn = null;

        private CheckButton allcheck = null;
        private CheckButton sharecheck = null;
        private CheckButton paramcheck = null;

        private ComboBox combobox = null;

        private TreeView filelist = null;
       
        public MainScreen()
        {
            Builder builder = Tools.ReadGlade("MainMenu");

            // Main window
            window = (Window)builder.GetObject("window");
            window.Title = "CLEM File Converter";
            window.Icon = new Gdk.Pixbuf($"{Directory.GetCurrentDirectory()}/Resources/png/Maize.png");

            // Entry boxes
            inentry = (Entry)builder.GetObject("inentry");
            outentry = (Entry)builder.GetObject("outentry");

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            inentry.Text = path + "\\IAT_Stuff\\ExampleInputs";
            outentry.Text = path;

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
            sharecheck = (CheckButton)builder.GetObject("sharecheck");
            paramcheck = (CheckButton)builder.GetObject("paramcheck");

            allcheck.Toggled += OnAllToggled;

            // Combo boxes
            combobox = (ComboBox)builder.GetObject("combobox");
            Tools.AddStoreToCombo(combobox);
            combobox.AppendText("IAT");
            combobox.AppendText("NABSA");
            combobox.Active = 0;

            // Tree views            
            filelist = (TreeView)builder.GetObject("filelist");
            BuildFileList(filelist);
            AppendListItems(null, null);            

            builder.Dispose();
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
                AppendListItems(null, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAllToggled(object sender, EventArgs e)
        {
            DialogBox box = null;

            ListStore store = filelist.Model as ListStore;

            if (allcheck.Active)
            {
                foreach (var x in filelist)
                {
                    box = new DialogBox(x.ToString());      
                }
            }
            else
            {
                box = new DialogBox("Deactivated");
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filelist"></param>
        private void BuildFileList(TreeView filelist)
        {
            ListStore store = new ListStore(typeof(ToggleButton), typeof(string));

            CellRendererToggle toggle = new CellRendererToggle()
            {
                Activatable = true,
                Sensitive = true
            };            

            TreeViewColumn checks = new TreeViewColumn("Select", toggle, "togglebutton", 0);          
            TreeViewColumn names = new TreeViewColumn("Filenames", new CellRendererText(), "text", 1);

            filelist.AppendColumn(checks);
            filelist.AppendColumn(names);            

            filelist.Model = store;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppendListItems(object sender, EventArgs e)
        {
            string pattern;
            if (combobox.ActiveText == "IAT") pattern = "*.xlsx";
            else pattern = "*.nabsa";

            if (!Directory.Exists(inentry.Text))
            {
                new DialogBox("Input folder not found.");
                return;
            }

            string[] items = Directory.GetFiles(inentry.Text, pattern, SearchOption.TopDirectoryOnly);

            ToggleButton toggle = new ToggleButton()
            {
                Sensitive = true
            };


            ListStore store = filelist.Model as ListStore;
            store.Clear();

            foreach (string item in items)
            {
                TreeIter iter = store.Append();

                string path = Path.GetFileName(item);                
                filelist.Model.SetValue(iter, 0, toggle);
                filelist.Model.SetValue(iter, 1, path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAboutClicked(object sender, EventArgs e)
        {
            var icon = new Gdk.Pixbuf("../../Resources/Maize.png");

            AboutDialog about = new AboutDialog()
            {
                ProgramName = "CLEM File Converter",
                Version = "0.1",
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

            switch (combobox.ActiveText)
            {
                case "IAT":
                    string[] files = Directory.GetFiles(inentry.Text, "*.xlsx").Where(file => !file.Contains("~$")).ToArray();
                    Toolbox.OutDir = outentry.Text;
                    Terminal.RunConverter(files, sharecheck.Active, paramcheck.Active);
                    break;

                case "NABSA":
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
        private static void OnQuitClicked(object sender, EventArgs e)
        {
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
            selecter.selected += AppendListItems;

            selecter.window.ShowAll();
        }

        
    }
}
