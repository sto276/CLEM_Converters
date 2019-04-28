using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Reader
{
    using static Queries;
    class Simulation
    {
        public static void Create(string path)
        {
            XElement nabsa = XElement.Load(path);

            Forages.FileForage(nabsa);

            // Prepares the datastore
            XElement datastore = new XElement
            (
                "DataStore",
                new XElement("Name", "DataStore"),
                new XElement("IncludeInDocumentation", "true")
            );

            // Prepares the XML structure
            XNamespace xns = "http://www.w3.org/2001/XMLSchema-instance";
            XAttribute xsi = new XAttribute(XNamespace.Xmlns + "xsi", xns);
            XAttribute version = new XAttribute("Version", "28");

            // Create the root element
            XElement apsimx = new XElement
            (
                "Simulations", 
                xsi, 
                version,
                new XElement("Name", "Simulations"),         
                datastore,
                GetSimulation(nabsa),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("ExplorerWidth", "300")
            );

            // Writes the finished document
            string filename = Path.GetFileNameWithoutExtension(path);
            Write(apsimx, filename);
        }

        private static XElement GetSimulation(XElement nabsa)
        {
            XElement summary = new XElement
            (
                "Summary",
                new XElement("Name", "summaryfile"),
                new XElement("IncludeInDocumentation", "true")
            );

            XElement simulation = new XElement
            (
                "Simulation",
                new XElement("Name", nabsa.Name),
                summary,
                GetClock(nabsa),
                GetForagesFile(nabsa),
                GetGRASP(nabsa.Name.LocalName),
                GetCLEM(nabsa),
                new XElement("IncludeInDocumentation", "true")
            );

            return simulation;
        }

        /// <summary>
        /// Writes the date and time information for the simulation
        /// </summary>
        /// <param name="nab">Source NABSA</param>
        private static XElement GetClock(XElement nabsa)
        {
            XElement param = nabsa.Element("SingleParams");
            int year = Convert.ToInt32(param.Element("Start_year").Value);
            int first = Convert.ToInt32(param.Element("Start_month").Value);
            int last = Convert.ToInt32(param.Element("End_month").Value);

            DateTime start = new DateTime(year, first, 1);
            DateTime end = start.AddMonths(last);

            //
            XElement clock = new XElement
            (
                "Clock",
                new XElement("Name", "Clock"),
                new XElement("IncludeInDocumentation", "true"),           
                new XElement("StartDate", start.ToString("s")),
                new XElement("EndDate", end.ToString("s"))
            );

            return clock;
        }

        /// <summary>
        /// Finds the forage file and writes it to the simulation
        /// </summary>
        /// <param name="nab">Source NABSA</param>
        private static XElement GetForagesFile(XElement nabsa)
        {
            // Find the path to the forages file
            XElement forages = FindByName(nabsa, "Forages File");
            string path = FindFirst(forages, "string").Value;
            string prn = Path.ChangeExtension(path, "prn");

            XElement file = new XElement
            (
                "FileCrop",
                new XElement("Name", "FileForage"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("FileName", prn)
            );

            return file;
        }

        /// <summary>
        /// Writes the GRASP file information to the simulation
        /// </summary>
        /// <param name="nab"></param>
        private static XElement GetGRASP(string name)
        {
            XElement grasp = new XElement
            (
                "FileSQLiteGRASP",
                new XElement("Name", "FileGrasp"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("FileName", $"{name}.db")
            );

            return grasp;
        }

        /// <summary>
        /// Writes the 'ZoneCLEM' section of the simulation
        /// </summary>
        /// <param name="nab">Source NABSA</param>
        private static XElement GetCLEM(XElement nabsa)
        {
            XElement resources = GetResources(nabsa);
            XElement activities = GetActivities(nabsa, resources);

            XElement clem = new XElement
            (
                "ZoneCLEM",
                new XElement("Name", "CLEM"),
                resources,
                activities,
                new XElement("IncludeInDocumentation", "true"),
                new XElement("Area", "1"),
                new XElement("Slope", "0"),
                new XElement("ClimateRegion", "0"),
                new XElement("EcologicalIndicatorsCalculationMonth", "12")
            );

            return clem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        public static XElement GetResources(XElement nabsa)
        {
            XElement resources = new XElement
            (
                "ResourcesHolder",
                new XElement("Name", "Resources"),
                Land.GetLand(nabsa),
                Labour.GetLabour(nabsa),
                Foodstores.GetGraze(nabsa),
                Foodstores.GetAnimal(nabsa),
                Ruminants.GetRuminants(nabsa),
                Finances.GetFinance(nabsa),
                new XElement("IncludeInDocumentation", "true")
            );

            return resources;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        public static XElement GetActivities(XElement nabsa, XElement resources)
        {
            XElement activities = new XElement
            (
                "ActivitiesHolder",
                new XElement("Name", "Activities"),
                Finances.GetCashFlow(nabsa),
                Land.GetPastureManage(nabsa),
                Ruminants.Manage(nabsa, resources),
                new XElement("IncludeInDocumentation", "true")
            );
            return activities;
        }

        /// <summary>
        /// Writes out the XML object to the file with the given name
        /// </summary>
        /// <param name="apsimx"></param>
        /// <param name="filename"></param>
        private static void Write(XElement apsimx, string filename)
        {
            FileStream fs = new FileStream($"{filename}.apsimx", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            XmlTextWriter xtw = new XmlTextWriter(sw)
            {
                Formatting = Formatting.Indented,
                Indentation = 4
            };

            xtw.WriteStartDocument();
            apsimx.WriteTo(xtw);
            xtw.WriteEndDocument();
            xtw.Close();
        }
    }
}

