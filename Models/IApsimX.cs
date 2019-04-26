using System.Collections.Generic;

namespace Models
{
    using Core;
    using CLEM.Activities;
    using CLEM.Resources;
    using CLEM.Groupings;
    public interface IApsimX
    {
        // PROPERTIES:
        // 
        string Name { get; set; }

        // RESOURCES:
        // Metadata methods
        Clock GetClock(Simulation simulation);

        // Land methods
        IEnumerable<LandType> GetLandTypes(Land parent);

        // Labour methods
        IEnumerable<LabourType> GetLabourTypes(Labour parent);

        IEnumerable<LabourAvailabilityItem> GetAvailabilityItems(LabourAvailabilityList parent);

        // Ruminant methods
        IEnumerable<RuminantType> GetRuminants(RuminantHerd parent);

        IEnumerable<RuminantTypeCohort> GetCohorts(RuminantInitialCohorts parent);

        IEnumerable<AnimalPriceGroup> GetAnimalPrices(AnimalPricing parent);

        // Finance methods
        void SetFinanceData(Finance finance);

        void SetBankData(FinanceType bank);

        // Store methods
        IEnumerable<AnimalFoodStoreType> GetAnimalStoreTypes(AnimalFoodStore store);

        IEnumerable<HumanFoodStoreType> GetHumanStoreTypes(HumanFoodStore store);

        IEnumerable<ProductStoreType> GetProductStoreTypes(ProductStore store);

        GrazeFoodStoreType GetGrazeFoodStore(GrazeFoodStore store);

        CommonLandFoodStoreType GetCommonFoodStore(AnimalFoodStore store);

        // ACTIVITIES
        // Finance activities
        ActivityFolder GetMonthlyExpenses(ActivityFolder cashflow);

        ActivityFolder GetAnnualExpenses(ActivityFolder cashflow);

        FinanceActivityCalculateInterest GetInterestRates(ActivityFolder cashflow);

        // Crop/Forage activities
        IEnumerable<CropActivityManageCrop> GetManageCrops(ActivityFolder parent);

        IEnumerable<CropActivityManageCrop> GetManageForages(ActivityFolder forages);

        IEnumerable<CropActivityManageCrop> GetNativePasture(ActivityFolder forages);

        // Ruminant activities
        IEnumerable<ActivityFolder> GetManageBreeds(ActivityFolder parent);
    }
}
