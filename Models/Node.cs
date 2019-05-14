using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models
{    
    /// <summary>
    /// Base node in the tree, this should not be instantiated directly
    /// </summary>
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
        
        /// <summary>
        /// Add a child node
        /// </summary>
        /// <param name="node"></param>
        public void Add(Node node)
        {
            if (node is null) return;
            Children.Add(node);
        }

        /// <summary>
        /// Add a collection of child nodes
        /// </summary>
        public void Add(IEnumerable<Node> nodes)
        {
            if (nodes is null) return;
            foreach (Node node in nodes) Add(node);
        }
    }
}
