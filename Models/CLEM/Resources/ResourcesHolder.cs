using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    class ResourcesHolder : Node
    {      
        public ResourcesHolder(ZoneCLEM parent) : base(parent)
        {
            Name = "Resources";
        }
    }    
}
