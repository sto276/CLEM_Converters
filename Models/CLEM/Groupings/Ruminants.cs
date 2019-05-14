namespace Models.CLEM.Groupings
{
    /// <summary>
    /// The price of a group of ruminants (filters can be applied to the group)
    /// </summary>
    public class AnimalPriceGroup : Node
    {
        public int PricingStyle { get; set; } = 0;

        public double Value { get; set; } = 0.0;

        public int PurchaseOrSale { get; set; } = 0;

        public AnimalPriceGroup(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// Filters the present ruminants by parameter and value
    /// </summary>
    public class RuminantFilter : Node
    {
        public int Parameter { get; set; } = 0;

        public int Operator { get; set; } = 0;

        public string Value { get; set; } = "";

        public RuminantFilter(Node parent) : base(parent)
        { }
    }
}
