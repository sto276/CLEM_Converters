using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    class Summary : Node
    {
        public bool CaptureErrors { get; set; } = true;

        public bool CaptureWarning { get; set; } = true;

        public bool CaptureSummaryText { get; set; } = true;

        public Summary(Node parent) : base(parent)
        {

        }
    }
}
