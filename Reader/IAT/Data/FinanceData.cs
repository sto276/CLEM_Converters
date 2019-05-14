using Models.CLEM.Activities;
using Models.CLEM.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reader
{
    // Implements all methods related to IAT finance
    public partial class IAT
    {
        /// <summary>
        /// Sets the currency data for a Finance model using IAT data
        /// </summary>
        /// <param name="finance">The base model</param>
        public void SetFinanceData(Finance finance)
        {
            int id = Convert.ToInt32(GetCellValue(Part, 4, 9));
            finance.CurrencyName = GetCellValue(Part, 4 + id, 8);
        }

        /// <summary>
        /// Sets the data for a FinanceType model using IAT data
        /// </summary>
        /// <param name="bank">The base model</param>
        public void SetBankData(FinanceType bank)
        {
            bank.Name = "Bank";
            bank.OpeningBalance = Overheads.GetData<double>(12, 0);
            bank.InterestRateCharged = Overheads.GetData<double>(11, 0);
        }

        /// <summary>
        /// Gets the monthly living expenses from the IAT data
        /// </summary>
        /// <param name="cashflow">The model to attach the data to</param>
        public ActivityFolder GetMonthlyExpenses(ActivityFolder cashflow)
        {
            var monthly = new ActivityFolder(cashflow)
            {
                Name = "MonthlyExpenses"
            };

            // Find the monthly living cost
            double amount = Overheads.GetData<double>(13, 0);

            // Only include if non-zero
            if (amount == 0) return null;

            monthly.Add(new FinanceActivityPayExpense(monthly)
            {
                Name = "LivingCost",
                Amount = amount
            });

            return monthly;
        }

        /// <summary>
        /// Return a collection of the annual expenses from the IAT data
        /// </summary>
        /// <param name="cashflow">The base model</param>
        public ActivityFolder GetAnnualExpenses(ActivityFolder cashflow)
        {
            var annual = new ActivityFolder(cashflow)
            {
                Name = "AnnualExpenses"
            };

            // Names of the expenses
            var rows = Overheads.RowNames;

            // Amounts of the expenses
            var amounts = Overheads.GetColData<double>(0);

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
                    annual.Add(new FinanceActivityPayExpense(annual)
                    {
                        Name = row.Replace("_", ""),
                        Amount = amount
                    });
                }
            }
            // If there are no annual expenses, ignore this folder
            if (annual.Children.Count == 0) return null;
            return annual;
        }

        /// <summary>
        /// Return the interest rate calculation model
        /// </summary>
        /// <param name="cashflow">The base model</param>
        public FinanceActivityCalculateInterest GetInterestRates(ActivityFolder cashflow)
        {           
            // Find the interest amount
            int row = Overheads.RowNames.FindIndex(s => s == "Int_rate");

            // If the interest is 0, ignore the model
            if (Overheads.GetData<int>(row, 0) == 0) return null;

            return new FinanceActivityCalculateInterest(cashflow);
        }

    }
}
