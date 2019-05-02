using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Reader
{
    public partial class NABSA :IApsimX
    {
        public NABSA(string path)
        {
            Source = XElement.Load(path);

            // General Data
            SingleParams = Source.Element("SingleParams");

            // Land Data
            LandSpecs = Source.Element("LandSpecs");

            // Labour Data
            Priority = FindByNameTag(Source, "Labour Number and Priority");
            Supply = FindByNameTag(Source, "Labour Supply");

            // Ruminant Data
            SuppAllocs = FindFirst(Source, "SuppAllocs");
            SuppSpecs = FindFirst(Source, "SuppSpecs");
            RumSpecs = FindFirst(Source, "RumSpecs");
            Numbers = FindByNameTag(Source, "Startup ruminant numbers");
            Ages = FindByNameTag(Source, "Startup ruminant ages");
            Weights = FindByNameTag(Source, "Startup ruminant weights");
            Prices = FindByNameTag(Source, "Ruminant prices");

            Fodder = FindFirst(Source, "Fodder");
            FodderSpecs = FindFirst(Source, "FodderSpecs");

            // List of all possible breeds
            Breeds = SuppAllocs.Element("ColumnNames").Elements().Select(e => e.Value).ToList();

            // Index of each breed
            var indices = from breed in Breeds
                          select Breeds.IndexOf(breed);

            // Breeds that have a presence in the simulation
            PresentBreeds = from index in indices                                   
                            where (
                                // The total number of each breed
                                from cohort in Numbers.Elements().Skip(1)
                                select Convert.ToInt32(cohort.Elements().ToList()[index].Value)
                                ).Sum() > 0
                            // Breeds with a non-zero number of ruminants present
                            select Breeds.ElementAt(index);
        }
    }
}
