using System.Collections.Generic;

namespace Models.CLEM
{
    using Resources;
    using Activities;
    using Reporting;
    public class ZoneCLEM : Node
    {
        public int RandomSeed { get; set; } = 1;

        public int ClimateRegion { get; set; }

        public int EcologicalIndicatorsCalculationMonth { get; set; } = 12;

        public double Area { get; set; }

        public double Slope { get; set; } = 0;

        private ResourcesHolder resources;

        private ActivitiesHolder activities;

        public ZoneCLEM(Node parent) : base(parent)
        {
            Name = "CLEM";

            resources = new ResourcesHolder(this);
            activities = new ActivitiesHolder(this);

            AddFiles();            
            Add(resources);
            Add(activities);
            AddReports();            
        }

        private void AddFiles()
        {           
            // Add the crop
            Add(new FileCrop(this)
            {
                FileName = Source.Name + "_FileCrop.prn",
                Name = "FileCrop"
            });

            // Add the crop residue
            Add(new FileCrop(this)
            {
                FileName = Source.Name + "_FileCropResidue.prn",
                Name = "FileCropResidue"
            });

            // Add the forage crop
            Add(new FileCrop(this)
            {
                FileName = Source.Name + "_FileForage.prn",
                Name = "FileForage"
            });
        }

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
                    "[Clock.CLEMEndOfTimeStep"
                }
            });

            reports.Add(new ReportActivitiesPerformed(reports));
            reports.Add(new ReportResourceShortfalls(reports));

            foreach(Node child in resources.Children)
            {
                string name = child.GetType().Name;

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

    public class CLEMFolder : Node
    {
        public bool ShowPageOfGraphs { get; set; } = true;

        public CLEMFolder(Node parent) : base(parent)
        { }
    }

    public class FileCrop : Node
    {
        public string FileName { get; set; }

        public string ExcelWorkSheetName { get; set; }

        public FileCrop(Node parent) : base(parent)
        { }
    }
    public class SummariseRuminantHerd : Node
    {
        public SummariseRuminantHerd(Node parent) : base(parent)
        {
            Name = "SummariseHerd";
        }
    }
}
