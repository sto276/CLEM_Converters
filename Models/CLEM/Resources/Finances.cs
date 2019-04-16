using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    public class Finance : Node
    {
        public string CurrencyName { get; set; }

        public Finance(ResourcesHolder parent) : base(parent)
        {
            Name = "Finance";
            new FinanceType(this);
        }
    }

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
            Source.SetFinanceData(this);
        }
    }
}
