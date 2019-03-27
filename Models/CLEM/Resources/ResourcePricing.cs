using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    class ResourcePricing : Node
    {
        public double PacketSize { get; set; }

        public bool UseWholePackets { get; set; } = true;

        public double PricePerPacket { get; set; }

        public ResourcePricing(Node parent) : base(parent)
        {
            Name = "Pricing";
        }
    }
}
