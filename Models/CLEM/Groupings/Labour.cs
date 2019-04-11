using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Groupings
{
    class LabourFilter : Node
    {
        public int Parameter { get; set; }

        public int Operator { get; set; }

        public string Value { get; set; }

        public LabourFilter(Node parent) : base(parent)
        {
            Name = "LabourFilter";
        }
    }

    class LabourFilterGroup : Node
    {
        public LabourFilterGroup(Node parent) : base(parent)
        {

        }
    }
}
