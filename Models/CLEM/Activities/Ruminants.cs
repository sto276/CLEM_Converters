namespace Models.CLEM.Activities
{
    using CLEM.Groupings;

    public class RuminantActivityBreed : ActivityNode
    {
        public double MaximumConceptionRateUncontrolled { get; set; } = 0.8;

        public RuminantActivityBreed(Node parent) : base(parent)
        {
            Name = "Breed";
        }
    }

    public  class RuminantActivityBuySell : ActivityNode
    {
        public string BankAccountName { get; set; } = "Finances.Bank";

        public RuminantActivityBuySell(Node parent) : base(parent)
        {
            Name = "BuySell";
        }
    }

    public class RuminantActivityFeed : ActivityNode
    {
        public string FeedTypeName { get; set; }

        public double ProportionTramplingWastage { get; set; } = 0.3;

        public string FeedStyle { get; set; } = "ProportionOfPotentialIntake";

        public RuminantActivityFeed(Node parent) : base(parent)
        { }
    }

    public class RuminantActivityGrazeAll : ActivityNode
    {
        public double HoursGrazed { get; set; } = 8.0;

        public RuminantActivityGrazeAll(Node parent) : base(parent)
        {
            Name = "GrazeAll";
            OnPartialResourcesAvailableAction = 2;

            var labour = new LabourRequirement(this);
            var group = new LabourFilterGroup(labour);
            var filter = new LabourFilter(group)
            {
                Parameter = 0,
                Operator = 0,
                Value = "Male"
            };
            group.Add(filter);
            labour.Add(group);
        }
    }

    public class RuminantActivityGrow : ActivityNode
    {
        public double EnergyGross { get; set; } = 18.4;

        public RuminantActivityGrow(Node parent) : base(parent)
        {
            Name = "GrowAll";
        }
    }

    public class RuminantActivityManage : ActivityNode
    {
        public int MaximumBreedersKept { get; set; } = 8;

        public int MinimumBreedersKept { get; set; } = 4;

        public int MaximumBreedingAge { get; set; } = 144;

        public double MaximumProportionBreedersPerPurchase { get; set; } = 1;

        public int NumberOfBreederPurchaseAgeClasses { get; set; } = 1;

        public double MaximumSiresKept { get; set; } = 0;

        public double MaximumBullAge { get; set; } = 96;

        public bool AllowSireReplacement { get; set; } = false;

        public int MaximumSiresPerPurchase { get; set; } = 0;

        public double MaleSellingAge { get; set; } = 1;

        public double MaleSellingWeight { get; set; } = 450;

        public string GrazeFoodStoreName { get; set; } = "GrazeFoodStore.NativePasture";

        public bool SellFemalesLikeMales { get; set; } = false;

        public bool ContinuousMaleSales { get; set; } = false;

        public RuminantActivityManage(Node parent) : base(parent)
        {
            Name = "Manage numbers";
        }
    }

    public class RuminantActivityMilking : ActivityNode
    {
        public string ResourceTypeName { get; set; } = "HumanFoodStore.Milk";

        public RuminantActivityMilking(Node parent) : base(parent)
        { }
    }

    public class RuminantActivityMuster : ActivityNode
    {
        public string ManagedPastureName { get; set; } = "GrazeFoodStore.NativePasture";

        public bool PerformAtStartOfSimulation { get; set; } = true;

        public bool MoveSucklings { get; set; } = true;

        public RuminantActivityMuster(Node parent) : base(parent)
        {
            Name = "Muster";
        }
    }

    public class RuminantActivitySellDryBreeders : ActivityNode
    {
        public double MinimumConceptionBeforeSell { get; set; } = 1.0;

        public double MonthsSinceBirth { get; set; } = 0.0;

        public double ProportionToRemove { get; set; } = 0.0;

        public RuminantActivitySellDryBreeders(Node parent) : base(parent)
        {
            Name = "SellDryBreeders";
        }
    }

    public class RuminantActivityWean : ActivityNode
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
