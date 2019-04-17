using System;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;

namespace IATReader
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

            public static void Construct(IAT source)
            {
                iat = source;
            }

            /// <summary>
            /// Build the 'Human Food Store' XML
            /// </summary>
            /// <param name="iat">Source IAT</param>
            public static XElement GetHumanFoodStore(IAT iat)
            {
                XElement store = new XElement("HumanFoodStore");
                store.Add(new XElement("Name", "HumanFoodStore"));
                store.Add(new XElement("IncludeInDocumentation", "true"));

                // Add grain product to store
                foreach (int id in iat.grains) store.Add(GetGrain(iat.tables["Grain Crop Specifications"], id));

                // Add milk product to store
                foreach (int id in iat.ruminants) store.Add(GetMilk(iat.tables["Ruminant specifications"], id));

                // If no product is stored, this store is not added
                if (store.Elements().Count() > 2) return store;
                else return null;
            }

            /// <summary>
            /// If a grain is being harvested, add a place to store it
            /// </summary>
            /// <param name="gcs">Grain Crops Specifications table</param>
            /// <param name="id">ID of the grain in the table</param>
            /// <returns></returns>
            private static XElement GetGrain(IATable gcs, int id)
            {
                // Check that some of the grain is retained
                double home_storage = gcs.GetData<double>(id + 1, 1);

                // Create a place to store it
                if (home_storage > 0)
                {
                    string name = gcs.GetRowNames()[id + 1];

                    XElement product = new XElement("HumanFoodStoreType");
                    product.Add(new XElement("Name", name));
                    product.Add(new XElement("IncludeInDocumentation", "true"));
                    product.Add(new XElement("DryMatter", "0"));
                    product.Add(new XElement("DMD", "0"));
                    product.Add(new XElement("Nitrogen", "0"));
                    product.Add(new XElement("StartingAge", "0"));
                    product.Add(new XElement("StartingAmount", "0"));

                    return product;
                }
                else return null;
            }

            /// <summary>
            /// If a ruminant is producing milk, adds a place to store it
            /// </summary>
            /// <param name="rs">Ruminant specifications table</param>
            /// <param name="id">ID of the ruminant in the table</param>
            /// <returns></returns>
            private static XElement GetMilk(IATable rs, int id)
            {
                // Check that some of the milk is retained
                double home_milk = rs.GetData<double>(18, id);

                // Create a place to store it
                if (home_milk > 0)
                {
                    string name = rs.GetColNames()[id];
                    XElement milk = new XElement("HumanFoodStoreType");
                    milk.Add(new XElement("Name", $"Milk_{name}"));
                    milk.Add(new XElement("IncludeInDocumentation", "true"));
                    milk.Add(new XElement("StartingAmount", "0"));

                    return milk;
                }
                else return null;
            }

            /// <summary>
            /// Build the 'Product Store' XML
            /// </summary>
            /// <param name="iat">Source IAT</param>
            public static XElement GetProductStore(IAT iat)
            {
                IATable gcs = iat.tables["Grain Crop Specifications"];

                XElement store = new XElement("ProductStore");
                store.Add(new XElement("Name", "ProductStore"));
                store.Add(new XElement("IncludeInDocumentation", "true"));

                return store;
            }

            /// <summary>
            /// Build the 'Animal Food Store' XML
            /// </summary>
            /// <param name="iat">Source IAT</param>
            public static XElement GetAnimalFoodStore(IAT iat)
            {
                XElement store = new XElement("AnimalFoodStore");
                store.Add(new XElement("Name", "AnimalFoodStore"));

                // Add each fodder pool to the animal food store
                foreach (int pool in iat.pools.Keys)
                {
                    XElement type = new XElement("AnimalFoodStoreType");
                    type.Add(new XElement("Name", $"{iat.pools[pool]}"));
                    type.Add(new XElement("IncludeInDocumentation", "true"));
                    type.Add(new XElement("DMD", "0"));
                    type.Add(new XElement("Nitrogen", "0"));
                    type.Add(new XElement("StartingAmount", "0"));
                    store.Add(type);
                }

                // Checks if there is any yield from the common land before adding it to the foodstore            
                double yield = Convert.ToDouble(iat.GetCellValue(iat.part, 81, 4));
                if (yield > 0) store.Add(GetCommonLand());

                store.Add(new XElement("IncludeInDocumentation", "true"));
                return store;
            }

            /// <summary>
            /// Build the 'Native Pasture' XML 
            /// </summary>
            private static XElement GetNativePasture()
            {
                XElement nativepasture = new XElement("AnimalFoodStoreType");
                nativepasture.Add(new XElement("Name", "NativePasture"));
                nativepasture.Add(new XElement("IncludeInDocumentation", "true"));
                nativepasture.Add(new XElement("DMD", "0"));
                nativepasture.Add(new XElement("Nitrogen", "0"));
                nativepasture.Add(new XElement("StartingAmount", "0"));
                return nativepasture;
            }

            /// <summary>
            /// Build the 'Common Land' XML
            /// </summary>
            private static XElement GetCommonLand()
            {
                XElement commonland = new XElement("CommonLandFoodStoreType");
                commonland.Add(new XElement("Name", "CommonLand"));
                commonland.Add(new XElement("IncludeInDocumentation", "true"));
                commonland.Add(new XElement("NToDMDCoefficient", "0"));
                commonland.Add(new XElement("NToDMDIntercept", "0"));
                commonland.Add(new XElement("NToDMDCrudeProteinDenominator", "0"));
                commonland.Add(new XElement("Nitrogen", "0"));
                commonland.Add(new XElement("MinimumNitrogen", "0"));
                commonland.Add(new XElement("MinimumDMD", "0"));
                commonland.Add(new XElement("PastureLink", "GrazeFoodStore.NativePasture"));
                commonland.Add(new XElement("NitrogenReductionFromPasture", "0"));
                return commonland;
            }

            /// <summary>
            /// Build the 'Graze Food Store' XML
            /// </summary>
            public static XElement GetGrazeFoodStore()
            {
                XElement store = new XElement("GrazeFoodStore");
                store.Add(new XElement("Name", "GrazeFoodStore"));

                XElement type = new XElement("GrazeFoodStoreType");
                type.Add(new XElement("Name", "NativePasture"));
                type.Add(new XElement("IncludeInDocumentation", "true"));
                type.Add(new XElement("NToDMDCoefficient", "0"));
                type.Add(new XElement("NToDMDIntercept", "0"));
                type.Add(new XElement("NToDMDCrudeProteinDenominator", "0"));
                type.Add(new XElement("GreenNitrogen", "0"));
                type.Add(new XElement("DecayNitrogen", "0"));
                type.Add(new XElement("MinimumNitrogen", "0"));
                type.Add(new XElement("DecayDMD", "0"));
                type.Add(new XElement("MinimumDMD", "0"));
                type.Add(new XElement("DetachRate", "0"));
                type.Add(new XElement("CarryoverDetachRate", "0"));
                type.Add(new XElement("IntakeTropicalQualityCoefficient", "0"));
                type.Add(new XElement("IntakeQualityCoefficient", "0"));
                store.Add(type);

                store.Add(new XElement("IncludeInDocumentation", "true"));
                return store;
            }

            /// <summary>
            /// Builds the list of animal fodder pools used in the simulation,
            /// and stores the ID of the crops grown
            /// </summary>
            /// <param name="iat">Source IAT</param>
            public static void GetGrownFodderPools(IAT iat)
            {
                WorksheetPart crop = (WorksheetPart)iat.book.GetPartById(iat.SearchSheets("crop_inputs").Id);

                IATable gcg = iat.tables["Grain Crops Grown"];
                IATable gcs = iat.tables["Grain Crop Specifications"];

                var crops = gcg.GetRowData<int>(0);
                var areas = gcg.GetRowData<double>(2);
                var residue = gcg.GetRowData<double>(4);

                var names = gcs.GetRowNames();

                // Select crop ID and the index of the ID
                var ids =
                    from id in crops
                    where id != 0
                    select new int[2] { id, crops.IndexOf(id) };

                // Attempt to find the fodder pool for each crop
                foreach (int[] id in ids)
                {
                    // Find the cropname
                    string cropname = names.ElementAt(id[0] + 1);

                    // Check the crop has growing area
                    if (areas.ElementAt(id[1]) <= 0) continue;

                    // Add the crop to the list of grown grains 
                    if (!iat.grains.Exists(i => i == id[0])) iat.grains.Add(id[0]);

                    // Check if there is residue
                    if (residue.ElementAt(id[0]) <= 0) continue;

                    var rows = crop.Worksheet.Descendants<Row>().Skip(1);

                    // Find crop data
                    var inputs =
                        from row in rows
                        where TestRow(id, row, iat)
                        select row;

                    // Check data was found
                    if (!inputs.Any()) throw new Exception($"{cropname}: Crop not found in inputs sheet");

                    // Select the first row of the valid inputs
                    var input = inputs.First().Descendants<Cell>();

                    // Find what pool the residue uses
                    int.TryParse(iat.ParseCell(input.ElementAt(10)), out int pool);

                    // If the pool does not exist, create a new pool with the residue in it
                    if (!iat.pools.ContainsKey(pool)) iat.pools.Add(pool, $"{cropname}_Residue");
                    // If the pool exists already, add the residue to it.
                    else iat.pools[pool] = iat.pools[pool] + $", {cropname}_Residue";
                }
            }

            /// <summary>
            /// Tests a row of input data to see if it contains the desired ID
            /// </summary>
            /// <param name="id">ID to search for</param>
            /// <param name="row">Row to search in</param>
            /// <param name="iat">Source IAT</param>
            /// <returns></returns>
            private static bool TestRow(int[] id, Row row, IAT iat)
            {
                Cell cell = row.Descendants<Cell>().ElementAt(2);
                string content = iat.ParseCell(cell);

                if (content == id[0].ToString()) return true;
                else return false;
            }

            /// <summary>
            /// Checks if there is fodder that can be bought and ensures there
            /// is a pool to store it in
            /// </summary>
            /// <param name="iat"></param>
            public static void GetBoughtFodderPools(IAT iat)
            {
                IATable bf = iat.tables["Bought fodder"];
                IATable bfs = iat.tables["Bought fodder specs"];

                // Look at each fodder type in the table
                for (int row = 0; row < bf.GetRowNames().Count; row++)
                {
                    // Check if a fodder type can be bought
                    double unit = bf.GetData<double>(row, 0);
                    int month = bf.GetData<int>(row, 1);
                    if ((unit > 0) && (month > 0))
                    {
                        // Create appropriate storage pool if none exists
                        int pool = bf.GetData<int>(row, 4);
                        string cropname = bfs.GetRowNames()[row + 1];
                        if (!iat.pools.ContainsKey(pool)) iat.pools.Add(pool, cropname);
                        else iat.pools[pool] = iat.pools[pool] + $", {cropname}";
                    }
                }
            }
        }
    }
}
