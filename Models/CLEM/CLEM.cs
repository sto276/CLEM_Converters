using System.Collections.Generic;

namespace Models.CLEM
{
    using Resources;
    using Activities;
    using Reporting;

    /// <summary>
    /// Container for a CLEM model
    /// </summary>
    public class ZoneCLEM : Node
    {
        public int RandomSeed { get; set; } = 1;

        public int ClimateRegion { get; set; }

        public int EcologicalIndicatorsCalculationMonth { get; set; } = 12;

        public double Area { get; set; }

        public double Slope { get; set; } = 0;

        public ZoneCLEM(Node parent) : base(parent)
        {
            Name = "CLEM";

            Add(new Memo(this)
            {
                Name = "Default parameters",
                Text = "In the case that the source file is missing values, " +
                "most parameters in the simulation have default values. " +
                "It is recommended to ensure the validity of all parameters before " +
                "running the simulation."
            });
            Add(Source.GetFiles(this));          
            Add(new ResourcesHolder(this));
            Add(new ActivitiesHolder(this));

            AddReports();            
        }

        /// <summary>
        /// Adds a series of reports to the CLEM model
        /// </summary>
        private void AddReports()
        {
            var reports = new CLEMFolder(this)
            {
                Name = "Reports"
            };

            reports.Add(new ReportResourceBalances(reports)
            {
                VariableNames = new List<string>()
                {
                    "[Clock].Today",
                    "AnimalFoodStore"
                },
                EventNames = new List<string>()
                {
                    "[Clock].CLEMEndOfTimeStep"
                }
            });
            reports.Add(new ReportActivitiesPerformed(reports));
            reports.Add(new ReportResourceShortfalls(reports));

            foreach(Node child in SearchTree<ResourcesHolder>(this).Children)
            {
                string name = child.Name;

                reports.Add(new ReportResourceLedger(reports)
                {
                    VariableNames = new List<string>()
                    {
                        name
                    },
                    Name = name
                });
            }

            Add(reports);
        }
    }

    /// <summary>
    /// A generic container for models inside CLEM
    /// </summary>
    public class CLEMFolder : Node
    {
        public bool ShowPageOfGraphs { get; set; } = true;

        public CLEMFolder(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// Contains reference to source data
    /// </summary>
    public class FileCrop : Node
    {
        public string FileName { get; set; }

        public string ExcelWorkSheetName { get; set; }

        public FileCrop(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// Contains reference to source data
    /// </summary>
    public class FileSQLiteGRASP : Node
    {
        public string FileName { get; set; } = "";

        public FileSQLiteGRASP(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// Summary of the ruminant herd
    /// </summary>
    public class SummariseRuminantHerd : Node
    {
        public SummariseRuminantHerd(Node parent) : base(parent)
        {
            Name = "SummariseHerd";
        }
    }
}
