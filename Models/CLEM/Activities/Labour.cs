using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    class LabourRequirement : Node
    {
        public double LabourPerUnit { get; set; }

        public double UnitSize { get; set; }

        public bool WholeUnitBlocks { get; set; } = false;

        public int UnitType { get; set; }

        public double MinimumPerPerson { get; set; }

        public double MaximumPerPerson { get; set; }

        public bool LabourShortfallAffectsActivity { get; set; } = true;

        public bool ApplyToAll { get; set; } = false;        

        public LabourRequirement(Node parent) : base(parent)
        {

        }
    }    

    
}
