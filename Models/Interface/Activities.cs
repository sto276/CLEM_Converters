using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interface
{
    using CLEM.Activities;
    public partial interface IApsimX
    {
        FinanceActivityPayExpense GetMonthlyExpenses(ActivityFolder cashflow);

        IEnumerable<FinanceActivityPayExpense> GetAnnualExpenses(ActivityFolder cashflow);

        FinanceActivityCalculateInterest GetInterestRates(ActivityFolder cashflow);

        ActivityFolder GetManageBreeds(ActivityFolder parent);
    }
}
