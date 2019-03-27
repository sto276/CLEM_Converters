using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM
{
    class ZoneCLEM : Node
    {
        public int RandomSeed { get; set; } = 1;

        public int ClimateRegion { get; set; }

        public int EcologicalIndicatorsCalculationMonth { get; set; } = 12;

        public double Area { get; set; }

        public double Slope { get; set; } = 0;

        public ZoneCLEM(Node parent) : base(parent)
        {

        }
    }
}
