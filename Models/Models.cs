using System;

namespace Models
{
    /// <summary>
    /// Models the simulation clock
    /// </summary>
    public class Clock : Node
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Clock(Node parent) : base(parent)
        {
            Name = "Clock";
        }
    }

    /// <summary>
    /// A memorandum for the user
    /// </summary>
    public class Memo : Node
    {
        public string Text { get; set; }

        public Memo(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// A summary of the entire simulation
    /// </summary>
    public class Summary : Node
    {
        public bool CaptureErrors { get; set; } = true;

        public bool CaptureWarnings { get; set; } = true;

        public bool CaptureSummaryText { get; set; } = true;

        public Summary(Node parent) : base(parent)
        {
            Name = "summaryfile";
        }
    }
}
