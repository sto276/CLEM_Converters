using Models.CLEM.Activities;
using Models.CLEM.Resources;
using System;

namespace Reader
{
    public partial class NABSA
    {
        public void SetFinanceData(Finance finance)
        {
            finance.CurrencyName = "AU$";
        }

        public void SetBankData(FinanceType bank)
        {
            // Find the balance and interest
            double balance = Convert.ToDouble(FindFirst(Source, "Cash_balance").Value);
            double interest = Convert.ToDouble(FindFirst(Source, "Int_rate").Value);

            bank.OpeningBalance = balance;
            bank.InterestRateCharged = interest;
        }            

        public ActivityFolder GetAnnualExpenses(ActivityFolder cashflow)
        {
            ActivityFolder annual = new ActivityFolder(cashflow)
            {
                Name = "AnnualExpenses"
            };

            // Names of parameters to search for in the Element
            string[] items = new string[]
            {
            "Farm_maint",
            "Mach_maint",
            "Fuel",
            "Pests",
            "Contractors",
            "Admin",
            "Rates",
            "Insurance",
            "Electricity",
            "Water",
            "Other_costs"
            };            

            foreach (string item in items)
            {
                string value = FindFirst(SingleParams, item).Value;
                int.TryParse(value, out int amount);

                // Only need to add the element if its a non-zero expenditure
                if (amount <= 0) continue; 
                
                FinanceActivityPayExpense expense = new FinanceActivityPayExpense(annual)
                {                        
                    Name = item.Replace("_", ""),
                    Amount = amount,
                    AccountName = "Finances.Bank",
                    IsOverhead = false
                };
                annual.Add(expense);                
            }

            return annual;
        }

        public FinanceActivityCalculateInterest GetInterestRates(ActivityFolder cashflow)
        {
            // Find the interest amount
            string value = FindFirst(SingleParams, "Int_rate").Value;
            int.TryParse(value, out int rate);

            // If the interest is 0, don't add the element
            if (rate == 0) return null;
            
            return new FinanceActivityCalculateInterest(cashflow);
        }

        public ActivityFolder GetMonthlyExpenses(ActivityFolder cashflow)
        {
            ActivityFolder monthly = new ActivityFolder(cashflow)
            {
                Name = "MonthlyExpenses"
            };

            string value = FindFirst(SingleParams, "Living_cost").Value;
            double.TryParse(value, out double amount);

            monthly.Add(new FinanceActivityPayExpense(monthly)
            {
                Name = "LivingCost",
                Amount = amount,
                AccountName = "Finance.Bank"
            });

            return monthly;
        }

    }
   
}