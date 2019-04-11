namespace Models.Core
{
    using CLEM;

    public class Simulations : Node
    {
        public int ExplorerWidth { get; set; } = 300;

        public int Version { get; set; } = 54;

        public Simulations(Node parent) : base(parent)
        {

        }
    }    
    
    public class Simulation : Node
    {
        public Simulation(Node parent) : base(parent)
        {
            new Clock(this);
            new Summary(this);
            new ZoneCLEM(this);
        }
    }

    class Folder : Node
    {
        public bool ShowPageOfGraphs { get; set; } = true;

        public Folder(Node parent) : base(parent)
        {

        }
    }
}
