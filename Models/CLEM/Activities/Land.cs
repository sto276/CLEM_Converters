namespace Models.CLEM.Activities   
{
    /// <summary>
    /// Models the management of pastoral land
    /// </summary>
    public class PastureActivityManage : Node
    {
        public string LandTypeNameToUse { get; set; } = "";

        public string FeedTypeName { get; set; } = "GrazeFoodStore.NativePasture";

        public double StartingAmount { get; set; } = 0;

        public double StartingStockingRate { get; set; } = 0;

        public double AreaRequested { get; set; } = 0;

        public bool UseAreaAvailable { get; set; } = true;

        public PastureActivityManage(Node parent) : base(parent)
        {
            Name = "ManagePasture";
        }
    }
}
