using System.Collections.Generic;

namespace Models.CLEM.Reporting
{
    /// <summary>
    /// The base node for a report, this should not be instantiated directly
    /// </summary>
    class Report : Node
    {
        public List<string> ExperimentFactorNames = new List<string>();

        public List<string> ExperimentFactorValues = new List<string>();

        public List<string> VariableNames = new List<string>();

        public List<string> EventNames = new List<string>();
        public Report(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// Report the balance of resources
    /// </summary>
    class ReportResourceBalances : Report
    {       
        public ReportResourceBalances(Node parent) : base(parent)
        {
            Name = "ReportResourceBalances";
        }
    }

    /// <summary>
    /// Report the activities performed
    /// </summary>
    class ReportActivitiesPerformed : Report
    {
        public ReportActivitiesPerformed(Node parent) : base(parent)
        {
            Name = "ReportActivitiesPerformed";
        }
    }

    /// <summary>
    /// Report any shortfalls of a particular resource
    /// </summary>
    class ReportResourceShortfalls : Report
    {
        public ReportResourceShortfalls(Node parent) : base(parent)
        {
            Name = "ReportResourceShortfalls";
        }
    }

    /// <summary>
    /// Report the status of an arbitrary resource
    /// </summary>
    class ReportResourceLedger : Report
    {
        public ReportResourceLedger(Node parent) : base(parent)
        { }
    }
        
    /// <summary>
    /// Report the status of the ruminant herd
    /// </summary>
    public class ReportRuminantHerd : Node
    {
        public ReportRuminantHerd(Node parent) : base(parent)
        {
            Name = "ReportHerd";
        }
    }
}
