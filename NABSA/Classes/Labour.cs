namespace NABSA
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using static Queries;
    static class Labour
    {      
        /// <summary>
        /// Writes the 'Labour' subsection
        /// </summary>
        /// <param name="nab">Source NABSA</param>
        public static XElement GetLabour(XElement nabsa)
        {           
            XElement labour = new XElement(
                "Labour", 
                new XElement("Name", "Labour"),
                GetTypes(nabsa),
                GetAvailability(nabsa),
                new XElement("IncludeIhDocumentation", "true"),
                new XElement("AllowAging", "false")
            );
            return labour;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetTypes(XElement nabsa)
        {
            XElement types = new XElement("types");
            XElement priority = FindByName(nabsa, "Labour Number and Priority");          
            var number = priority.Elements().ToArray();

            for (int i = 2; i < number.Length; i++)
            {
                var ndata = number[i].Elements().ToArray();
                if (number[i].Elements().First().Value != "0")
                {
                    string name = number[i].Name.LocalName;

                    // Find the initial age of the labourer                    
                    string age = "40";
                    if (name.Contains("Elderly")) age = "65";
                    else if (name.Contains("Teenager")) age = "15";
                    else if (name.Contains("Child")) age = "8";

                    // Find the gender of the labourer
                    string gender = "Male";
                    if (name.Contains("F")) gender = "Female";

                    // Represent the data in XML
                    XElement type = new XElement(
                        "LabourType",
                        new XElement("Name", number[i].Name),
                        new XElement("IncludeInDocumentation", "true"),
                        new XElement("InitialAge", age),
                        new XElement("Gender", gender),
                        new XElement("Individuals", ndata[0].Value)
                    );
                    types.Add(type);
                }
            }
            return types.Elements();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static XElement GetAvailability(XElement nabsa)
        {
            XElement availability = new XElement
            ("LabourAvailabilityList",
                new XElement("Name", "LabourAvailabilityList"),
                GetItems(nabsa),
                new XElement("IncludeInDocumentation", "true")
            );

            // If all the items are zero, ignore the availability
            if (availability.Elements().Count() > 2) return availability;
            else return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetItems(XElement nabsa)
        {
            XElement supply = FindByName(nabsa, "Labour Supply");
            XElement items = new XElement("items");

            for (int i = 2; i < supply.Elements().Count(); i++)
            {
                XElement group = supply.Elements().ElementAt(i);

                // Skip this item if it has zero-value
                string value = group.Elements().First().Value;
                if (value == "0") continue;

                string name = group.Name.LocalName;
                items.Add(GetItem(name, value));
            }

            return items.Elements();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private static XElement GetItem(string name, string value)
        {
            XElement item = new XElement
            (
                "LabourAvailabilityItem",
                new XElement("Name", name),
                GetFilter(name),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("Value", value)
            );
            return item;
        }

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
            XElement group = new XElement("LabourFilterGroup");
            group.Add(new XElement("Name", "Male"));            
            group.Add(GetFilter("Male"));
            group.Add(new XElement("IncludeInDocumentation", "true"));
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

    }
}
