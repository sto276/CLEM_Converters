namespace Models.CLEM.Resources
{   
    /// <summary>
    /// Container for different feed given to ruminants
    /// </summary>
    public class AnimalFoodStore : Node
    {
        public AnimalFoodStore(Node parent) : base(parent)
        {
            Name = "AnimalFoodStore";
            Add(Source.GetAnimalStoreTypes(this));
            Add(Source.GetCommonFoodStore(this));
        }
    }

    /// <summary>
    /// Models a generic fodder type
    /// </summary>
    public class AnimalFoodStoreType : Node
    {
        public double DMD { get; set; } = 0.01;

        public string Units { get; set; } = "kg";

        public double Nitrogen { get; set; } = 0.01;

        public double StartingAmount { get; set; } = 0.01;

        public AnimalFoodStoreType(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// Models the food type of shared land
    /// </summary>
    public class CommonLandFoodStoreType : Node
    {
        public double NToDMDCoefficient { get; set; } = 0;

        public double NToDMDIntercept { get; set; } = 0;

        public double NToDMDCrudeProteinDenominator { get; set; } = 0;

        public double Nitrogen { get; set; } = 0;

        public double MinimumNitrogen { get; set; } = 0;

        public double MinimumDMD { get; set; } = 0;

        public string PastureLink { get; set; } = "GrazeFoodStore.NativePasture";

        public double NitrogenReductionFromPasture { get; set; } = 0;

        public CommonLandFoodStoreType(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// Container for feed types found in grazing land
    /// </summary>
    public class GrazeFoodStore : Node
    {
        public GrazeFoodStore(Node parent) : base(parent)
        {
            Name = "GrazeFoodStore";
            Add(Source.GetGrazeFoodStore(this));
        }
    }

    /// <summary>
    /// Models a generic food type sourced from grazing land
    /// </summary>
    public class GrazeFoodStoreType : Node
    {
        public double NToDMDCoefficient { get; set; } = 0.0;

        public double NToDMDIntercept { get; set; } = 0.0;

        public double NToDMDCrudeProteinDenominator { get; set; } = 0.0;

        public double GreenNitrogen { get; set; } = 0.0;

        public double DecayNitrogen { get; set; } = 0.0;

        public double MinimumNitrogen { get; set; } = 0.0;

        public double DecayDMD { get; set; } = 0.0;

        public double MinimumDMD { get; set; } = 0.0;

        public double DetachRate { get; set; } = 0.0;

        public double CarryoverDetachRate { get; set; } = 0.0;

        public double IntakeTropicalQualityCoefficient { get; set; } = 0.0;

        public double IntakeQualityCoefficient { get; set; } = 0.0;

        public string Units { get; set; }

        public GrazeFoodStoreType(Node parent) : base(parent)
        {
            Name = "NativePasture";
        }
    }

    /// <summary>
    /// Container for food consumed by humans
    /// </summary>
    public class HumanFoodStore : Node
    {
        public HumanFoodStore(Node parent) : base(parent)
        {
            Name = "HumanFoodStore";
            Add(Source.GetHumanStoreTypes(this));
        }
    }

    /// <summary>
    /// Models a generic type of human food
    /// </summary>
    public class HumanFoodStoreType : Node
    {
        public double StartingAge { get; set; } = 0;

        public double StartingAmount { get; set; } = 0;

        public HumanFoodStoreType(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// Container for arbitrary products
    /// </summary>
    public class ProductStore : Node
    {
        public ProductStore(ResourcesHolder parent) : base(parent)
        {
            Name = "ProductStore";
            Add(Source.GetProductStoreTypes(this));
        }
    }

    /// <summary>
    /// Models an arbitrary product type
    /// </summary>
    public class ProductStoreType : Node
    {
        public double StartingAmount { get; set; } = 0.0;

        public string Units { get; set; } = "kg";

        public ProductStoreType(ProductStore parent) : base(parent)
        { }
    }
}
