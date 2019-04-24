using System.Collections.Generic;

namespace Models.CLEM.Reporting
{
    class Report : Node
    {
        public List<string> ExperimentFactorNames = new List<string>();

        public List<string> ExperimentFactorValues = new List<string>();

        public List<string> VariableNames = new List<string>();

        public List<string> EventNames = new List<string>();
        public Report(Node parent) : base(parent)
        { }
    }

    class ReportResourceBalances : Report
    {       
        public ReportResourceBalances(Node parent) : base(parent)
        {
            Name = "ReportResourceBalances";
        }
    }

    class ReportActivitiesPerformed : Report
    {
        public ReportActivitiesPerformed(Node parent) : base(parent)
        {
            Name = "ReportActivitiesPerformed";
        }
    }

    class ReportResourceShortfalls : Report
    {
        public ReportResourceShortfalls(Node parent) : base(parent)
        {
            Name = "ReportResourceShortfalls";
        }
    }

    class ReportResourceLedger : Report
    {
        public ReportResourceLedger(Node parent) : base(parent)
        { }
    }
}
