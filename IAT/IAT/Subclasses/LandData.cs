using Models.CLEM.Resources;
using System.Collections.Generic;
using System.Linq;

namespace ReadIAT
{
    public partial class IAT
    {
        /// <summary>
        /// Contains the Land data from an IAT source file
        /// </summary>
        private static class LandData
        {
            /// <summary>
            /// IAT sub table containing land specification data
            /// </summary>
            public static SubTable Specs { get; private set; }

            private static double TotalArea { get; set; }

            /// <summary>
            /// Pseudo constructor
            /// </summary>
            public static void Construct(IAT source)
            {
                Specs = new SubTable("Land specifications", source);

                TotalArea = Specs.GetColData<double>(0).Sum();
            }

            /// <summary>
            /// Searches the land specification table for valid land type data,
            /// and returns the collection
            /// </summary>
            /// <param name="parent"></param>
            public static IEnumerable<LandType> GetLandTypes(Land parent)
            {
                List<LandType> types = new List<LandType>();

                // Iterate over the rows in the table
                int row = -1;
                foreach (string item in Specs.RowNames)
                {
                    row++;

                    // Skip empty land types
                    if (Specs.GetData<string>(row, 0) == "0") continue;

                    double area = Specs.GetData<double>(row, 0);

                    // Create a new type model based on the current row
                    types.Add(new LandType(parent)
                    {
                        Name = Specs.RowNames[row],
                        LandArea = area,
                        PortionBuildings = Specs.GetData<double>(row, 1),
                        ProportionOfTotalArea = area / TotalArea,
                        SoilType = Specs.GetData<int>(row, 3)
                    });                    
                }
                return types;
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public IEnumerable<LandType> GetLandTypes(Land parent)
        {
            return LandData.GetLandTypes(parent);
        }
    }
}