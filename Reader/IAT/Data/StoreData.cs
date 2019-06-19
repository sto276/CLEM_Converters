using Models.CLEM.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;

namespace Reader
{
    public partial class IAT
    {
        

        /// <summary>
        /// Fodder pools used in the simulation.
        /// Keys are the pool ID.
        /// Values are the crop names in the pool
        /// </summary>
        private static Dictionary<int, string> Pools { get; set; }

        /// <summary>
        /// Checks if there is fodder that can be bought and ensures there
        /// is a pool to store it in
        /// </summary>
        /// <param name="iat"></param>
        private void GetBoughtFodderPools()
        {
            // Look at each fodder type in the table
            for (int row = 0; row < Fodder.RowNames.Count; row++)
            {
                // Check if a fodder type can be bought
                double unit = Fodder.GetData<double>(row, 0);
                int month = Fodder.GetData<int>(row, 1);
                if ((unit > 0) && (month > 0))
                {
                    // Create appropriate storage pool if none exists
                    int pool = Fodder.GetData<int>(row, 4);
                    string cropname = FodderSpecs.RowNames[row + 1];
                    if (!Pools.ContainsKey(pool)) Pools.Add(pool, cropname);
                    else Pools[pool] = Pools[pool] + $", {cropname}";
                }
            }
        }

        /// <summary>
        /// Builds the list of animal fodder pools used in the simulation,
        /// and stores the ID of the crops grown
        /// </summary>
        /// <param name="iat">Source IAT</param>
        private void GetGrownFodderPools()
        {
            WorksheetPart crop = (WorksheetPart)Book.GetPartById(SearchSheets("crop_inputs").Id);
            var rows = crop.Worksheet.Descendants<Row>().Skip(1);

            // Attempt to find the fodder pool for each crop
            foreach (int id in GrainIDs)
            {                   
                // Check if the crop has residue
                if (CropsGrown.GetData<double>(4, id) <= 0) continue;

                // Find the cropname
                string cropname = CropSpecs.RowNames.ElementAt(id + 1);

                // Check data was found
                int pool = 0;
                if (rows.Any(r => TestRow(id, r)))
                {
                    // Select the first row of the valid inputs
                    Cell input = rows.First(r => TestRow(id, r)).Descendants<Cell>().ElementAt(10);

                    // Find what pool the residue uses
                    int.TryParse(ParseCell(input), out pool);
                }
                else
                {
                    Shared.WriteError(new ErrorData()
                    {
                        FileName = Name,
                        FileType = "IAT",
                        Message = $"Crop type {cropname} wasn't found in the inputs sheet",
                        Severity = "Low",
                        Table = "-",
                        Sheet = "crop_inputs"
                    });
                }

                // If the pool does not exist, create a new pool with the residue in it
                if (!Pools.ContainsKey(pool)) Pools.Add(pool, $"{cropname}_Residue");
                // If the pool exists already, add the residue to it.
                else Pools[pool] = Pools[pool] + $", {cropname}_Residue";                   
            }
        }

        /// <summary>
        /// Tests a row of input data to see if it contains the desired ID
        /// </summary>
        /// <param name="id">ID to search for</param>
        /// <param name="row">Row to search in</param>
        /// <param name="iat">Source IAT</param>
        /// <returns></returns>
        private bool TestRow(int id, Row row)
        {
            Cell cell = row.Descendants<Cell>().ElementAt(2);
            string content = ParseCell(cell);

            if (content == id.ToString()) return true;
            else return false;
        }           

        public IEnumerable<AnimalFoodStoreType> GetAnimalStoreTypes(AnimalFoodStore store)
        {
            List<AnimalFoodStoreType> types = new List<AnimalFoodStoreType>();

            // Add each fodder pool to the animal food store
            foreach (int pool in Pools.Keys)
            {
                types.Add(new AnimalFoodStoreType(store) { Name = Pools[pool] });
            }

            return types;
        }

        public IEnumerable<HumanFoodStoreType> GetHumanStoreTypes(HumanFoodStore store)
        {
            List<HumanFoodStoreType> types = new List<HumanFoodStoreType>();

            foreach (int id in GrainIDs)
            {
                // Check if the crop is stored at home
                double home_storage = CropSpecs.GetData<double>(id + 1, 1);
                if (home_storage <= 0) continue;

                // Add the grain type to the store
                types.Add(new HumanFoodStoreType(store) { Name = CropSpecs.RowNames[id + 1] });
            }

            foreach (int id in RumIDs)
            {
                // Check if milk is stored at home
                double home_milk = RumSpecs.GetData<double>(18, id);
                if (home_milk <= 0) continue;

                // Add the milk type to the store
                types.Add(new HumanFoodStoreType(store) { Name = RumSpecs.ColumnNames[id] + "_Milk" });
            }

            return types;
        }

        public IEnumerable<ProductStoreType> GetProductStoreTypes(ProductStore store)
        {
            return new List<ProductStoreType>();
        }

        public GrazeFoodStoreType GetGrazeFoodStore(GrazeFoodStore store)
        {
            return new GrazeFoodStoreType(store);
        }

        public CommonLandFoodStoreType GetCommonFoodStore(AnimalFoodStore store)
        {
            // Checks if there is any yield from the common land before adding it to the foodstore            
            double yield = Convert.ToDouble(GetCellValue(Part, 81, 4));
            if (yield > 0) return new CommonLandFoodStoreType(store);
            else return null;
        }
    }
}
