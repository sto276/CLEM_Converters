using Models.CLEM.Activities;
using Models.CLEM.Resources;
using System.Collections.Generic;
using System.Linq;

namespace Reader
{
    public partial class IAT
    {           
        private double TotalArea { get; set; }

        /// <summary>
        /// Searches the land specification table for valid land type data,
        /// and returns the collection
        /// </summary>
        /// <param name="parent"></param>
        public IEnumerable<LandType> GetLandTypes(Land parent)
        {
            List<LandType> types = new List<LandType>();

            // Iterate over the rows in the table
            int row = -1;
            foreach (string item in LandSpecs.RowNames)
            {
                row++;

                // Skip empty land types
                if (LandSpecs.GetData<string>(row, 0) == "0") continue;

                double area = LandSpecs.GetData<double>(row, 0);

                // Create a new type model based on the current row
                types.Add(new LandType(parent)
                {
                    Name = LandSpecs.RowNames[row],
                    LandArea = area,
                    PortionBuildings = LandSpecs.GetData<double>(row, 1),
                    ProportionOfTotalArea = area / TotalArea,
                    SoilType = LandSpecs.GetData<int>(row, 3)
                });                    
            }
            return types;
        }                   

        public PastureActivityManage GetManagePasture(ActivitiesHolder folder)
        {
            return null;
        }
    }
}