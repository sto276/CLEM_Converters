using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models
{    
    public class Node
    {
        public string Name { get; set; }      

        public List<Node> Children { get; set; } = new List<Node>();

        public bool IncludeInDocumentation { get; set; } = true;

        public bool Enabled { get; set; } = true;

        public bool ReadOnly { get; set; } = false;

        [JsonIgnore]
        public Node Parent { get; set; }

        [JsonIgnore]
        public IApsimX Source { get; set; }

        public Node(Node parent)
        {
            Parent = parent;
            if (parent != null) Source = parent?.Source;
        }
        
        public void Add(Node node)
        {
            Children.Add(node);
        }

        public void Add(IEnumerable<Node> nodes)
        {
            Children.AddRange(nodes);
        }
    }
}
