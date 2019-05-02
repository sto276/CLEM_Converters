using Models.CLEM;
using Models.Core;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Reader
{
    using static Queries;
    public partial class NABSA
    {
        

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
                GetForagesFile(nabsa),

                new XElement("IncludeInDocumentation", "true")
            );

            return simulation;
        }

        /// <summary>
        /// Finds the forage file and writes it to the simulation
        /// </summary>
        /// <param name="nab">Source NABSA</param>
        private static XElement GetForagesFile(XElement nabsa)
        {
            // Find the path to the forages file
            XElement forages = FindByNameTag(nabsa, "Forages File");
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


        public FileSQLiteGrasp GetFiles(Simulation simulation)
        {
            return new FileSQLiteGrasp(simulation)
            {
                Name = "FileGrasp",
                FileName = Source.Name.LocalName + ".db"
            };
        }

    }
}

