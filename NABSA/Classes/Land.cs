using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NABSA
{
    using static Queries;
    static class Land
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
                    (
                        "LandType",
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
                GetConditionIndex(nabsa),
                GetBasalArea(nabsa),
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
        /// <returns></returns>
        private static XElement GetConditionIndex(XElement nabsa)
        {
            string start = FindFirst(nabsa, "Land_con").Value;

            string[] xdata = new[] { "0", "20", "30", "100" };
            string[] ydata = new[] { "-0.625", "-0.125", "0", "0.75" };

            XElement index = new XElement
            (
                "Relationship",
                new XElement("LandConditionIndex"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("StartingValue", start),
                new XElement("Minimum", "1"),
                new XElement("Maximum", "8"),
                GetDoubles("XValues", xdata),
                GetDoubles("YValues", ydata)
            );

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static XElement GetBasalArea(XElement nabsa)
        {
            string start = FindFirst(nabsa, "Grass_BA").Value;
            string max = FindFirst(nabsa, "GBAmax").Value;

            string[] xdata = new[] { "0", "20", "30", "100" };
            string[] ydata = new[] { "-0.95", "-0.15", "0", "1.05" };

            XElement basal = new XElement
            (
                "Relationship",
                new XElement("GrassBasalArea"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("StartingValue", start),
                new XElement("Minimum", max),
                new XElement("Maximum", "8"),
                GetDoubles("XValues", xdata),
                GetDoubles("YValues", ydata)
            );

            return basal;
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
