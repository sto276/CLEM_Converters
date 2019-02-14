using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace IAT
{   
    /// <summary>
    /// Methods for building the structure of a CLEM simulation
    /// </summary>
    class Simulation
    {
        /// <summary>
        /// Gets the XML representation of an .apsimx
        /// </summary>
        public static XElement GetApsimx(XElement simulations)
        {           
            XNamespace xns = "http://www.w3.org/2001/XMLSchema-instance";
            XAttribute attribute = new XAttribute(XNamespace.Xmlns + "xsi", xns);
            XAttribute version = new XAttribute("Version", "28");

            XElement apsimx = new XElement(
                "Simulations", 
                attribute, 
                version,
                new XElement("Name", "Simulations"),
                GetDataStore(),
                simulations.Elements(),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("ExplorerWidth", "300")
            );

            return apsimx;            
        }

        /// <summary>
        /// Gets the XML representation of a single simulation
        /// </summary>
        /// <param name="iat"></param>
        public static XElement GetSimulation(IAT iat, XElement clem)
        {
            XElement simulation = new XElement(
                "Simulation",
                new XElement("Name", iat.sheet.Name),
                GetClock(iat),
                GetSummary(),                
                clem,
                new XElement("IncludeInDocumentation", "true")
            );

            return simulation;
        }

        /// <summary>
        /// Creates the XML structure for a CLEM simulation in an ApsimX model
        /// </summary>
        /// <param name="iat"></param>
        /// <returns>XML structure of a CLEM simulation</returns>
        public static XElement GetCLEM(IAT iat, XElement prns)
        {            
            XElement resources = iat.GetResources();
            XElement activities = iat.GetActivities();
            XElement reports = Reports.GetReports(resources);

            XElement clem = new XElement(
                "ZoneCLEM",
                new XElement("Name", "CLEM"),
                prns.Elements(),
                resources,
                activities,
                reports,
                new XElement("IncludeInDocumentation", "true"),
                new XElement("Area", "1"),
                new XElement("Slope", "0"),
                new XElement("ClimateRegion", "0"),
                new XElement("EcologicalIndicatorsCalculationMonth", "12")
            );

            return clem;
        }

        /// <summary>
        /// Returns the datastore component of a simulation
        /// </summary>
        private static XElement GetDataStore()
        {
            XElement datastore = new XElement("DataStore");
            datastore.Add(new XElement("Name", "DataStore"));
            datastore.Add(new XElement("IncludeInDocumentation", "true"));
            return datastore;
        }

        /// <summary>
        /// Searches the IAT for data on the time frame of the model
        /// </summary>
        /// <param name="iat">Source IAT</param>
        /// <returns>XML container for a simulation clock</returns>
        private static XElement GetClock(IAT iat)
        {
            XElement clock = new XElement("Clock");

            int start_year = Convert.ToInt32(iat.GetCellValue(iat.part, 44, 4));
            int start_month = Convert.ToInt32(iat.GetCellValue(iat.part, 45, 4));
            int run_time = Convert.ToInt32(iat.GetCellValue(iat.part, 46, 4));

            DateTime start = new DateTime(start_year, start_month, 1, 0, 0, 0, 0);
            DateTime end = start.AddYears(run_time);

            clock.Add(new XElement("Name", "Clock"));
            clock.Add(new XElement("IncludeInDocumentation", "true"));
            clock.Add(new XElement("StartDate", start.ToString("s")));
            clock.Add(new XElement("EndDate", end.ToString("s")));

            return clock;
        }

        /// <summary>
        /// Creates the summary file XML object for an ApsimX model
        /// </summary>
        /// <returns>XML container for simulation summary file</returns>
        private static XElement GetSummary()
        {
            XElement summary = new XElement("Summary");
            summary.Add(new XElement("Name", "summaryfile"));
            summary.Add(new XElement("IncludeInDocumentation", "true"));
            return summary;
        }

        /// <summary>
        /// Creates the XML structures for prn files in an ApsimX model
        /// </summary>
        /// <param name="iat">Source IAT</param>
        /// <returns>Array of prn file XML structures</returns>
        public static XElement GetFiles(IAT iat, string path)
        {
            XElement files = new XElement("Files");

            List<string> items = new List<string>()
            {
                "FileCrop",
                "FileCropResidue",
                "FileForage"
            };

            foreach(string item in items)
            {
                XElement file = new XElement(
                    "FileCrop",
                    new XElement("Name", item),
                    new XElement("IncludeInDocumentation", "true"),
                    new XElement("FileName", $"{path}\\{iat.name}_{item}.prn")                    
                    );
                files.Add(file);
            }

            return files;
        }

        
    }
}
