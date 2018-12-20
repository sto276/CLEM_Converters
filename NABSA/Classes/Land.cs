using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NABSA
{
    class Land
    {
        /// <summary>
        /// Builds the 'Land' subsection 
        /// </summary>
        /// <param name="nab">Source NABSA</param>
        public static XElement GetLand(XElement nabsa)
        {
            XElement land = new XElement("Land", new XElement("Name", "Land"));

            XElement specs = nabsa.Element("LandSpecs");
            var items = specs.Elements().ToArray();

            for (int i = 2; i < items.Length; i++)
            {
                var data = items[i].Elements().ToArray();
                if (data[0].Value != "0")
                {
                    XElement type = new XElement
                    ("LandType",
                        new XElement("Name", items[i].Name),
                        new XElement("IncludeInDocumentation", "true"),
                        new XElement("LandArea", data[0].Value),
                        new XElement("SoilType", data[3].Value)
                    );

                    land.Add(type);
                }
            }

            land.Add(new XElement("IncludeInDocumentation", "true"));

            return land;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        public static XElement GetPastureManage(XElement nabsa)
        {
            XElement pasture = new XElement
            (
                "PastureActivityManage",
                new XElement("Name", "PastureActivityManage"),
                GetRelationships(nabsa),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("LandTypeNameToUse", ""),
                new XElement("FeedTypeName", "GrazeFoodStore.NativePasture"),
                new XElement("StartingAmount", "0"),
                new XElement("StartingStockingRate", "0"),
                new XElement("AreaRequested", "0"),
                new XElement("UseAreaAvailable", "true")
            );
                       
            return pasture;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetRelationships(XElement nabsa)
        {
            XElement relationships = new XElement("Relationships");

            List<string> xdata = new List<string>() {"0", "20", "30", "100"};
            List<string> ydata = new List<string>() { "-0.95", "-0.15", "0", "1.05" };

            var changes = nabsa.Element("LandChangeCoeffs").Elements().Skip(1);
            foreach(var land in changes.First().Elements())
            {
                XElement relationship = new XElement
                (
                    "Relationship",
                    new XElement("IncludeInDocumentation", "true"),
                    new XElement("StartingValue", "3"),
                    new XElement("Minimum", "1"),
                    new XElement("Maximum", "8"),
                    GetDoubles("XValues", xdata),
                    GetDoubles("YValues", ydata)
                );
            }            

            return relationships.Elements();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static XElement GetDoubles(string name, IEnumerable<string> data)
        {
            XElement doubles = new XElement(name);

            foreach (string d in data)
            {
                doubles.Add(new XElement("double", d));
            }

            return doubles;
        }
    }
}
