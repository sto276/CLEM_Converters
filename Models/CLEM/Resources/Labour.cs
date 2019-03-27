using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    class Labour : Node
    {
        public bool AllowAging { get; set; } = true;

        public Labour(ResourcesHolder parent) : base(parent)
        {
            Name = "Labour";
        }
    }

    class LabourType : Node
    {
        public double InitialAge { get; set; }

        public int Gender { get; set; }

        public int Individuals { get; set; }

        public string Units { get; set; }

        public LabourType(Labour parent) : base(parent)
        {

        }
    }

    class LabourAvailabilityList : Node
    {
        public LabourAvailabilityList(Labour parent) : base(parent)
        {

        }
    }

    class LabourAvailabilityItem : Node
    {
        public LabourAvailabilityItem(Labour parent) : base(parent)
        {

        }
    }
}
