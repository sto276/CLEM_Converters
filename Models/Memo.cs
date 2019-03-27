using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    class Memo : Node
    {
        public string Text { get; set; }

        public Memo(Node parent) : base(parent)
        {

        }
    }
}
