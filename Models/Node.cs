using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{    
    /// <summary>
    /// Base node in the tree, this should not be instantiated directly
    /// </summary>
    public class Node : IDisposable
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

        [JsonIgnore]
        private bool disposed = false;

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

        /// <summary>
        /// Use a Depth-first search to find an instance of 
        /// the given node type. Returns null if none are found.
        /// </summary>
        /// <typeparam name="Node">The type of node to search for</typeparam>
        public Node SearchTree<Node>(Models.Node node) where Node : Models.Node
        {
            var result = node.Children
                .Select(n => (n.GetType() == typeof(Node)) ? n : SearchTree<Node>(n));

            return result.OfType<Node>().FirstOrDefault();
        }

        /// <summary>
        /// Iterates over the nodes ancestors until it finds
        /// the first instance of the given node type.
        /// </summary>
        /// <typeparam name="Node">The type of node to search for</typeparam>
        public Node GetAncestor<Node>() where Node : Models.Node
        {
            Models.Node ancestor = Parent;

            while (ancestor.Parent.GetType() != typeof(Node))
            {
                ancestor = ancestor.Parent;
            }

            return (Node)ancestor.Parent;
        }

        /// <summary>
        /// Implements IDisposable
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implements IDisposable
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                Source?.Dispose();
            }

            disposed = true;
        }
    }
}
