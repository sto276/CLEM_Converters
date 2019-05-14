using Models.CLEM.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Reader
{    
    // Implements all methods related to IAT grain crops   
    public partial class IAT
    {               
        /// <summary>
        /// Search the grown crops for all valid crop types, and track
        /// them in the GrainIDs list
        /// </summary>
        private void SetGrains()
        {
            GrainIDs = new List<int>();

            // Row of crop IDs
            var crops = CropsGrown.GetRowData<int>(0);

            // Row of area allocated to crop
            var areas = CropsGrown.GetRowData<double>(2);
                        
            // Select all non-zero IDs
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
        /// Finds all crops in the source IAT which require management
        /// </summary>
        public IEnumerable<CropActivityManageCrop> GetManageCrops(ActivityFolder manage)
        {
            List<CropActivityManageCrop> crops = new List<CropActivityManageCrop>();

            int[] ids = CropsGrown.GetRowData<int>(0).ToArray();

            foreach (int id in GrainIDs)
            {
                // Find the name of the crop in the file
                Sheet sheet = SearchSheets("crop_inputs");
                WorksheetPart inputs = (WorksheetPart)Book.GetPartById(sheet.Id);

                // Find the name of the crop
                IEnumerable<Row> rows = sheet.Elements<Row>();
                string name = "Unknown";               
                int row = 1;

                while (row < rows.Count())
                {
                    if (GetCellValue(inputs, row, 3) == id.ToString())
                    {
                        name = GetCellValue(inputs, row, 4);
                        break;
                    }
                    row++;
                }

                // Find which column holds the data for a given crop ID
                int col = Array.IndexOf(ids, id);

                // Find names
                int land = CropsGrown.GetData<int>(1, col);
                string cropname = CropSpecs.RowNames[id + 1];

                // Model the crop management
                CropActivityManageCrop crop = new CropActivityManageCrop(manage)
                {
                    Name = "Manage " + cropname,
                    LandItemNameToUse = LandSpecs.RowNames[land - 1],
                    AreaRequested = CropsGrown.GetData<double>(2, col)
                };

                // Find the storage pool which the crop uses
                string pool = Pools.Values.ToList().Find(s => s.Contains(cropname));

                // Add the product management model
                crop.Add(new CropActivityManageProduct(crop)
                {
                    Name = "Manage grain",
                    ModelNameFileCrop = "FileCrop",
                    CropName = name,
                    ProportionKept = 1.0 - CropsGrown.GetData<double>(5, col) / 100.0,
                    StoreItemName = "HumanFoodStore." + pool
                });

                // Add the residue management
                crop.Add(new CropActivityManageProduct(crop)
                {
                    Name = "Manage residue",
                    ModelNameFileCrop = "FileCropResidue",
                    CropName = name,
                    ProportionKept = CropsGrown.GetData<double>(4, col) / 100.0,
                    StoreItemName = "AnimalFoodStore." + pool
                });

                crops.Add(crop);
            }
            return crops;
        }
    }
}