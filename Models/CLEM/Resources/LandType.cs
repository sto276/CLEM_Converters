using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    class LandType : Node
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
