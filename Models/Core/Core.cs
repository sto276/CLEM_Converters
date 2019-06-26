using Models.CLEM;
using Models.Storage;

namespace Models.Core
{
    /// <summary>
    /// Container for a series of simulations
    /// </summary>
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
    
    /// <summary>
    /// Models a single simulation
    /// </summary>
    public class Simulation : Node
    {
        public Simulation(Node parent) : base(parent)
        {
            Add(Source.GetClock(this));            
            Add(new Summary(this));
            Add(new ZoneCLEM(this));
        }
    }

    /// <summary>
    /// Generic container for models
    /// </summary>
    public class Folder : Node
    {
        public bool ShowPageOfGraphs { get; set; } = true;

        public Folder(Node parent) : base(parent)
        { }
    }
}
