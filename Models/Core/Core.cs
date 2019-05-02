namespace Models.Core
{
    using CLEM;
    using Storage;

    public class Simulations : Node
    {
        public int ExplorerWidth { get; set; } = 300;

        public int Version { get; set; } = 54;

        public Simulations(Node parent) : base(parent)
        {
            Name = "Simulations";
            Add(new DataStore(this));
        }
    }    
    
    public class Simulation : Node
    {
        public Simulation(Node parent) : base(parent)
        {
            Add(Source.GetClock(this));
            Add(Source.GetFiles(this));
            Add(new Summary(this));
            Add(new ZoneCLEM(this));
        }
    }

    public class Folder : Node
    {
        public bool ShowPageOfGraphs { get; set; } = true;

        public Folder(Node parent) : base(parent)
        { }
    }
}
