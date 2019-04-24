namespace Models.CLEM.Resources
{
    public class Land : Node
    {
        public string UnitsOfArea { get; set; } = "hectares";

        public double UnitsOfAreaToHaConversion { get; set; } = 1.0;

        public Land(ResourcesHolder parent) : base(parent)
        {
            Name = "Land";
            Add(Source.GetLandTypes(this));
        }
    }

    public class LandType : Node
    {
        public double LandArea { get; set; }

        public double PortionBuildings { get; set; }

        public double ProportionOfTotalArea { get; set; }

        public int SoilType { get; set; }

        public LandType(Land parent) : base(parent)
        {

        }
    }

}
