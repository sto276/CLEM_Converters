namespace IAT
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    class Finances
    {
        /// <summary>
        /// Creates the 'Bank' XML structure for a CLEM simulation (subset of 'Finance')
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetBank(IAT iat)
        {   
            // Load the table containing financial data
            IATable overheads = iat.tables["Overheads"];

            XElement finance = new XElement("Finance");
            finance.Add(new XElement("Name", "Finances"));

            // Find what currency is being used
            int id = Convert.ToInt32(iat.GetCellValue(iat.part, 4, 9));
            finance.Add(new XElement("CurrencyName", $"{iat.GetCellValue(iat.part, 4 + id, 8)}"));

            // Find the initial bank balance and interest
            XElement type = new XElement("FinanceType");
            type.Add(new XElement("Name", "Bank"));
            type.Add(new XElement("IncludeInDocumentation", "true"));
            type.Add(new XElement("OpeningBalance", overheads.GetData<string>(12, 0)));
            type.Add(new XElement("EnforceWithdrawalLimit", "false"));
            type.Add(new XElement("WithdrawalLimit", "0"));
            type.Add(new XElement("InterestRateCharged", overheads.GetData<string>(11, 0)));
            type.Add(new XElement("InterestRatePaid", "0"));

            finance.Add(type);
            finance.Add(new XElement("IncludeInDocumentation", "true"));

            return finance;
        }

        /// <summary>
        /// Create the cashflow model
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement CashFlow(IAT iat)
        {
            XElement cashflow = new XElement("ActivityFolder");
            cashflow.Add(new XElement("Name", "CashFlow"));
            cashflow.Add(GetMonthly(iat));
            cashflow.Add(GetAnnual(iat));
            cashflow.Add(CalculateInterest(iat));

            return cashflow;
        }

        /// <summary>
        /// Finds monthly expenses, represented as XML
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetMonthly(IAT iat)
        {           
            XElement expenses = new XElement("ActivityFolder");
            expenses.Add(new XElement("Name", "ExpensesMonthly"));        
            expenses.Add(GetLivingCost(iat));

            return expenses;
        }

        /// <summary>
        /// Finds the monthly living cost, represented as XML
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetLivingCost(IAT iat)
        {
            IATable overheads = iat.tables["Overheads"];

            XElement cost = new XElement("FinanceActivityPayExpense");
            cost.Add(new XElement("Name", "LivingCost"));
            cost.Add(new XElement("IncludeInDocumentation", "true"));
            cost.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));
            cost.Add(new XElement("Amount", $"{overheads.GetData<string>(13, 0)}"));
            cost.Add(new XElement("AccountName", "Finances.Bank"));
            cost.Add(new XElement("IsOverhead", "false"));

            return cost;
        }

        /// <summary>
        /// Finds annual expenses, represented as XML
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetAnnual(IAT iat)
        {
            IATable overheads = iat.tables["Overheads"];

            XElement expenses = new XElement("ActivityFolder");
            expenses.Add(new XElement("Name", "ExpensesAnnual"));

            // Find row names and the amount data
            var rows = overheads.GetRowNames();
            var amounts = overheads.GetColData<double>(0);

            double amount = 0;
            foreach (string row in rows)
            {
                // Only items prior to Int_rate are annual expenses
                if (row == "Int_rate") break;

                // Find the upkeep amount
                int index = rows.FindIndex(s => s == row);
                amount = amounts.ElementAt(index);

                // Only need to add the element if its a non-zero expenditure
                if (amount > 0)
                {
                    XElement expense = new XElement("FinanceActivityPayExpense");
                    expense.Add(new XElement("Name", $"{row.Replace("_", "")}"));
                    expense.Add(new XElement("IncludeInDocumentation", "true"));
                    expense.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));
                    expense.Add(new XElement("Amount", $"{amount}"));
                    expense.Add(new XElement("AccountName", "Finances.Bank"));
                    expense.Add(new XElement("IsOverhead", "false"));

                    expenses.Add(expense);
                }
            }

            return expenses;
        }

        /// <summary>
        /// Finds the annual interest, represented as XML
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement CalculateInterest(IAT iat)
        {
            // Find the interest amount
            IATable overheads = iat.tables["Overheads"];
            int row = overheads.GetRowNames().FindIndex(s => s == "Int_rate");

            // If the interest is 0, don't add the element
            if (overheads.GetData<int>(row, 0) == 0) return null;

            XElement interest = new XElement("FinanceActivityCalculateInterest");
            interest.Add(new XElement("Name", "FinanceActivityCalculateInterest"));
            interest.Add(new XElement("IncludeInDocumentation", "true"));
            interest.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));

            return interest;
        }
    }
}
