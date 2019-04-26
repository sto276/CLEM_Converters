namespace Models.CLEM.Groupings
{
    public class AnimalPriceGroup : Node
    {
        public int PricingStyle { get; set; } = 0;

        public double Value { get; set; } = 0.0;

        public int PurchaseOrSale { get; set; } = 0;

        public AnimalPriceGroup(Node parent) : base(parent)
        { }
    }

    public class RuminantFilter : Node
    {
        public int Parameter { get; set; } = 0;

        public int Operator { get; set; } = 0;

        public string Value { get; set; } = "";

        public RuminantFilter(Node parent) : base(parent)
        { }
    }
}
