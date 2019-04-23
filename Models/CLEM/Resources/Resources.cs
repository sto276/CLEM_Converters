using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    public class ResourcesHolder : Node
    {      
        public ResourcesHolder(ZoneCLEM parent) : base(parent)
        {
            Name = "Resources";

            new Land(this);
            new Labour(this);
            new RuminantHerd(this);
            new Finance(this);
            new AnimalFoodStore(this);
            new HumanFoodStore(this);
            new GrazeFoodStore(this);
            new ProductStore(this);
        }
    }    
}
