using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    class CropActivityManageCrop : ActivityNode
    {
        public string LandItemNameToUse { get; set; }

        public double AreaRequested { get; set; }

        public bool UseAreaAvailable { get; set; } = false;

        public CropActivityManageCrop(ActivitiesHolder parent) : base(parent)
        {

        }
    }

    class CropActivityManageProduct : ActivityNode
    {
        public string ModelNameFileCrop { get; set; }

        public string CropName { get; set; }

        public string StoreItemName { get; set; }

        public double ProportionKept { get; set; }

        public double TreesPerHa { get; set; }

        public double UnitsToHaConverter { get; set; }

        public CropActivityManageProduct(CropActivityManageCrop parent) : base(parent)
        {

        }
    }

    class CropActivityManageTask : ActivityNode
    {
        public CropActivityManageTask(CropActivityManageProduct parent) : base(parent)
        {

        }
    }

    class ActivityTimerCropHarvest : ActivityNode
    {
        public int OffsetMonthHarvestStart { get; set; }

        public int OffsetMonthHarvestStopo { get; set; }

        public ActivityTimerCropHarvest(ActivityNode parent) : base(parent)
        {

        }
    }
}
