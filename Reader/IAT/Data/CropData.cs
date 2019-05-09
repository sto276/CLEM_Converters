using Models.CLEM.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Reader
{
    public partial class IAT
    {               
        private void SetGrains()
        {
            GrainIDs = new List<int>();

            var crops = CropsGrown.GetRowData<int>(0);
            var areas = CropsGrown.GetRowData<double>(2);
            var residue = CropsGrown.GetRowData<double>(4);

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
                if (!GrainIDs.Exists(i => i == id)) GrainIDs.Add(id);
            }
        }       

        /// <summary>
        /// Writes the 'Manage Crops' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="iat"></param>
        public IEnumerable<CropActivityManageCrop> GetManageCrops(ActivityFolder manage)
        {
            List<CropActivityManageCrop> crops = new List<CropActivityManageCrop>();

            int[] ids = CropsGrown.GetRowData<int>(0).ToArray();

            foreach (int id in GrainIDs)
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
                int land = CropsGrown.GetData<int>(1, col);
                string cropname = CropSpecs.RowNames[id + 1];

                CropActivityManageCrop crop = new CropActivityManageCrop(manage)
                {
                    Name = "Manage " + cropname,
                    LandItemNameToUse = LandSpecs.RowNames[land - 1],
                    AreaRequested = CropsGrown.GetData<double>(2, col)
                };

                // Find the storage pool
                string pool = Pools.Values.ToList().Find(s => s.Contains(cropname));

                // Add the crop management
                new CropActivityManageProduct(crop)
                {
                    Name = "Manage grain",
                    ModelNameFileCrop = "FileCrop",
                    CropName = inputname,
                    ProportionKept = 1.0 - CropsGrown.GetData<double>(5, col) / 100.0,
                    StoreItemName = "HumanFoodStore." + pool
                };

                // Add the residue management
                new CropActivityManageProduct(crop)
                {
                    Name = "Manage residue",
                    ModelNameFileCrop = "FileCropResidue",
                    CropName = inputname,
                    ProportionKept = CropsGrown.GetData<double>(4, col) / 100.0,
                    StoreItemName = "AnimalFoodStore." + pool
                };

                crops.Add(crop);
            }
            return crops;
        }
    }
}