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
        ICollection<LandType> GetLandTypes(Land parent);

        // Labour methods
        ICollection<LabourType> GetLabourTypes(Labour parent);

        ICollection<LabourAvailabilityItem> GetAvailabilityItems(LabourAvailabilityList parent);

        ICollection<LabourFilter> GetLabourFilters(Node parent);

        // Ruminant methods
        ICollection<RuminantType> GetRuminants(RuminantHerd parent);

        ICollection<RuminantTypeCohort> GetCohorts(RuminantType parent);

        ICollection<AnimalPriceGroup> GetAnimalPrices(RuminantType parent);

        // Finance methods
        void SetFinanceData(FinanceType parent);

        // Store methods
        void GetAnimalStoreTypes(AnimalFoodStore parent);

        void SetGrazeData(GrazeFoodStoreType parent);

        void GetProductStoreTypes(ProductStore parent);
    }
}
