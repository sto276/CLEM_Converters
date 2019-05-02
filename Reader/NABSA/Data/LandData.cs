using Models.CLEM.Activities;
using Models.CLEM.Resources;
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
        /// <summary>
        /// Builds the 'Land' subsection 
        /// </summary>
        /// <param name="nab">Source NABSA</param>
        public IEnumerable<LandType> GetLandTypes(Land land)
        {
            List<LandType> types = new List<LandType>();

            var items = LandSpecs.Elements().ToArray();

            for (int i = 2; i < items.Length; i++)
            {
                var data = items[i].Elements().ToArray();
                if (data[0].Value != "0")
                {
                    LandType type = new LandType(land)
                    {
                        Name = items[i].Name.LocalName,
                        LandArea = Convert.ToDouble(data[0].Value),
                        SoilType = Convert.ToInt32(data[3].Value)
                    };

                    types.Add(type);
                }
            }
            return types;
        }

        public PastureActivityManage GetManagePasture(ActivitiesHolder folder)
        {
            PastureActivityManage pasture = new PastureActivityManage(folder){};

            pasture.Add(new Relationship(pasture)
            {
                Name = "LandConditionIndex",
                StartingValue = Convert.ToDouble(FindFirst(Source, "Land_con").Value),
                Minimum = 1,
                Maximum = 8 
            });

            pasture.Add(new Relationship(pasture)
            {
                Name = "GrassBasalArea",
                StartingValue = Convert.ToDouble(FindFirst(Source, "Grass_BA").Value),
                Minimum = 1,
                Maximum = Convert.ToDouble(FindFirst(Source, "GBAmax").Value),
                YValues = new[] { -0.95, -0.15, 0, 1.05 }
            });

            return pasture;
        } 

    }
}
