using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Reader
{
    using static Queries;
    class Finances
    {
        /// <summary>
        /// Writes the 'Finance' subsection
        /// </summary>
        /// <param name="nab"></param>
        public static XElement GetFinance(XElement nabsa)
        {
            // Find the balance and interest
            string balance = FindFirst(nabsa, "Cash_balance").Value;
            string interest = FindFirst(nabsa, "Int_rate").Value;
           
            // Write the bank data XML
            XElement bank = new XElement(
                "FinanceType",
                new XElement("Name", "Bank"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OpeningBalance", balance),
                new XElement("EnforceWithdrawalLimit", "false"),
                new XElement("WithdrawalLimit", "0"),
                new XElement("InterestRateCharged", interest),
                new XElement("InterestRatePaid", "0")
                );

            // Add the bank data to the finance 
            XElement finance = new XElement(
                "Finance",
                new XElement("Name", "Finances"),
                bank,
                new XElement("IncludeInDocumentation", "true")
                );

            return finance;
        }

        /// <summary>
        /// Get an XML representation of the CashFlow activity
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetCashFlow(XElement nabsa)
        {
            XElement singleparams = FindFirst(nabsa, "SingleParams");

            XElement cashflow = new XElement
            (
                "ActivityFolder",
                new XElement("Name", "CashFlow"),
                GetMonthly(singleparams),
                GetAnnual(singleparams),
                GetCalculateInterest(singleparams)
            );

            return cashflow;
        }

        /// <summary>
        /// Finds monthly expenses, represented as XML
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetMonthly(XElement singleparams)
        {
            XElement expenses = new XElement
            (
                "ActivityFolder",
                new XElement("Name", "ExpensesMonthly"),
                GetLivingCost(singleparams)
            );

            return expenses;
        }

        /// <summary>
        /// Finds the monthly living cost, represented as XML
        /// </summary>
        /// <param name="iat">Source IAT</param>
        private static XElement GetLivingCost(XElement singleparams)
        {
            string value = FindFirst(singleparams, "Living_cost").Value;
            int.TryParse(value, out int amount);

            XElement cost = new XElement
            (
                "FinanceActivityPayExpense",
                new XElement("Name", "LivingCost"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("Amount", amount),
                new XElement("AccountName", "Finances.Bank"),
                new XElement("IsOverhead", "false")
            );

            return cost;
        }

        /// <summary>
        /// Finds annual expenses, represented as XML
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetAnnual(XElement singleparams)
        {           
            XElement annual = new XElement
            (
                "ActivityFolder",
                new XElement("Name", "ExpensesAnnual"),
                GetExpenses(singleparams)
            );           

            return annual;
        }

        /// <summary>
        /// Gets a collection of expenses in XML representation
        /// </summary>
        /// <param name="singleparams"></param>
        private static IEnumerable<XElement> GetExpenses(XElement singleparams)
        {
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

            XElement expenses = new XElement("Expenses");
            foreach (string item in items)
            {
                string value = FindFirst(singleparams, item).Value;
                int.TryParse(value, out int amount);

                // Only need to add the element if its a non-zero expenditure
                if (amount > 0)
                {
                    XElement expense = new XElement
                    (
                        "FinanceActivityPayExpense",
                        new XElement("Name", $"{item.Replace("_", "")}"),
                        new XElement("IncludeInDocumentation", "true"),
                        new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                        new XElement("Amount", $"{amount}"),
                        new XElement("AccountName", "Finances.Bank"),
                        new XElement("IsOverhead", "false")
                    );
                    expenses.Add(expense);
                }
            }

            return expenses.Elements();
        }

        /// <summary>
        /// Gets an XML representation of the Calculate Interest activity
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetCalculateInterest(XElement singleparams)
        {
            // Find the interest amount
            string value = FindFirst(singleparams, "Int_rate").Value;
            int.TryParse(value, out int rate);

            // If the interest is 0, don't add the element
            if (rate == 0) return null;

            XElement interest = new XElement
            (
                "FinanceActivityCalculateInterest",
                new XElement("Name", "FinanceActivityCalculateInterest"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop")
            );

            return interest;
        }
    }
}
