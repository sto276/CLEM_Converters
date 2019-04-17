using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IATReader
{
    class LabourData
    {
        /// <summary>
        /// 
        /// </summary>
        static readonly Dictionary<string, string> ages = new Dictionary<string, string>()
        {
            {"Elderly Male", "72"},
            {"Elderly Female", "68"},
            {"Adult Male", "42"},
            {"Adult Female", "31"},
            {"Teenager Male", "13"},
            {"Teenager Female", "12"},
            {"Child Male", "7"},
            {"Child Female", "7"},
        };

        /// <summary>
        /// Creates the 'Labour' XML structure for a CLEM simulation
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetLabour(IAT iat)
        {
            IATable supply = iat.tables["Labour supply/hire"];

            XElement labour = new XElement
            (
                "Labour",
                new XElement("Name", "Labour"),            
                GetTypes(supply),
                GetAvailability(supply),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("AllowAging", "true")
            );

            if (labour.Elements().Count() > 3) return labour;
            else return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supply"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetTypes(IATable supply)
        {
            XElement types = new XElement("Types");

            int row = -1;
            foreach (string item in supply.GetRowNames())
            {
                row++;
                if (supply.GetData<string>(row, 0) != "0")
                {
                    // Finds the current demographic
                    string demo = supply.GetExtra()[row] + " " + supply.GetRowNames()[row];

                    // Tries to find an age for the demographic, defaults to 20
                    string age = "20";
                    ages.TryGetValue(demo, out age);

                    XElement type = new XElement("LabourType",
                        new XElement("Name", demo),
                        new XElement("IncludeInDocumentation", "true"),
                        new XElement("InitialAge", age),
                        new XElement("Gender", supply.GetRowNames()[row]),
                        new XElement("Individuals", supply.GetData<string>(row, 0))
                    );

                    types.Add(type);
                }
            }

            return types.Elements();
        }

        /// <summary>
        /// Creates the 'Labour Availability' XML structure for a CLEM simulation
        /// </summary>
        /// <param name="supply">IAT table containing availability data</param>
        /// <returns>Labour Availability XML structure</returns>
        private static XElement GetAvailability(IATable supply)
        {
            XElement availability = new XElement
            (
                "LabourAvailabilityList",
                new XElement("Name", "LabourAvailabilityList"),
                GetFilters(supply),            
                new XElement("IncludeInDocumentation", "true")
            );

            if (availability.Elements().Count() > 2) return availability;
            else return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static XElement GetLabourRequirement()
        {
            XElement requirement = new XElement
            (
                "LabourRequirement",
                new XElement("Name", "LabourRequirement"),
                GetLabourGroup(),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("LabourPerUnit", "0.75"),
                new XElement("UnitSize", "25"),
                new XElement("WholeUnitBlocks", "false"),
                new XElement("UnitType", "perKg"),
                new XElement("MinimumPerPerson", "1"),
                new XElement("MaximumPerPerson", "100"),
                new XElement("LabourShortfallAffectsActivity", "false")
            );
            return requirement;
        }

        /// <summary>
        /// Writes a 'Labour group' section for an Activity
        /// </summary>
        public static XElement GetLabourGroup()
        {
            XElement group = new XElement
            (
                "LabourFilterGroup",
                new XElement("Name", "Male"),
                GetFilter("Male"),
                new XElement("IncludeInDocumentation", "true")
            );
            return group;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private static XElement GetFilter(string name)
        {
            XElement filter = new XElement
            (
                "LabourFilter",
                new XElement("Name", "LabourFilter"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("Parameter", "Name"),
                new XElement("Operator", "Equal"),
                new XElement("Value", name)
            );
            return filter;
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supply"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetFilters(IATable supply)
        {
            XElement filters = new XElement("Filters");
            int row = -1;
            foreach (string cat in supply.GetRowNames())
            {
                row++;
                if (supply.GetData<string>(row, 0) != "0")
                {
                    int value = (int)Math.Round(supply.GetData<double>(row, 2));
                    string name = $"{supply.GetExtra()[row]} {cat} ";

                    XElement item = new XElement
                    (
                        "LabourAvailabilityItem",
                        new XElement("Name", name),
                        GetFilter(name),
                        new XElement("IncludeInDocumentation", "true"),
                        new XElement("Value", value)
                    );

                    filters.Add(item);
                }
            }
            return filters.Elements();
        }

    }
}
