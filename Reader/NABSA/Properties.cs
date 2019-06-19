using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Reader
{
    public partial class NABSA
    {
        public string Name { get; set; }

        private XElement Source { get; set; }

        private XElement SingleParams { get; set; }

        private XElement LandSpecs { get; set; }

        private XElement Priority { get; set; }

        private XElement Supply { get; set; }        

        private XElement SuppAllocs { get; set; }

        private XElement SuppSpecs { get; set; }

        private XElement RumSpecs { get; set; }

        private XElement Numbers { get; set; }

        private XElement Ages { get; set; }

        private XElement Weights { get; set; }

        private XElement Prices { get; set; }

        private XElement Fodder { get; set; }

        private XElement FodderSpecs { get; set; }

        private List<string> Breeds { get; set; }

        private IEnumerable<string>  PresentBreeds { get; set; }

        private bool disposed = false;
    }
}
