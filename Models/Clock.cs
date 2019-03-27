using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    class Clock : Node
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Clock(Node parent) : base(parent)
        {

        }
    }
}
