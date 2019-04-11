using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    class RuminantActivityBreed : ActivityNode
    {
        public double MaximumConceptionRateUncontrolled { get; set; } = 0.0;

        public RuminantActivityBreed(Node parent) : base(parent)
        {
            Name = "Breed";
        }
    }

    class RuminantActivityBuySell : ActivityNode
    {
        public string BankAccountName { get; set; } = "Finances.Bank";

        public RuminantActivityBuySell(Node parent) : base(parent)
        {
            Name = "BuySell";
        }
    }
    
    class RuminantActivityGrazeAll : ActivityNode
    {
        public double HoursGrazed { get; set; } = 8.0;

        public RuminantActivityGrazeAll(Node parent) : base(parent)
        {
            Name = "GrazeAll";

            OnPartialResourcesAvailableAction = 2;
        }
    }

    class RuminantActivityGrow : ActivityNode
    {
        public double EnergyGross { get; set; } = 0.0;

        public RuminantActivityGrow(Node parent) : base(parent)
        {
            Name = "Grow all ruminants";
        }
    }

    class RuminantActivityManage : ActivityNode
    {
        public int MaximumBreedersKept { get; set; } = 0;

        public int MinimumBreedersKept { get; set; } = 0;

        public double MaximumBreedingAge { get; set; } = 0.0;

        public double MaximumProportionBreedersPerPurchase { get; set; } = 0.0;

        public double MaximumSiresKept { get; set; } = 0.0;

        public double MaximumBullAge { get; set; } = 0.0;

        public bool AllowSireReplacement { get; set; } = false;

        public int MaximumSiresPerPurchase { get; set; } = 0;

        public double MaleSellingAge { get; set; } = 0.0;

        public double MaleSellingWeight { get; set; } = 0.0;

        public string GrazeFoodStoreName { get; set; } = "GrazeFoodStore";

        public bool SellFemalesLikeMales { get; set; } = false;

        public bool ContinuousMaleSales { get; set; } = false;

        public string HerdFilters { get; set; } = null;

        public RuminantActivityManage(Node parent) : base(parent)
        {
            Name = "Manage numbers";
        }
    }

    class RuminantActivityMuster : ActivityNode
    {
        public string ManagedPastureName { get; set; } = "GrazeFoodStore.NativePasture";

        public bool PerformAtStartOfSimulation { get; set; } = true;

        public bool MoveSucklings { get; set; } = true;

        public RuminantActivityMuster(Node parent) : base(parent)
        {
            Name = "Muster";
        }
    }

    class RuminantActivitySellDryBreeders : ActivityNode
    {
        public double MinimumConceptionBeforeSell { get; set; } = 1.0;

        public double MonthsSinceBirth { get; set; } = 0.0;

        public double ProportionToRemove { get; set; } = 0.0;

        public RuminantActivitySellDryBreeders(Node parent) : base(parent)
        {
            Name = "SellDryBreeders";
        }
    }

    class RuminantActivityWean : ActivityNode
    {
        public double WeaningAge { get; set; } = 0.0;

        public double WeaningWeight { get; set; } = 0.0;

        public string GrazeFoodStoreName { get; set; } = null;

        public string HerdFilters { get; set; } = null;

        public RuminantActivityWean(Node parent) : base(parent)
        {
            Name = "Wean";
        }
    }

}
