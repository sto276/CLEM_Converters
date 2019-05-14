namespace Models.CLEM.Activities
{
    /// <summary>
    /// Models the payment of an arbitrary expenditure
    /// </summary>
    public class FinanceActivityPayExpense : Node
    {
        public double Amount { get; set; } = 1;

        public string AccountName { get; set; } = "Finances.Bank";

        public bool IsOverhead { get; set; } = false;

        public int OnPartialresourcesAvailableAction { get; set; } = 0;

        public FinanceActivityPayExpense(Node parent) : base(parent)
        { }
    }
    
    /// <summary>
    /// Models the calculation of interest
    /// </summary>
    public class FinanceActivityCalculateInterest : Node
    {
        public FinanceActivityCalculateInterest(Node parent) : base(parent)
        {
            Name = "CalculateInterest";
        }
    }
}
