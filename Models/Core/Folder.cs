using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Core
{
    class Folder : Node
    {
        public bool ShowPageOfGraphs { get; set; } = true;

        public Folder(Node parent) : base(parent)
        {

        }
    }
}
