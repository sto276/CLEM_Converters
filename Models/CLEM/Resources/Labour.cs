namespace Models.CLEM.Resources
{
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

    public class LabourType : Node
    {
        public double InitialAge { get; set; }

        public int Gender { get; set; }

        public int Individuals { get; set; }

        public string Units { get; set; }

        public LabourType(Labour parent) : base(parent)
        {

        }
    }

    public class LabourAvailabilityList : Node
    {
        public LabourAvailabilityList(Labour parent) : base(parent)
        {
            Name = "LabourAvailabilityList";
            Add(Source.GetAvailabilityItems(this));
        }
    }

    public class LabourAvailabilityItem : Node
    {
        public double Value { get; set; }

        public LabourAvailabilityItem(LabourAvailabilityList parent) : base(parent)
        {
            
        }
    }
}
