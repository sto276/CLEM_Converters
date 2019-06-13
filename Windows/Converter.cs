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
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Windows
{
    public partial class Converter : Form
    {
        public Panel Panel { get { return panel; } }        

        public Converter()
        {
            InitializeComponent();
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

                if (label.Contains(".xlsx")) item.Combo.Items.AddRange(GetSheets(file));

                items.Add(item);
                i++;
            }

            panel.Controls.AddRange(items.ToArray());
            panel.Refresh();
        }

        private string[] GetSheets(string file)
        {
            SpreadsheetDocument doc = SpreadsheetDocument.Open(file, false);
            WorkbookPart part = doc.WorkbookPart;

            var sheets = part.Workbook.Descendants<Sheet>()
                .Where(s => !s.Name.ToString().ToLower().Contains("input"))
                .Select(s => s.Name.ToString())
                .ToArray();

            doc.Close();

            return sheets;
        }

        private void Converter_Load(object sender, EventArgs e)
        {
            IAT.Progress += Update_Progress;
            NABSA.Progress += Update_Progress;

            btnInput.ToolTipText = "Directory containing files to convert:\n" + Shared.InDir;
            btnOutput.ToolTipText = "Directory where output is saved:\n" + Shared.OutDir;

            UpdateFileList();
        }

        private void Update_Progress(object sender, EventArgs e)
        {
            progressBar.PerformStep();
            progressBar.Refresh();
        }

        private void BtnInput_Click(object sender, EventArgs e)
        {
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
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Shared.OutDir = folderBrowserDialog.SelectedPath;
                btnOutput.ToolTipText = "Directory where output is saved:\n" + Shared.OutDir;
            }
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            var selected = panel.Controls.OfType<FileListItem>()
                .Where(i => i.Check.Checked)
                .Select(i => Shared.InDir + "/" + i.Check.Text);
            
            var iat = selected.Where(s => s.EndsWith(".xlsx"));
            var nabsa = selected.Where(s => s.EndsWith(".nabsa"));

            progressBar.Maximum = iat.Count() * 5 + nabsa.Count();

            IAT.Run(iat, groupSheets.Checked, groupSimulations.Checked);
            NABSA.Run(nabsa);
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            var items = panel.Controls.OfType<FileListItem>();

            bool check = true;
            if (!items.Any(i => i.Check.Checked == false)) check = false;

            foreach (var item in items) item.Check.Checked = check;
        }
    }
}
