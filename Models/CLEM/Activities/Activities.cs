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

            ActivityFolder crops = new ActivityFolder(this) { Name = "Manage crops" };
            crops.Children.AddRange(Source.GetManageCrops(crops));

            ActivityFolder forages = new ActivityFolder(this) { Name = "Manage forages" };
            forages.Children.AddRange(Source.GetManageForages(forages));
            if (forages.Children.Count > 0) forages.Children.Add(Source.GetNativePasture(forages));

            GetHerd();

            new SummariseRuminantHerd(this);
            new ReportRuminantHerd(this);
        }

        private void GetCashFlow()
        {
            ActivityFolder cashflow = new ActivityFolder(this)
            {
                Name = "CashFlow"
            };
            Children.Add(Source.GetMonthlyExpenses(new ActivityFolder(cashflow) { Name = "ExpensesMonthly" }));
            Children.AddRange(Source.GetAnnualExpenses(new ActivityFolder(cashflow) { Name = "ExpensesAnnual" }));
            Children.Add(Source.GetInterestRates(new ActivityFolder(cashflow) { Name = "InterestRates" }));
        }

        private void GetHerd()
        {
            ActivityFolder herd = new ActivityFolder(this){Name = "Manage herd"};

            Children.Add(Source.GetManageBreeds(herd));
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
