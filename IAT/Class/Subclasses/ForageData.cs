using Models.CLEM.Activities;
using System.Collections.Generic;
using System.Linq;

namespace ReadIAT
{
    public partial class IAT
    {
        /// <summary>
        /// Provides methods for reading and converting
        /// the forage crop data contained within an IAT file
        /// </summary>
        private static class ForageData
        {
            public static SubTable Grown { get; private set; }

            public static SubTable Specs { get; private set; }

            public static List<int> Columns { get; private set; }

            public static void Construct(IAT source)
            {
                Grown = new SubTable("Forage Crops Grown", source);
                Specs = new SubTable("Forage Crop Specifications", source);

                Columns = Grown.GetRowData<int>(0).Where(n => n != 0).ToList();
            }                        
        }

        /// <summary>
        /// Writes the 'Manage forages' activity segment of an .apsimx file
        /// </summary>
        /// <param name="iat">The IAT file to access data from</param>
        public IEnumerable<CropActivityManageCrop> GetManageForages(ActivityFolder forages)
        {
            List<CropActivityManageCrop> manage = new List<CropActivityManageCrop>();

            int col = 0;
            // Check present forages
            var list = ForageData.Grown.GetRowData<double>(0);
            foreach (var item in list)
            {
                if (item == 0) continue;

                double area = ForageData.Grown.GetData<double>(2, col);
                if (area > 0)
                {
                    int num = ForageData.Grown.GetData<int>(1, col);
                    int row = ForageData.Grown.GetData<int>(0, col);
                    string name = ForageData.Specs.RowNames[row + 1];

                    CropActivityManageCrop crop = new CropActivityManageCrop(forages)
                    {
                        Name = "Manage " + name,
                        LandItemNameToUse = "Land." + LandData.Specs.RowNames[num],
                        AreaRequested = ForageData.Grown.GetData<double>(2, col)
                    };

                    new CropActivityManageProduct(crop)
                    {
                        Name = "Cut and carry " + name,
                        ModelNameFileCrop = "FileForage",
                        CropName = name,
                        StoreItemName = "AnimalFoodStore"
                    };
                    manage.Add(crop);
                }
                col++;
            }

            return manage.AsEnumerable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public ActivityFolder GetNativePasture(ActivityFolder forages)
        {
            // Check if there are native pasture feeds before proceeding
            var feed_type = RuminantData.Specs.GetRowData<int>(28);
            var feeds = from breed in feed_type
                    where RuminantData.Columns.Contains(feed_type.IndexOf(breed))
                    where breed > 1
                    select breed;
            if (feeds.Count() == 0) return null;               

            CropActivityManageCrop farm = new CropActivityManageCrop(forages)
            {
                LandItemNameToUse = "Land",
                UseAreaAvailable = true,
                Name = "NativePastureFarm"
            };

            new CropActivityManageProduct(farm)
            {
                ModelNameFileCrop = "FileForage",
                CropName = "Native_grass",
                StoreItemName = "AnimalFoodStore.NativePasture",
                Name = "Cut and carry Native Pasture"
            };

            CropActivityManageCrop common = new CropActivityManageCrop(forages)
            {
                LandItemNameToUse = "Land",
                Name = "Native Pasture Common Land"
            };

            new CropActivityManageProduct(common)
            {
                ModelNameFileCrop = "FileForage",
                CropName = "Native_grass",
                StoreItemName = "GrazeFoodStore.NativePasture",
                Name = "Grazed Common Land"
            };

            return forages;
        }
    }
}
