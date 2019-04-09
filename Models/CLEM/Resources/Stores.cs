using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    class ProductStore : Node
    {
        public ProductStore(ResourcesHolder parent) : base(parent)
        {
            Name = "ProductStore";
        }
    }

    class ProductStoreType : Node
    {
        public double StartingAmount { get; set; }

        public string Units { get; set; } = "kg";

        public ProductStoreType(ProductStore parent) : base(parent)
        {

        }
    }

    class AnimalFoodStore : Node
    {
        public AnimalFoodStore(Node parent) : base(parent)
        {
            Name = "AnimalFoodStore";
        }
    }

    class AnimalFoodStoreType : Node
    {
        public AnimalFoodStoreType(Node parent) : base(parent)
        {

        }
    }

    class GrazeFoodStore : Node
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

        public GrazeFoodStore(Node parent) : base(parent)
        {

        }
    }


}
