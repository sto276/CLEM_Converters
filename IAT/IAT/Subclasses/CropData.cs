using Models.CLEM.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ReadIAT
{
    public partial class IAT
    {
        /// <summary>
        /// Provides methods for reading and converting
        /// the grain crop data contained within an IAT file
        /// </summary>
        private static class CropData
        {
            public static List<int> Columns { get; private set; }

            public static SubTable Grown { get; private set; }

            public static SubTable Specs { get; private set; }

            public static void Construct(IAT source)
            {
                Grown = new SubTable("Grain Crops Grown", source);
                Specs = new SubTable("Grain Crop Specifications", source);

                Columns = GetGrains();
            }

            private static List<int> GetGrains()
            {
                List<int> grains = new List<int>();

                var crops = Grown.GetRowData<int>(0);
                var areas = Grown.GetRowData<double>(2);
                var residue = Grown.GetRowData<double>(4);

                // Select crop ID and the index of the ID
                var ids =
                    from id in crops
                    where id != 0
                    select id;

                foreach (int id in ids)
                {
                    int index = crops.IndexOf(id);

                    // Check the crop has growing area
                    if (areas.ElementAt(index) <= 0) continue;

                    // Add the crop to the list of grown grains 
                    if (!grains.Exists(i => i == id)) grains.Add(id);
                }

                return grains;
            }         

        }

        /// <summary>
        /// Writes the 'Manage Crops' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="iat"></param>
        public IEnumerable<CropActivityManageCrop> GetManageCrops(ActivityFolder manage)
        {
            List<CropActivityManageCrop> crops = new List<CropActivityManageCrop>();

            int[] ids = CropData.Grown.GetRowData<int>(0).ToArray();

            foreach (int id in CropData.Columns)
            {
                // Find the name of the crop in the file
                Sheet sheet = SearchSheets("crop_inputs");
                WorksheetPart inputs = (WorksheetPart)Book.GetPartById(sheet.Id);
                string inputname = "Unknown";
                int row = 1;

                IEnumerable<Row> rows = sheet.Elements<Row>();
                while (row < rows.Count())
                {
                    if (GetCellValue(inputs, row, 3) == id.ToString())
                        inputname = GetCellValue(inputs, row, 4);
                    row++;
                }

                // Find which column holds the data for a given crop ID
                int col = Array.IndexOf(ids, id);

                // Find names
                int land = CropData.Grown.GetData<int>(1, col);
                string cropname = CropData.Specs.RowNames[id + 1];

                CropActivityManageCrop crop = new CropActivityManageCrop(manage)
                {
                    Name = "Manage " + cropname,
                    LandItemNameToUse = LandData.Specs.RowNames[land - 1],
                    AreaRequested = CropData.Grown.GetData<double>(2, col)
                };

                // Find the storage pool
                string pool = StoreData.Pools.Values.ToList().Find(s => s.Contains(cropname));

                // Add the crop management
                new CropActivityManageProduct(crop)
                {
                    Name = "Manage grain",
                    ModelNameFileCrop = "FileCrop",
                    CropName = inputname,
                    ProportionKept = 1.0 - CropData.Grown.GetData<double>(5, col) / 100.0,
                    StoreItemName = "HumanFoodStore." + pool
                };

                // Add the residue management
                new CropActivityManageProduct(crop)
                {
                    Name = "Manage residue",
                    ModelNameFileCrop = "FileCropResidue",
                    CropName = inputname,
                    ProportionKept = CropData.Grown.GetData<double>(4, col) / 100.0,
                    StoreItemName = "AnimalFoodStore." + pool
                };

                crops.Add(crop);
            }
            return crops;
        }
    }
}