namespace Models.CLEM.Resources
{
    /// <summary>
    /// Container for labour resources
    /// </summary>
    public class Labour : Node
    {
        public bool AllowAging { get; set; } = true;

        public Labour(ResourcesHolder parent) : base(parent)
        {
            Name = "Labour";
            Add(Source.GetLabourTypes(this));
            Add(new LabourAvailabilityList(this));
        }
    }

    /// <summary>
    /// Models a generic labour resource type
    /// </summary>
    public class LabourType : Node
    {
        public double InitialAge { get; set; }

        public int Gender { get; set; } = 0;

        public int Individuals { get; set; } = 1;

        public string Units { get; set; }

        public LabourType(Labour parent) : base(parent)
        { }
    }

    /// <summary>
    /// Models the availability of a given labour resource
    /// </summary>
    public class LabourAvailabilityList : Node
    {
        public LabourAvailabilityList(Labour parent) : base(parent)
        {
            Name = "LabourAvailabilityList";
            Add(Source.GetAvailabilityItems(this));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LabourAvailabilityItem : Node
    {
        public double Value { get; set; }

        public LabourAvailabilityItem(LabourAvailabilityList parent) : base(parent)
        {
            
        }
    }
}
