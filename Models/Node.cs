using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

        public Node SearchTree<Node>(Models.Node node) where Node : Models.Node
        {            
            foreach(Models.Node child in node.Children)
            {
                if (child.GetType() == typeof(Node)) return (Node)child;
                else
                {
                    var search = SearchTree<Node>(child);
                    if (search != null) return search;
                }
            }
            return null;
        }

        public Node GetAncestor<Node>() where Node : Models.Node
        {
            Models.Node ancestor = Parent;

            while (ancestor.Parent.GetType() != typeof(Node))
            {
                ancestor = ancestor.Parent;
            }

            return (Node)ancestor.Parent;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

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
