using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    class ProductStore : Node
    {
        public ProductStore(ResourcesHolder parent) : base(parent)
        {
            Name = "ProductStore";
        }
    }

    class ProductStoreType : Node
    {
        public double StartingAmount { get; set; }

        public string Units { get; set; } = "kg";

        public ProductStoreType(ProductStore parent) : base(parent)
        {

        }
    }
}
