namespace Models.CLEM
{
    using Resources;
    using Activities;
    public class ZoneCLEM : Node
    {
        public int RandomSeed { get; set; } = 1;

        public int ClimateRegion { get; set; }

        public int EcologicalIndicatorsCalculationMonth { get; set; } = 12;

        public double Area { get; set; }

        public double Slope { get; set; } = 0;

        public ZoneCLEM(Node parent) : base(parent)
        {
            AddFiles();
            new ResourcesHolder(this);
            new ActivitiesHolder(this);
            AddReports();            
        }

        private void AddFiles()
        {           
            // Add the crop
            new FileCrop(this)
            {
                FileName = Source.Name + "_FileCrop.prn",
                Name = "FileCrop"
            };

            // Add the crop residue
            new FileCrop(this)
            {
                FileName = Source.Name + "_FileCropResidue.prn",
                Name = "FileCropResidue"
            };

            // Add the forage crop
            new FileCrop(this)
            {
                FileName = Source.Name + "_FileForage.prn",
                Name = "FileForage"
            };
        }

        private void AddReports()
        {
            new CLEMFolder(this)
            {

            };
        }
    }

    public class CLEMFolder : Node
    {
        public bool ShowPageOfGraphs { get; set; } = true;

        public CLEMFolder(Node parent) : base(parent)
        {

        }
    }

    public class FileCrop : Node
    {
        public string FileName { get; set; }

        public string ExcelWorkSheetName { get; set; }

        public FileCrop(Node parent) : base(parent)
        {

        }
    }

    public class SummariseRuminantHerd : Node
    {
        public SummariseRuminantHerd(Node parent) : base(parent)
        {
            Name = "SummariseHerd";
        }
    }

    public class ReportRuminantHerd : Node
    {
        public ReportRuminantHerd(Node parent) : base(parent)
        {
            Name = "ReportHerd";
        }
    }
}
