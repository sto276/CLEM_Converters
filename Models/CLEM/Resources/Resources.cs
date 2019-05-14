namespace Models.CLEM.Resources
{
    /// <summary>
    /// Container for all resources
    /// </summary>
    public class ResourcesHolder : Node
    {      
        public ResourcesHolder(ZoneCLEM parent) : base(parent)
        {
            Name = "Resources";

            Add(new Land(this));
            Add(new Labour(this));
            Add(new RuminantHerd(this));
            Add(new Finance(this));
            Add(new AnimalFoodStore(this));            
            Add(new GrazeFoodStore(this));
            Add(new ProductStore(this));
            Add(new HumanFoodStore(this));
        }
    }    
}
