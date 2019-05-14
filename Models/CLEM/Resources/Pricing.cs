namespace Models.CLEM.Resources
{
    /// <summary>
    /// Container for animal prices
    /// </summary>
    public class AnimalPricing : Node
    {
        public string PricingStyle { get; set; } = "perKg";

        public double SirePrice { get; set; } = 0.0;

        public AnimalPricing(Node parent) : base(parent)
        {
            Name = "AnimalPricing";
            Add(Source.GetAnimalPrices(this));
        }
    }
    
}
