using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    public class Labour : Node
    {
        public bool AllowAging { get; set; } = true;

        public Labour(ResourcesHolder parent) : base(parent)
        {
            Name = "Labour";
            Source.GetLabourTypes(this);
            new LabourAvailabilityList(this);
        }
    }

    public class LabourType : Node
    {
        public double InitialAge { get; set; }

        public int Gender { get; set; }

        public int Individuals { get; set; }

        public string Units { get; set; }

        public LabourType(Labour parent) : base(parent)
        {

        }
    }

    public class LabourAvailabilityList : Node
    {
        public LabourAvailabilityList(Labour parent) : base(parent)
        {
            Source.GetAvailabilityItems(this);
        }
    }

    public class LabourAvailabilityItem : Node
    {
        public LabourAvailabilityItem(Labour parent) : base(parent)
        {
            Source.GetLabourFilters(this);
        }
    }
}
