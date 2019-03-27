using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    class ActivitiesHolder : Node
    {
        public string LastShortfallResourceRequest { get; set; }

        public string LastActivityPerformed { get; set; }

        public ActivitiesHolder(ZoneCLEM parent) : base(parent)
        {
            Name = "Activities";
        }
    }
}
