using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    class ResourceActivitySell : Node
    {
        public string AccountName { get; set; }

        public string ResourceTypeName { get; set; }

        public double AmountReserved { get; set; }

        public int OnPartialResourcesAvailableAction { get; set; }

        public ResourceActivitySell(Node parent) : base(parent)
        {

        }
    }
}
