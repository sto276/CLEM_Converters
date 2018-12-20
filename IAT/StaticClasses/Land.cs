using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Linq;

namespace IAT
{
    /// <summary>
    /// Additional 'Resources' sections for a CLEM simulation
    /// </summary>
    class Land
    {
        /// <summary>
        /// Creates the 'Land' XML structure for a CLEM simulation
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetLand(IAT iat)
        {
            IATable specs = iat.tables["Land specifications"];
            XElement land = new XElement("Land");
            land.Add(new XElement("Name", "Land"));

            // Iterate over the table and write all non-zero data
            // Output format is subject to change as model changes
            int row = 0;
            foreach (string item in specs.GetRowNames())
            {
                if (specs.GetData<string>(row, 0) != "0")
                {
                    XElement type = new XElement
                    (
                        "LandType",
                        new XElement("Name", specs.GetRowNames()[row]),
                        new XElement("IncludeInDocumentation", "true"),
                        new XElement("LandArea", specs.GetData<string>(row, 0)),
                        new XElement("PortionBuildings", specs.GetData<string>(row, 1)),
                        new XElement("SoilType", specs.GetData<string>(row, 3))
                    );

                    land.Add(type);
                }
                row += 1;
            }
            land.Add(new XElement("IncludeInDocumentation", "true"));

            return land;
        }                
    }
}
