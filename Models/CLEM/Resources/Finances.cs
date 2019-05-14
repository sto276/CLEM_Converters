namespace Models.CLEM.Resources
{
    /// <summary>
    /// Container for financial resources
    /// </summary>
    public class Finance : Node
    {
        public string CurrencyName { get; set; }

        public Finance(ResourcesHolder parent) : base(parent)
        {
            Name = "Finance";
            Source.SetFinanceData(this);
            Add(new FinanceType(this));
        }
    }

    /// <summary>
    /// Models a generic financial resource type
    /// </summary>
    public class FinanceType : Node
    {
        public double OpeningBalance { get; set; }

        public bool EnforceWithdrawalLimt { get; set; }

        public double WithdrawalLimit { get; set; } = 0.0;

        public double InterestRateCharged { get; set; } = 0.0;

        public double InterestRatePaid { get; set; } = 0.0;

        public string Units { get; set; }

        public FinanceType(Finance parent) : base(parent)
        {
            Source.SetBankData(this);
        }
    }
}
