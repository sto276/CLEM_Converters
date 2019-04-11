using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    public class ActivityNode : Node
    {
        public object SelectedTab { get; set; } = null;

        public int OnPartialResourcesAvailableAction { get; set; } = 0;

        public ActivityNode(Node parent) : base(parent)
        {

        }
    }

    public class ActivitiesHolder : Node
    {
        public string LastShortfallResourceRequest { get; set; }

        public string LastActivityPerformed { get; set; }

        public ActivitiesHolder(ZoneCLEM parent) : base(parent)
        {
            Name = "Activities";
        }
    }

    public class ActivitiesFolder : ActivityNode
    {
        public ActivitiesFolder(ActivityNode parent) : base(parent)
        {

        }
    }

    public class ActivityTimerInterval : Node
    {
        public int Interval { get; set; } = 12;

        public int MonthDue { get; set; } = 12;

        public ActivityTimerInterval(Node parent) : base(parent)
        {

        }
    }
}
