﻿using Models.CLEM.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;

namespace ReadIAT
{
    public partial class IAT
    {
        /// <summary>
        /// Contains methods for writing the various food store subsections of the
        /// resources folder in a CLEM simulation
        /// </summary>
        private static class StoreData
        {
            private static IAT iat;

            public static SubTable Fodder { get; private set; }

            public static SubTable FodderSpecs { get; private set; }

            /// <summary>
            /// Fodder pools used in the simulation.
            /// Keys are the pool ID.
            /// Values are the crop names in the pool
            /// </summary>
            public static Dictionary<int, string> Pools { get; private set; }

            public static void Construct(IAT source)
            {
                iat = source;

                Fodder = new SubTable("Bought fodder", source);
                FodderSpecs = new SubTable("Bought fodder specs", source);

                Pools = new Dictionary<int, string>();
                GetGrownFodderPools();
            }

            /// <summary>
            /// Checks if there is fodder that can be bought and ensures there
            /// is a pool to store it in
            /// </summary>
            /// <param name="iat"></param>
            public static void GetBoughtFodderPools()
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
            public static void GetGrownFodderPools()
            {
                WorksheetPart crop = (WorksheetPart)iat.Book.GetPartById(iat.SearchSheets("crop_inputs").Id);               

                // Attempt to find the fodder pool for each crop
                foreach (int id in CropData.Columns)
                {                   
                    // Check if the crop has residue
                    if (CropData.Grown.GetData<double>(4, id) <= 0) continue;

                    // Find crop data
                    var rows = crop.Worksheet.Descendants<Row>().Skip(1);                    
                    var inputs =
                        from row in rows
                        where TestRow(id, row, iat)
                        select row;

                    // Find the cropname
                    string cropname = CropData.Specs.RowNames.ElementAt(id + 1);

                    // Check data was found
                    if (!inputs.Any()) throw new Exception($"{cropname}: Crop not found in inputs sheet");

                    // Select the first row of the valid inputs
                    var input = inputs.First().Descendants<Cell>();

                    // Find what pool the residue uses
                    int.TryParse(iat.ParseCell(input.ElementAt(10)), out int pool);

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
            private static bool TestRow(int id, Row row, IAT iat)
            {
                Cell cell = row.Descendants<Cell>().ElementAt(2);
                string content = iat.ParseCell(cell);

                if (content == id.ToString()) return true;
                else return false;
            }            
        }

        public IEnumerable<AnimalFoodStoreType> GetAnimalStoreTypes(AnimalFoodStore store)
        {
            List<AnimalFoodStoreType> types = new List<AnimalFoodStoreType>();

            // Add each fodder pool to the animal food store
            foreach (int pool in StoreData.Pools.Keys)
            {
                types.Add(new AnimalFoodStoreType(store) { Name = StoreData.Pools[pool] });
            }

            return types;
        }

        public IEnumerable<HumanFoodStoreType> GetHumanStoreTypes(HumanFoodStore store)
        {
            List<HumanFoodStoreType> types = new List<HumanFoodStoreType>();

            foreach (int id in CropData.Columns)
            {
                // Check if the crop is stored at home
                double home_storage = CropData.Grown.GetData<double>(id + 1, 1);
                if (home_storage <= 0) continue;

                // Add the grain type to the store
                types.Add(new HumanFoodStoreType(store) { Name = CropData.Grown.RowNames[id + 1] });
            }

            foreach (int id in RuminantData.Columns)
            {
                // Check if milk is stored at home
                double home_milk = RuminantData.Specs.GetData<double>(18, id);
                if (home_milk <= 0) continue;

                // Add the milk type to the store
                types.Add(new HumanFoodStoreType(store) { Name = RuminantData.Specs.ColumnNames[id] + "_Milk" });
            }

            return types;
        }

        public IEnumerable<ProductStoreType> GetProductStoreTypes(ProductStore store)
        {
            return null;
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
