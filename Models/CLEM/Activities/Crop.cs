﻿namespace Models.CLEM.Activities
{
    /// <summary>
    /// Models the management of crops
    /// </summary>
    public class CropActivityManageCrop : ActivityNode
    {
        public string LandItemNameToUse { get; set; } = "";

        public double AreaRequested { get; set; } = 0.0;

        public bool UseAreaAvailable { get; set; } = false;

        public CropActivityManageCrop(Node parent) : base(parent)
        {  }
    }

    /// <summary>
    /// Models the management of crop products
    /// </summary>
    public class CropActivityManageProduct : ActivityNode
    {
        public string ModelNameFileCrop { get; set; } = "";

        public string CropName { get; set; } = "";

        public string StoreItemName { get; set; } = "";

        public double ProportionKept { get; set; } = 1.0;

        public double TreesPerHa { get; set; } = 0.0;

        public double UnitsToHaConverter { get; set; } = 0.0;

        public CropActivityManageProduct(CropActivityManageCrop parent) : base(parent)
        { }
    }    
}
