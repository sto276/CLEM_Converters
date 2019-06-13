using Reader;

namespace Windows
{
    partial class Converter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Converter));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iATToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.includeIAT = new System.Windows.Forms.ToolStripMenuItem();
            this.groupSheets = new System.Windows.Forms.ToolStripMenuItem();
            this.groupSimulations = new System.Windows.Forms.ToolStripMenuItem();
            this.nABSAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.includeNABSA = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnInput = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOutput = new System.Windows.Forms.ToolStripButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConvert = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.panel = new System.Windows.Forms.Panel();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundConverter = new System.ComponentModel.BackgroundWorker();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(484, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iATToolStripMenuItem,
            this.nABSAToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // iATToolStripMenuItem
            // 
            this.iATToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.includeIAT,
            this.groupSheets,
            this.groupSimulations});
            this.iATToolStripMenuItem.Name = "iATToolStripMenuItem";
            this.iATToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.iATToolStripMenuItem.Text = "IAT";
            // 
            // includeIAT
            // 
            this.includeIAT.Checked = true;
            this.includeIAT.CheckOnClick = true;
            this.includeIAT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeIAT.Name = "includeIAT";
            this.includeIAT.Size = new System.Drawing.Size(171, 22);
            this.includeIAT.Text = "Include";
            // 
            // groupSheets
            // 
            this.groupSheets.Checked = true;
            this.groupSheets.CheckOnClick = true;
            this.groupSheets.CheckState = System.Windows.Forms.CheckState.Checked;
            this.groupSheets.Name = "groupSheets";
            this.groupSheets.Size = new System.Drawing.Size(171, 22);
            this.groupSheets.Text = "Group sheets";
            // 
            // groupSimulations
            // 
            this.groupSimulations.CheckOnClick = true;
            this.groupSimulations.Name = "groupSimulations";
            this.groupSimulations.Size = new System.Drawing.Size(171, 22);
            this.groupSimulations.Text = "Group simulations";
            // 
            // nABSAToolStripMenuItem
            // 
            this.nABSAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.includeNABSA});
            this.nABSAToolStripMenuItem.Name = "nABSAToolStripMenuItem";
            this.nABSAToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.nABSAToolStripMenuItem.Text = "NABSA";
            // 
            // includeNABSA
            // 
            this.includeNABSA.BackColor = System.Drawing.SystemColors.Control;
            this.includeNABSA.Checked = true;
            this.includeNABSA.CheckOnClick = true;
            this.includeNABSA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeNABSA.Name = "includeNABSA";
            this.includeNABSA.Size = new System.Drawing.Size(113, 22);
            this.includeNABSA.Text = "Include";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSelect,
            this.toolStripSeparator1,
            this.btnInput,
            this.toolStripSeparator2,
            this.btnOutput});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(484, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnSelect
            // 
            this.btnSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.Image")));
            this.btnSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(57, 22);
            this.btnSelect.Text = "Select all";
            this.btnSelect.ToolTipText = "Select/Deselect all files";
            this.btnSelect.Click += new System.EventHandler(this.BtnSelect_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnInput
            // 
            this.btnInput.Image = global::Windows.Properties.Resources.Folder;
            this.btnInput.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(106, 22);
            this.btnInput.Text = "Input Directory";
            this.btnInput.Click += new System.EventHandler(this.BtnInput_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOutput
            // 
            this.btnOutput.Image = global::Windows.Properties.Resources.Folder;
            this.btnOutput.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(116, 22);
            this.btnOutput.Text = "Output Directory";
            this.btnOutput.Click += new System.EventHandler(this.BtnOutput_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(397, 426);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(316, 426);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(75, 23);
            this.btnConvert.TabIndex = 4;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.BtnConvert_Click);
            // 
            // progressBar
            // 
            this.progressBar.ForeColor = System.Drawing.Color.Lime;
            this.progressBar.Location = new System.Drawing.Point(12, 426);
            this.progressBar.Maximum = 10;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(298, 23);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 5;
            // 
            // panel
            // 
            this.panel.AutoScroll = true;
            this.panel.Location = new System.Drawing.Point(12, 52);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(460, 368);
            this.panel.TabIndex = 6;
            // 
            // Converter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 461);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Converter";
            this.Text = "CLEM Converter";
            this.Load += new System.EventHandler(this.Converter_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iATToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem includeIAT;
        private System.Windows.Forms.ToolStripMenuItem groupSheets;
        private System.Windows.Forms.ToolStripMenuItem groupSimulations;
        private System.Windows.Forms.ToolStripMenuItem nABSAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnSelect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnInput;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnOutput;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem includeNABSA;
        private System.ComponentModel.BackgroundWorker backgroundConverter;
    }
}