using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    public class ActivityNode : Node
    {
        public object SelectedTab { get; set; } = null;

        public int OnPartialResourcesAvailableAction { get; set; } = 0;

        public ActivityNode(Node parent) : base(parent)
        {

        }
    }

    public class ActivitiesHolder : Node
    {
        public string LastShortfallResourceRequest { get; set; }

        public string LastActivityPerformed { get; set; }

        public ActivitiesHolder(ZoneCLEM parent) : base(parent)
        {
            Name = "Activities";
            GetCashFlow();
            GetForages();
            GetHerd();

            new SummariseRuminantHerd(this);
            new ReportRuminantHerd(this);
        }

        private void GetCashFlow()
        {
            ActivityFolder cashFlow = new ActivityFolder(this)
            {
                Name = "CashFlow"
            };
            
            Source.GetMonthlyExpenses(new ActivityFolder(cashFlow) { Name = "ExpensesMonthly" });
            Source.GetAnnualExpenses(new ActivityFolder(cashFlow) { Name = "ExpensesAnnual" });
        }

        private void GetForages()
        {
            ActivityFolder forages = new ActivityFolder(this)
            {
                Name = "Manage forages"
            };

            CropActivityManageCrop farm = new CropActivityManageCrop(forages)
            {
                LandItemNameToUse = "Land",
                UseAreaAvailable = true,
                Name = "NativePastureFarm"
            };

            new CropActivityManageProduct(farm)
            {
                ModelNameFileCrop = "FileForage",
                CropName = "Native_grass",
                StoreItemName = "AnimalFoodStore.NativePasture",
                Name = "Cut and carry Native Pasture"
            };

            CropActivityManageCrop common = new CropActivityManageCrop(forages)
            {
                LandItemNameToUse = "Land",
                Name = "Native Pasture Common Land"
            };

            new CropActivityManageProduct(common)
            {
                ModelNameFileCrop = "FileForage",
                CropName = "Native_grass",
                StoreItemName = "GrazeFoodStore.NativePasture",
                Name = "Grazed Common Land"
            };
        }

        private void GetHerd()
        {
            ActivityFolder herd = new ActivityFolder(this){Name = "Manage herd"};

            Source.GetManageRuminants(herd);
            new ActivityFolder(herd){Name = "Cut and carry"};
            new RuminantActivityGrazeAll(herd);
            new RuminantActivityGrow(herd);
            new RuminantActivityBreed(herd);
            new RuminantActivityBuySell(herd);
            new RuminantActivityMuster(herd);
        }
    }

    public class ActivityFolder : ActivityNode
    {
        public int test { get; set; }

        public ActivityFolder(Node parent) : base(parent)
        {

        }
    }

    public class ActivityTimerInterval : Node
    {
        public int Interval { get; set; } = 12;

        public int MonthDue { get; set; } = 12;

        public ActivityTimerInterval(Node parent) : base(parent)
        {

        }
    }
}
