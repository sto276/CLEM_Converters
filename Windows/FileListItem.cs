using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows
{
    public partial class FileListItem : UserControl
    {
        public ComboBox Combo { get { return comboBox; } }

        public CheckBox Check { get { return checkBox; } }

        public FileListItem(string text)
        {
            InitializeComponent();
            checkBox.Text = text;
        }
    }
}
