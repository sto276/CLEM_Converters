using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Clock : Node
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Clock(Node parent) : base(parent)
        {
            Name = "Clock";
        }
    }

    public class Memo : Node
    {
        public string Text { get; set; }

        public Memo(Node parent) : base(parent)
        {

        }
    }

    public class Summary : Node
    {
        public bool CaptureErrors { get; set; } = true;

        public bool CaptureWarning { get; set; } = true;

        public bool CaptureSummaryText { get; set; } = true;

        public Summary(Node parent) : base(parent)
        {

        }
    }
}
