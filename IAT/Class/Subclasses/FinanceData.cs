using Models.CLEM.Activities;
using Models.CLEM.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReadIAT
{
    public partial class IAT
    {
        /// <summary>
        /// Contains the Finance data from a source IAT file
        /// </summary>
        private static class FinanceData
        {
            /// <summary>
            /// IAT sub table containing overhead data
            /// </summary>
            public static SubTable Overheads { get; private set; }            

            /// <summary>
            /// Pseudo constructor
            /// </summary>
            public static void Construct(IAT source)
            {
                Overheads = new SubTable("Overheads", source);
            }            
        }

        /// <summary>
        /// Sets the currency data for a Finance model using the IAT data
        /// </summary>
        /// <param name="finance">The base model</param>
        public void SetFinanceData(Finance finance)
        {
            int id = Convert.ToInt32(GetCellValue(Part, 4, 9));
            finance.CurrencyName = GetCellValue(Part, 4 + id, 8);
        }

        /// <summary>
        /// Sets the data for a FinanceType model using the IAT data
        /// </summary>
        /// <param name="bank">The base model</param>
        public void SetBankData(FinanceType bank)
        {
            bank.Name = "Bank";
            bank.OpeningBalance = FinanceData.Overheads.GetData<double>(12, 0);
            bank.InterestRateCharged = FinanceData.Overheads.GetData<double>(11, 0);
        }

        /// <summary>
        /// Gets the monthly living expenses from the IAT data
        /// </summary>
        /// <param name="cashflow">The model to attach the data to</param>
        public FinanceActivityPayExpense GetMonthlyExpenses(ActivityFolder cashflow)
        {
            return new FinanceActivityPayExpense(cashflow)
            {
                Name = "LivingCost",
                Amount = FinanceData.Overheads.GetData<double>(13, 0)
            };
        }

        /// <summary>
        /// Return a collection of the annual expenses from the IAT data
        /// </summary>
        /// <param name="cashflow">The base model</param>
        public IEnumerable<FinanceActivityPayExpense> GetAnnualExpenses(ActivityFolder cashflow)
        {
            List<FinanceActivityPayExpense> expenses = new List<FinanceActivityPayExpense>();

            // Names of the expenses
            var rows = FinanceData.Overheads.RowNames;

            // Amounts of the expenses
            var amounts = FinanceData.Overheads.GetColData<double>(0);

            // Look at each row in the overheads table
            foreach (string row in rows)
            {
                // The overheads table contains more than just annual data,
                // so stop at the "Int_rate" row (table is ordered)
                if (row == "Int_rate") break;

                // Find the upkeep amount
                int index = rows.FindIndex(s => s == row);
                double amount = amounts.ElementAt(index);

                // Only need to add the element if its a non-zero expenditure
                if (amount > 0)
                {
                    expenses.Add(new FinanceActivityPayExpense(cashflow)
                    {
                        Name = row.Replace("_", ""),
                        Amount = amount
                    });
                }
            }

            return expenses;
        }

        /// <summary>
        /// Return the interest rate calculation model (if any)
        /// </summary>
        /// <param name="cashflow">The base model</param>
        public FinanceActivityCalculateInterest GetInterestRates(ActivityFolder cashflow)
        {
            // Find the interest amount
            int row = FinanceData.Overheads.RowNames.FindIndex(s => s == "Int_rate");

            // If the interest is 0, don't add the element
            if (FinanceData.Overheads.GetData<int>(row, 0) != 0) return new FinanceActivityCalculateInterest(cashflow);

            return null;
        }

    }
}
