using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Storage
{
    public class DataStore : Node
    {
        public DataStore(Node parent) : base(parent)
        {
            Name = "DataStore";
        }
    }
}
