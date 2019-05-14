namespace Models.CLEM.Groupings
{
    /// <summary>
    /// Filters the labourers by the selected parameter and value
    /// </summary>
    public class LabourFilter : Node
    {
        public int Parameter { get; set; } = 0;

        public int Operator { get; set; } = 0;

        public string Value { get; set; } = "";

        public LabourFilter(Node parent) : base(parent)
        {
            Name = "LabourFilter";
        }
    }

    /// <summary>
    /// The result of a collection of individual labour filters
    /// </summary>
    public class LabourFilterGroup : Node
    {
        public LabourFilterGroup(Node parent) : base(parent)
        { }
    }
}
