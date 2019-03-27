using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    class Land : Node
    {
        public string UnitsOfArea { get; set; } = "hectares";

        public double UnitsOfAreaToHaConversion { get; set; } = 1.0;

        public Land(ResourcesHolder parent) : base(parent)
        {
            Name = "Land";
        }
    }
}
