using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    class LabourRequirement : Node
    {
        public double LabourPerUnit { get; set; } = 0.75;

        public double UnitSize { get; set; } = 25.0;

        public bool WholeUnitBlocks { get; set; } = false;

        public int UnitType { get; set; } = 5;

        public double MinimumPerPerson { get; set; } = 1.0;

        public double MaximumPerPerson { get; set; } = 100.0;

        public bool LabourShortfallAffectsActivity { get; set; } = false;

        public bool ApplyToAll { get; set; } = false;        

        public LabourRequirement(Node parent) : base(parent)
        {

        }
    }    

    
}
