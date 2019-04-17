using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interface
{
    using CLEM.Resources;
    using CLEM.Groupings;
    public partial interface IApsimX
    {
        // Land methods
        IEnumerable<LandType> GetLandTypes(Land parent);

        // Labour methods
        IEnumerable<LabourType> GetLabourTypes(Labour parent);

        IEnumerable<LabourAvailabilityItem> GetAvailabilityItems(LabourAvailabilityList parent);

        IEnumerable<LabourFilter> GetLabourFilters(Node parent);

        // Ruminant methods
        IEnumerable<RuminantType> GetRuminants(RuminantHerd parent);

        IEnumerable<RuminantTypeCohort> GetCohorts(RuminantInitialCohorts parent);

        IEnumerable<AnimalPriceGroup> GetAnimalPrices(AnimalPricing parent);

        void SetSellDryData();

        // Finance methods
        void SetFinanceData(Finance finance);

        void SetBankData(FinanceType bank);

        // Store methods
        void GetAnimalStoreTypes(AnimalFoodStore parent);

        void SetGrazeData(GrazeFoodStoreType parent);

        void GetProductStoreTypes(ProductStore parent);
    }
}
