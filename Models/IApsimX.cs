using System.Collections.Generic;

namespace Models
{
    using Core;
    using CLEM;
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

        IEnumerable<Node> GetFiles(ZoneCLEM clem);

        // Land methods
        IEnumerable<LandType> GetLandTypes(Land land);

        // Labour methods
        IEnumerable<LabourType> GetLabourTypes(Labour labour);

        IEnumerable<LabourAvailabilityItem> GetAvailabilityItems(LabourAvailabilityList list);

        // Ruminant methods
        IEnumerable<RuminantType> GetRuminants(RuminantHerd herd);

        IEnumerable<RuminantTypeCohort> GetCohorts(RuminantInitialCohorts cohorts);

        IEnumerable<AnimalPriceGroup> GetAnimalPrices(AnimalPricing pricing);

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
        IEnumerable<CropActivityManageCrop> GetManageCrops(ActivityFolder folder);

        IEnumerable<CropActivityManageCrop> GetManageForages(ActivityFolder forages);

        IEnumerable<CropActivityManageCrop> GetNativePasture(ActivityFolder forages);

        PastureActivityManage GetManagePasture(ActivitiesHolder folder);

        // Ruminant activities
        IEnumerable<ActivityFolder> GetManageBreeds(ActivityFolder folder);
    }
}
