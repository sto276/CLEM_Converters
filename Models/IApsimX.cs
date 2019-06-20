using Models.Core;
using Models.CLEM;
using Models.CLEM.Activities;
using Models.CLEM.Resources;
using Models.CLEM.Groupings;
using System;
using System.Collections.Generic;

namespace Models
{
    /// <summary>
    /// Interface for accessing the data needed to generate an ApsimX
    /// model from external source.
    /// </summary>
    public interface IApsimX : IDisposable
    {
        // PROPERTIES:  
        /// <summary>
        /// Name of the source
        /// </summary>
        string Name { get; set; }


        // RESOURCES:
        // Metadata methods
        /// <summary>
        /// Searches the source for the simulation clock
        /// </summary>
        Clock GetClock(Simulation simulation);

        /// <summary>
        /// Construct or attach additional files through the source
        /// </summary>
        IEnumerable<Node> GetFiles(ZoneCLEM clem);

        // Land methods
        /// <summary>
        /// Search the source for land types
        /// </summary>
        IEnumerable<LandType> GetLandTypes(Land land);

        // Labour methods
        /// <summary>
        /// Search the source for labour types
        /// </summary>
        IEnumerable<LabourType> GetLabourTypes(Labour labour);

        /// <summary>
        /// Search the source for the availability of labour
        /// </summary>
        IEnumerable<LabourAvailabilityItem> GetAvailabilityItems(LabourAvailabilityList list);

        // Ruminant methods
        /// <summary>
        /// Search the source for present ruminant breeds
        /// </summary>
        IEnumerable<RuminantType> GetRuminants(RuminantHerd herd);

        /// <summary>
        /// Search the source for cohorts present in a given ruminant breed
        /// </summary>
        IEnumerable<RuminantTypeCohort> GetCohorts(RuminantInitialCohorts cohorts);

        /// <summary>
        /// Search the source for the pricing of ruminants
        /// </summary>
        IEnumerable<AnimalPriceGroup> GetAnimalPrices(AnimalPricing pricing);
        
        // Finance methods
        /// <summary>
        /// Set the finance data from the source
        /// </summary>
        void SetFinanceData(Finance finance);

        /// <summary>
        /// Set the bank data from the source
        /// </summary>
        void SetBankData(FinanceType bank);

        // Store methods
        /// <summary>
        /// Search the source for types of animal fodder
        /// </summary>
        IEnumerable<AnimalFoodStoreType> GetAnimalStoreTypes(AnimalFoodStore store);

        /// <summary>
        /// Search the source for types of human food
        /// </summary>
        IEnumerable<HumanFoodStoreType> GetHumanStoreTypes(HumanFoodStore store);

        /// <summary>
        /// Search the source for miscellaneous products
        /// </summary>
        IEnumerable<ProductStoreType> GetProductStoreTypes(ProductStore store);

        /// <summary>
        /// Constructs the graze food store
        /// </summary>
        GrazeFoodStoreType GetGrazeFoodStore(GrazeFoodStore store);

        /// <summary>
        /// Constructs the common food store
        /// </summary>
        CommonLandFoodStoreType GetCommonFoodStore(AnimalFoodStore store);


        // ACTIVITIES
        // Finance activities
        /// <summary>
        /// Search the source for monthly expenses
        /// </summary>
        ActivityFolder GetMonthlyExpenses(ActivityFolder cashflow);

        /// <summary>
        /// Search the source for annual expenses
        /// </summary>
        ActivityFolder GetAnnualExpenses(ActivityFolder cashflow);

        /// <summary>
        /// Search the source for the interest rates
        /// </summary>
        FinanceActivityCalculateInterest GetInterestRates(ActivityFolder cashflow);

        // Crop/Forage activities
        /// <summary>
        /// Search the source for crops to manage
        /// </summary>
        IEnumerable<CropActivityManageCrop> GetManageCrops(ActivityFolder folder);

        /// <summary>
        /// Search the source for forages to manage
        /// </summary>
        IEnumerable<CropActivityManageCrop> GetManageForages(ActivityFolder forages);

        /// <summary>
        /// Search the source for native pasture to manage
        /// </summary>
        IEnumerable<CropActivityManageCrop> GetNativePasture(ActivityFolder forages);

        /// <summary>
        /// Search the source for normal pasture to manage
        /// </summary>
        PastureActivityManage GetManagePasture(ActivitiesHolder folder);

        // Ruminant activities
        /// <summary>
        /// Search the source for breeds to manage
        /// </summary>
        IEnumerable<ActivityFolder> GetManageBreeds(ActivityFolder folder);
    }
}
