using Models.CLEM.Activities;
using System.Collections.Generic;
using System.Linq;

namespace Reader
{
    public partial class IAT
    {       
        /// <summary>
        /// Constructs the Manage Forages model
        /// </summary>
        /// <param name="iat">The IAT file to access data from</param>
        public IEnumerable<CropActivityManageCrop> GetManageForages(ActivityFolder forages)
        {
            List<CropActivityManageCrop> manage = new List<CropActivityManageCrop>();

            int col = 0;
            // Check present forages
            var list = ForagesGrown.GetRowData<double>(0);
            foreach (var item in list)
            {
                if (item == 0) continue;

                double area = ForagesGrown.GetData<double>(2, col);
                if (area > 0)
                {
                    int num = ForagesGrown.GetData<int>(1, col);
                    int row = ForagesGrown.GetData<int>(0, col);
                    string name = ForageSpecs.RowNames[row + 1];

                    CropActivityManageCrop crop = new CropActivityManageCrop(forages)
                    {
                        Name = "Manage " + name,
                        LandItemNameToUse = "Land." + LandSpecs.RowNames[num],
                        AreaRequested = ForagesGrown.GetData<double>(2, col)
                    };

                    crop.Add(new CropActivityManageProduct(crop)
                    {
                        Name = "Cut and carry " + name,
                        ModelNameFileCrop = "FileForage",
                        CropName = name,
                        StoreItemName = "AnimalFoodStore"
                    });

                    manage.Add(crop);
                }
                col++;
            }

            return manage.AsEnumerable();
        }

        /// <summary>
        /// Constructs the Native Pasture model
        /// </summary>
        public IEnumerable<CropActivityManageCrop> GetNativePasture(ActivityFolder forages)
        {
            List<CropActivityManageCrop> pastures = new List<CropActivityManageCrop>();

            // Check if there are native pasture feeds before proceeding
            var feed_type = RumSpecs.GetRowData<int>(28);
            var feeds = from breed in feed_type
                    where RumIDs.Contains(feed_type.IndexOf(breed))
                    where breed > 1
                    select breed;
            if (feeds.Count() == 0) return null;               

            CropActivityManageCrop farm = new CropActivityManageCrop(forages)
            {
                LandItemNameToUse = "Land",
                UseAreaAvailable = true,
                Name = "NativePastureFarm"
            };

            farm.Add(new CropActivityManageProduct(farm)
            {
                ModelNameFileCrop = "FileForage",
                CropName = "Native_grass",
                StoreItemName = "AnimalFoodStore.NativePasture",
                Name = "Cut and carry Native Pasture"
            });

            CropActivityManageCrop common = new CropActivityManageCrop(forages)
            {
                LandItemNameToUse = "Land",
                Name = "Native Pasture Common Land"
            };

            common.Add(new CropActivityManageProduct(common)
            {
                ModelNameFileCrop = "FileForage",
                CropName = "Native_grass",
                StoreItemName = "GrazeFoodStore.NativePasture",
                Name = "Grazed Common Land"
            });

            pastures.Add(farm);
            pastures.Add(common);
            return pastures.AsEnumerable();
        }
    }
}
