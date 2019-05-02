namespace Models.CLEM.Activities
{
    using CLEM.Reporting;

    public class ActivityNode : Node
    {
        public object SelectedTab { get; set; } = null;

        public int OnPartialResourcesAvailableAction { get; set; } = 0;

        public ActivityNode(Node parent) : base(parent)
        { }
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
            crops.Add(Source.GetManageCrops(crops));
            Add(crops);

            ActivityFolder forages = new ActivityFolder(this) { Name = "Manage forages" };
            forages.Add(Source.GetManageForages(forages));
            if (forages.Children.Count > 0) forages.Add(Source.GetNativePasture(forages));
            Add(forages);

            GetHerd();

            Add(Source.GetManagePasture(this));
            Add(new SummariseRuminantHerd(this));
            Add(new ReportRuminantHerd(this));
        }

        private void GetCashFlow()
        {
            ActivityFolder cashflow = new ActivityFolder(this)
            {
                Name = "CashFlow"
            };
            cashflow.Add(Source.GetMonthlyExpenses(cashflow));
            cashflow.Add(Source.GetAnnualExpenses(cashflow));
            cashflow.Add(Source.GetInterestRates(cashflow));

            Add(cashflow);
        }

        private void GetHerd()
        {
            ActivityFolder herd = new ActivityFolder(this){Name = "Manage herd"};

            herd.Add(Source.GetManageBreeds(herd));
            herd.Add(new RuminantActivityGrazeAll(herd));
            herd.Add(new RuminantActivityGrow(herd));
            herd.Add(new RuminantActivityBreed(herd));
            herd.Add(new RuminantActivityBuySell(herd));
            herd.Add(new RuminantActivityMuster(herd));

            Add(herd);
        }
    }

    public class ActivityFolder : ActivityNode
    {
        public ActivityFolder(Node parent) : base(parent)
        {  }
    }

    public class ActivityTimerInterval : Node
    {
        public int Interval { get; set; } = 12;

        public int MonthDue { get; set; } = 12;

        public ActivityTimerInterval(Node parent) : base(parent)
        {  }
    }

    public class Relationship : Node
    {
        public double StartingValue { get; set; } = 0;

        public double Minimum { get; set; } = 0;

        public double Maximum { get; set; } = 0;

        public double[] XValues { get; set; } = new[] { 0.0, 20.0, 30.0, 100.0 };

        public double[] YValues { get; set; } = new[] { -0.625, -0.125, 0, 0.75 };

        public Relationship(Node parent) : base(parent)
        { }
    }
}
