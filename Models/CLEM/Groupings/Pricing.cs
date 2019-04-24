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
}
