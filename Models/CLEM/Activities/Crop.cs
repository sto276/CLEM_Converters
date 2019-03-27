using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    class CropActivityManageCrop : Node
    {
        public string LandItemNameToUse { get; set; }

        public double AreaRequested { get; set; }

        public bool UseAreaAvailable { get; set; } = false;

        public int OnPartialResourcesAvailableAction { get; set; }

        public CropActivityManageCrop(ActivitiesHolder parent) : base(parent)
        {

        }
    }

    class CropActivityManageProduct : Node
    {
        public string ModelNameFileCrop { get; set; }

        public string CropName { get; set; }

        public string StoreItemName { get; set; }

        public double ProportionKept { get; set; }

        public double TreesPerHa { get; set; }

        public double UnitsToHaConverter { get; set; }

        public int OnPartialResourcesAvailableAction { get; set; }

        public CropActivityManageProduct(CropActivityManageCrop parent) : base(parent)
        {

        }
    }

    class CropActivityManageTask : Node
    {
        public int OnPartialResourcesAvailableAction { get; set; }

        public CropActivityManageTask(CropActivityManageProduct parent) : base(parent)
        {

        }
    }

    class ActivityTimerCropHarvest : Node
    {
        public int OffsetMonthHarvestStart { get; set; }

        public int OffsetMonthHarvestStopo { get; set; }

        public ActivityTimerCropHarvest(Node parent) : base(parent)
        {

        }
    }
}
