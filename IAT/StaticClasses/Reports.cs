using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace IAT
{
    public class Reports
    {
        /// <summary>
        /// Creates the reports folder for a CLEM simulation
        /// </summary>
        public static XElement GetReports(XElement resources)
        {
            // Skip first and last elements (they contain metadata, not actual resource data)
            var r = resources.Elements();
            var input = r.Skip(1).Take(r.Count() - 2);

            XElement reports = new XElement
            (
                "CLEMFolder",
                new XElement("Name", "Reports"),
                GetBalances(),
                GetPerformed(),
                GetShortfalls(),
                GetLedgers(input)
            );

            return reports;
        }

        /// <summary>
        /// Returns the herd summary report
        /// </summary>
        public static XElement GetHerdSummary()
        {
            XElement summary = new XElement("SummariseRuminantHerd");
            summary.Add(new XElement("Name", "SummariseHerd"));
            summary.Add(new XElement("IncludeInDocumentation", "true"));
            return summary;
        }

        /// <summary>
        /// Returns the herd report
        /// </summary>
        public static XElement GetHerdReport()
        {
            XElement report = new XElement("ReportRuminantHerd");
            report.Add(new XElement("Name", "ReportHerd"));
            report.Add(new XElement("IncludeInDocumentation", "true"));
            return report;
        }

        /// <summary>
        /// Returns the resource ledgers reports
        /// </summary>
        private static IEnumerable<XElement> GetLedgers(IEnumerable<XElement> resources)
        {
            XElement ledgers = new XElement("Ledgers");

            foreach (XElement resource in resources)
            {
                XElement ledger = new XElement("ReportResourceLedger");
                ledger.Add(new XElement("Name", resource.Name.LocalName));
                ledger.Add(new XElement("IncludeInDocumentation", "true"));
                ledger.Add(new XElement("ExperimentFactorNames"));
                ledger.Add(new XElement("ExperimentFactorValues"));

                XElement variables = new XElement("VariableNames");
                variables.Add(new XElement("string", resource.Name));
                ledger.Add(variables);

                ledgers.Add(ledger);
            }

            return ledgers.Elements();
        }

        /// <summary>
        /// Returns the resource balances report
        /// </summary>
        private static XElement GetBalances()
        {
            XElement balances = new XElement("ReportResourceBalances");
            balances.Add(new XElement("Name", "ReportResourceBalances"));
            balances.Add(new XElement("IncludeInDocumentation", "true"));
            balances.Add(new XElement("ExperimentFactorNames"));
            balances.Add(new XElement("ExperimentFactorValues"));

            XElement variables = new XElement("VariableNames");
            variables.Add(new XElement("string", "[Clock].Today"));
            variables.Add(new XElement("string", "AnimalFoodStore"));
            balances.Add(variables);

            XElement events = new XElement("EventNames");
            events.Add(new XElement("string", "[Clock.CLEMEndOfTimeStep"));
            balances.Add(events);

            return balances;
        }

        /// <summary>
        /// Returns the activities performed report
        /// </summary>
        private static XElement GetPerformed()
        {
            XElement performed = new XElement("ReportActivitiesPerformed");
            performed.Add(new XElement("Name", "ReportActivitiesPerformed"));
            performed.Add(new XElement("IncludeInDocumentation", "true"));
            performed.Add(new XElement("ExperimentFactorNames"));
            performed.Add(new XElement("ExperimentFactorValues"));

            return performed;
        }

        /// <summary>
        /// Returns the resource shortfalls report
        /// </summary>
        private static XElement GetShortfalls()
        {
            XElement shortfalls = new XElement("ReportResourceShortfalls");
            shortfalls.Add(new XElement("Name", "ReportResourceShortfalls"));
            shortfalls.Add(new XElement("IncludeInDocumentation", "true"));
            shortfalls.Add(new XElement("ExperimentFactorNames"));
            shortfalls.Add(new XElement("ExperimentFactorValues"));

            return shortfalls;
        }
    }
}
