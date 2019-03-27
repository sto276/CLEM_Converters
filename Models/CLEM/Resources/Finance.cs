using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    class Finance : Node
    {
        public string CurrencyName { get; set; }

        public Finance(ResourcesHolder parent) : base(parent)
        {
            Name = "Finance";
        }
    }
}
