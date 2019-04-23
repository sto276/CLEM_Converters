using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Groupings
{
    public class LabourFilter : Node
    {
        public int Parameter { get; set; } = 0;

        public int Operator { get; set; } = 0;

        public string Value { get; set; }

        public LabourFilter(Node parent) : base(parent)
        {
            Name = "LabourFilter";
        }
    }

    public class LabourFilterGroup : Node
    {
        public LabourFilterGroup(Node parent) : base(parent)
        {

        }
    }
}
