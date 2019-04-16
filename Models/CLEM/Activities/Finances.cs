using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Activities
{
    class FinanceActivityPayExpense : Node
    {
        public double Amount { get; set; } = 0.0;

        public string AccountName { get; set; } = "Finances.Bank";

        public bool IsOverhead { get; set; } = false;

        public int OnPartialresourcesAvailableAction = 0;

        public FinanceActivityPayExpense(Node parent) : base(parent)
        {

        }
    }
    
}
