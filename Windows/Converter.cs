using Reader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Windows
{
    public partial class Converter : Form
    {        
        private List<string> nabsa = new List<string>();

        private List<Tuple<string, string>> iat = new List<Tuple<string, string>>();

        public Converter()
        {
            InitializeComponent();
            backgroundConverter.WorkerReportsProgress = true;
            backgroundConverter.WorkerSupportsCancellation = true;

            backgroundConverter.DoWork += new DoWorkEventHandler(BeginConversion);
            backgroundConverter.ProgressChanged += new ProgressChangedEventHandler(ProgressUpdate);
            backgroundConverter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompletedConversion);

            FormClosed += new FormClosedEventHandler(CloseConverter);
        }

        public void UpdateSettings()
        {
            var settings = ConverterSettings.Read();

            Shared.InDir = settings.InDirectory;
            Shared.OutDir = settings.OutDirectory;
            includeIAT.Checked = settings.IncludeIAT;
            groupSheets.Checked = settings.GroupSheets;
            groupSimulations.Checked = settings.GroupSims;
            includeNABSA.Checked = settings.IncludeNABSA;
        }

        public void UpdateFileList()
        {
            panel.Controls.Clear();

            List<FileListItem> items = new List<FileListItem>();

            var files = Directory.EnumerateFiles(Shared.InDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => !s.StartsWith("~") && 
                    (
                        (includeIAT.Checked && s.EndsWith(".xlsx")) ||
                        (includeNABSA.Checked && s.EndsWith(".nabsa"))
                    )
                );

            int i = 0;
            foreach (string file in files)
            {
                string label = Path.GetFileName(file);

                FileListItem item = new FileListItem(label)
                {
                    Anchor = AnchorStyles.Top,
                    Location = new Point(0, i * 25)
                };
                if (label.Contains(".nabsa")) item.Combo.Visible = false;

                if (label.Contains(".xlsx"))
                {
                    var sheets = GetSheets(file);
                    if (sheets != null) item.Combo.Items.AddRange(sheets);
                }

                items.Add(item);
                i++;
            }

            panel.Controls.AddRange(items.ToArray());
            panel.Refresh();
        }

        private string[] GetSheets(string file)
        {
            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(file, false))
                {
                    WorkbookPart part = document.WorkbookPart;

                    var sheets = part.Workbook.Descendants<Sheet>()
                        .Where(s => !s.Name.ToString().ToLower().Contains("input"))
                        .Select(s => s.Name.ToString())
                        .ToArray();

                    return sheets;
                }
            }
            catch (IOException)
            {

            }

            return null;
        }

        private void Converter_Load(object sender, EventArgs e)
        {            
            UpdateSettings();
            UpdateFileList();

            btnInput.ToolTipText = "Directory containing files to convert:\n" + Shared.InDir;
            btnOutput.ToolTipText = "Directory where output is saved:\n" + Shared.OutDir;
        }

        private void BtnInput_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = Shared.InDir;
            DialogResult result = folderBrowserDialog.ShowDialog();             

            if(result == DialogResult.OK)
            {
                Shared.InDir = folderBrowserDialog.SelectedPath;

                btnInput.ToolTipText = "Directory containing files to convert:\n" + Shared.InDir;
                UpdateFileList();
            }
        }

        private void BtnOutput_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = Shared.OutDir;
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Shared.OutDir = folderBrowserDialog.SelectedPath;
                btnOutput.ToolTipText = "Directory where output is saved:\n" + Shared.OutDir;
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            var items = panel.Controls.OfType<FileListItem>();

            bool check = true;
            if (!items.Any(i => i.Check.Checked == false)) check = false;

            foreach (var item in items) item.Check.Checked = check;
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            ToggleEnabled();

            // Find all the items which the user selected
            var selected = panel.Controls.OfType<FileListItem>()
                .Where(i => i.Check.Checked);

            // Reset trackers
            int sheets = 0;
            nabsa.Clear();
            iat.Clear();

            foreach (var selection in selected)
            {
                // Find the full file path
                string file = Shared.InDir + "/" + selection.Check.Text;

                if (file.EndsWith(".nabsa"))
                {
                    nabsa.Add(file);
                }
                else if(file.EndsWith(".xlsx"))
                {
                    string sheet = selection.Combo.Text;
                    iat.Add(new Tuple<string, string>(file, sheet));

                    if (sheet != "All") sheets++;
                    else
                    {
                        int count = selection.Combo.Items
                            .OfType<string>()
                            .Where(
                                s => s
                                .ToLower()
                                .Contains("param"))
                            .Count();

                        sheets += count;
                    }
                }
            }

            progressBar.Value = 0;
            progressBar.Maximum = iat.Count() + nabsa.Count() + sheets;

            // Start the asynchronous operation.
            backgroundConverter.RunWorkerAsync();                        
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {         
            // Cancel the asynchronous operation.
            backgroundConverter.CancelAsync();            
        }

        private void BeginConversion(object sender, EventArgs e)
        {
            Shared.Worker = sender as BackgroundWorker;

            IAT.GroupSheets = groupSheets.Checked;
            IAT.GroupSims = groupSimulations.Checked;
            IAT.Run(iat);
            NABSA.Run(nabsa);
        }

        private void ProgressUpdate(object sender, EventArgs e)
        {
            progressBar.PerformStep();
            progressBar.Refresh();
        }

        private void CompletedConversion(object sender, EventArgs e)
        {
            progressBar.Value = 0;
            ToggleEnabled();
        }
        
        private void CloseConverter(object sender, EventArgs e)
        {
            ConverterSettings settings = new ConverterSettings()
            {
                InDirectory = Shared.InDir,
                OutDirectory = Shared.OutDir,
                IncludeIAT = includeIAT.Checked,
                GroupSheets = groupSheets.Checked,
                GroupSims = groupSimulations.Checked,
                IncludeNABSA = includeNABSA.Checked
            };

            settings.Write();
        }

        private void ToggleEnabled()
        {
            btnCancel.Enabled = !btnCancel.Enabled;
            btnConvert.Enabled = !btnConvert.Enabled;
            toolStrip.Enabled = !toolStrip.Enabled;
            menuStrip.Enabled = !menuStrip.Enabled;
            panel.Enabled = !panel.Enabled;
        }
    }
}
