using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Node
    {
        public string Name { get; set; }        

        public Node Parent { get; set; }

        public List<Node> Children { get; set; }

        public bool IncludeInDocumentation { get; set; } = true;

        public bool Enabled { get; set; } = true;

        public bool ReadOnly { get; set; } = false;

        public IApsimX Source { get; set; }

        private readonly bool root;

        public Node(Node parent)
        {
            if (parent == null) root = true;
            else
            {
                root = false;
                Source = parent.Source;
            }
            Parent = parent;
        }        
    }
}
