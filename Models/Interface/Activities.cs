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
        void GetMonthlyExpenses(ActivityFolder parent);

        void GetAnnualExpenses(ActivityFolder parent);

        void GetManageRuminants(ActivityFolder parent);
    }
}
