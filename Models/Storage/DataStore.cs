namespace Models.Storage
{
    /// <summary>
    /// Reference to the SQLite database (.DB) storing output data 
    /// </summary>
    public class DataStore : Node
    {
        public DataStore(Node parent) : base(parent)
        {
            Name = "DataStore";
        }
    }
}
