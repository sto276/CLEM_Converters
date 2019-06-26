using Models.CLEM.Reporting;
using Models.CLEM.Resources;

namespace Models.CLEM.Activities
{
    /// <summary>
    /// Generic base node for all activity models. 
    /// This node should not be instantiated directly.
    /// </summary>
    public class ActivityNode : Node
    {
        public object SelectedTab { get; set; } = null;

        public int OnPartialResourcesAvailableAction { get; set; } = 0;

        public ActivityNode(Node parent) : base(parent)
        { }
    }

    /// <summary>
    /// The activities holder is the model which contains all other activities.
    /// </summary>
    public class ActivitiesHolder : Node
    {
        public string LastShortfallResourceRequest { get; set; }

        public string LastActivityPerformed { get; set; }

        public ActivitiesHolder(ZoneCLEM parent) : base(parent)
        {
            Name = "Activities";

            // Model the finance activities
            GetCashFlow();

            // Model the crop growth activities
            ActivityFolder crops = new ActivityFolder(this) { Name = "Manage crops" };
            crops.Add(Source.GetManageCrops(crops));
            Add(crops);

            // Model the forage growth activities
            ActivityFolder forages = new ActivityFolder(this) { Name = "Manage forages" };
            forages.Add(Source.GetManageForages(forages));
            if (forages.Children.Count > 0) forages.Add(Source.GetNativePasture(forages));
            Add(forages);

            // Model the ruminant activities
            GetHerd();

            // Model the pasture management
            Add(Source.GetManagePasture(this));

            // Attach summary/report
            Add(new SummariseRuminantHerd(this));
            Add(new ReportRuminantHerd(this));
        }

        /// <summary>
        /// Models the finances in the simulation
        /// </summary>
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

        /// <summary>
        /// Models the management of the ruminant herd in the simulation
        /// </summary>
        private void GetHerd()
        {
            ActivityFolder herd = new ActivityFolder(this){Name = "Manage herd"};

            herd.Add(Source.GetManageBreeds(herd));
            herd.Add(GetFeed(herd));
            herd.Add(new RuminantActivityGrazeAll(herd));
            herd.Add(new RuminantActivityGrow(herd));
            herd.Add(new RuminantActivityBreed(herd));
            herd.Add(new RuminantActivityBuySell(herd));
            herd.Add(new RuminantActivityMuster(herd));

            Add(herd);
        }

        /// <summary>
        /// Add a RuminantActivityFeed for each item in the AnimalFoodStore
        /// </summary>
        private ActivityFolder GetFeed(ActivityFolder herd)
        {            
            AnimalFoodStore store = SearchTree<AnimalFoodStore>((ZoneCLEM)Parent);

            if (store.Children.Count == 0) return null; 

            ActivityFolder feed = new ActivityFolder(herd) { Name = "Feed ruminants" };            

            foreach (Node child in store.Children)
            {
                feed.Add(new RuminantActivityFeed(feed)
                {
                    Name = "Feed " + child.Name,
                    FeedTypeName = "AnimalFoodStore." + child.Name
                });
            }

            return feed;
        }
    }

    /// <summary>
    /// A miscellaneous container for activities
    /// </summary>
    public class ActivityFolder : ActivityNode
    {
        public ActivityFolder(Node parent) : base(parent)
        {  }
    }

    /// <summary>
    /// Models the frequency with which an activity is performed
    /// </summary>
    public class ActivityTimerInterval : Node
    {
        public int Interval { get; set; } = 12;

        public int MonthDue { get; set; } = 12;

        public ActivityTimerInterval(Node parent) : base(parent)
        {  }
    }

    /// <summary>
    /// Models the relationship between two variables
    /// </summary>
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
