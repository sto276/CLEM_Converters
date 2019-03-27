using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Core
{
    class Simulations : Node
    {
        public int ExplorerWidth { get; set; } = 300;

        public int Version { get; set; } = 54;

        public Simulations(Node parent) : base(parent)
        {

        }
    }
}
