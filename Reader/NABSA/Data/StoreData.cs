using Models.CLEM.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Reader
{
    public partial class NABSA
    {
        public GrazeFoodStoreType GetGrazeFoodStore(GrazeFoodStore store)
        {                        
            return new GrazeFoodStoreType(store)
            {
                NToDMDCoefficient = GetValue<double>(SingleParams, "Coeff_DMD"),
                NToDMDIntercept = GetValue<double>(SingleParams, "Incpt_DMD"),
                GreenNitrogen = GetValue<double>(SingleParams, "Native_N"),
                DecayNitrogen = GetValue<double>(SingleParams, "Decay_N"),
                MinimumNitrogen = GetValue<double>(SingleParams, "Native_N_min"),
                DecayDMD = GetValue<double>(SingleParams, "Decay_DMD"),
                MinimumDMD = GetValue<double>(SingleParams, "Native_DMD_min"),
                DetachRate = GetValue<double>(SingleParams, "Native_detach"),
                CarryoverDetachRate = GetValue<double>(SingleParams, "Carryover_detach"),
                IntakeTropicalQualityCoefficient = GetValue<double>(SingleParams, "Intake_trop_quality"),
                IntakeQualityCoefficient = GetValue<double>(SingleParams, "Intake_coeff_quality")
            };
        }

        public IEnumerable<AnimalFoodStoreType> GetAnimalStoreTypes(AnimalFoodStore store)
        {
            List<AnimalFoodStoreType> types = new List<AnimalFoodStoreType>();
            AddSupplements(types, store);
            AddBought(types, store);
            return types;
        }       

        private void AddSupplements(List<AnimalFoodStoreType> types, AnimalFoodStore store)
        {
            // List of all supplement allocations (skipping metadata)
            var allocs = SuppAllocs.Elements().Skip(2).ToList();

            // Indices of non-zero allocations
            var indices = from alloc in allocs
                         where alloc.Elements().Select(e => e.Value).ToList().Exists(v => v != "0")
                         select allocs.IndexOf(alloc);

            // List of all supplement specifications (skipping metadata)
            var supps = SuppSpecs.Elements().Skip(3).ToList();

            // Collection of specifications with allocations
            var specs = from spec in supps
                        where indices.Contains(supps.IndexOf(spec))
                        select new string[3] 
                        {
                            spec.Name.LocalName,
                            spec.Elements().ElementAt(1).Value,
                            spec.Elements().ElementAt(2).Value
                        };

            foreach (var spec in specs)
            {
                types.Add(new AnimalFoodStoreType(store)
                {
                    Name = spec[0],
                    DMD = Convert.ToDouble(spec[1]),
                    Nitrogen = Convert.ToDouble(spec[2])
                });
            }
        }

        private void AddBought(List<AnimalFoodStoreType> types, AnimalFoodStore store)
        {
            var fodders = Fodder.Elements().Skip(2).ToList();            

            var indices = from fodder in fodders
                          where fodder.Elements().ElementAt(3).Value == "TRUE"
                          select fodders.IndexOf(fodder);

            var slist = FodderSpecs.Elements().Skip(3).ToList();
            var specs = from spec in slist
                        where indices.Contains(slist.IndexOf(spec))
                        select new string[3]
                        {
                            spec.Name.LocalName,
                            spec.Elements().ElementAt(1).Value,
                            spec.Elements().ElementAt(2).Value
                        };

            foreach (var spec in specs)
            {
                types.Add(new AnimalFoodStoreType(store)
                {
                    Name = spec[0],
                    DMD = Convert.ToDouble(spec[1]),
                    Nitrogen = Convert.ToDouble(spec[2])
                });
            }
        }

        public IEnumerable<HumanFoodStoreType> GetHumanStoreTypes(HumanFoodStore store)
        {
            return null;
        }

        public IEnumerable<ProductStoreType> GetProductStoreTypes(ProductStore store)
        {
            return null;
        }

        public CommonLandFoodStoreType GetCommonFoodStore(AnimalFoodStore store)
        {
            return null;
        }
    }
}
