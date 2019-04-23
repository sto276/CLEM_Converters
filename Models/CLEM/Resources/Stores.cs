using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{   
    public class AnimalFoodStore : Node
    {
        public AnimalFoodStore(Node parent) : base(parent)
        {
            Name = "AnimalFoodStore";
            Children.AddRange(Source.GetAnimalStoreTypes(this));
            Children.Add(Source.GetCommonFoodStore(this));
        }
    }

    public class AnimalFoodStoreType : Node
    {
        public double DMD { get; set; } = 0;

        public double Nitrogen { get; set; } = 0;

        public double StartingAmount { get; set; } = 0;

        public AnimalFoodStoreType(Node parent) : base(parent)
        {

        }
    }

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

    public class GrazeFoodStore : Node
    {
        public GrazeFoodStore(Node parent) : base(parent)
        {
            Name = "GrazeFoodStore";
            Children.Add(Source.GetGrazeFoodStore(this));
        }
    }

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

    public class HumanFoodStore : Node
    {
        public HumanFoodStore(Node parent) : base(parent)
        {
            Name = "HumanFoodStore";
        }
    }

    public class HumanFoodStoreType : Node
    {
        public double StartingAge { get; set; } = 0;

        public double StartingAmount { get; set; } = 0;

        public HumanFoodStoreType(Node parent) : base(parent)
        {

        }
    }

    public class ProductStore : Node
    {
        public ProductStore(ResourcesHolder parent) : base(parent)
        {
            Name = "ProductStore";
            Children.AddRange(Source.GetProductStoreTypes(this));
        }
    }

    public class ProductStoreType : Node
    {
        public double StartingAmount { get; set; }

        public string Units { get; set; } = "kg";

        public ProductStoreType(ProductStore parent) : base(parent)
        {

        }
    }
}
