using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Groupings
{
    public class AnimalPriceGroup : Node
    {
        public int PricingStyle { get; set; }

        public double Value { get; set; }

        public int PurchaseOrSale { get; set; }

        public AnimalPriceGroup(Node parent) : base(parent)
        {

        }
    }
}
