using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM
{
    class FileCrop : Node
    {
        public string FileName { get; set; }

        public string ExcelWorkSheetName { get; set; }

        public FileCrop(Node parent) : base(parent)
        {

        }
    }
}
