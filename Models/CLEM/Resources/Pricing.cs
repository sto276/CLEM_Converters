﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    public class ResourcePricing : Node
    {
        public double PacketSize { get; set; }

        public bool UseWholePackets { get; set; } = true;

        public double PricePerPacket { get; set; }

        public ResourcePricing(Node parent) : base(parent)
        {
            Name = "Pricing";
        }
    }

    public class AnimalPricing : Node
    {
        public string PricingStyle { get; set; } = "perKg";

        public double SirePrice { get; set; } = 0.0;

        public AnimalPricing(Node parent) : base(parent)
        {
            Name = "AnimalPricing";
            Children.AddRange(Source.GetAnimalPrices(this));
        }
    }
    
}
