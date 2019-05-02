using Models;
using Models.CLEM;
using Models.Core;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Reader
{
    public partial class NABSA
    {
        public static void Run(IEnumerable<string> files)
        {
            
        }

        public Clock GetClock(Simulation simulation)
        {
            int year = Convert.ToInt32(SingleParams.Element("Start_year").Value);
            int first = Convert.ToInt32(SingleParams.Element("Start_month").Value);
            int last = Convert.ToInt32(SingleParams.Element("End_month").Value);

            DateTime start = new DateTime(year, first, 1);
            DateTime end = start.AddMonths(last);

            return new Clock(simulation)
            {
                StartDate = start,
                EndDate = end
            };
        }

        public IEnumerable<Node> GetFiles(ZoneCLEM clem)
        {
            List<Node> files = new List<Node>();

            files.Add(new FileSQLiteGrasp(clem)
            {
                Name = "FileGrasp",
                FileName = Source.Name.LocalName + ".db"
            });

            XElement forages = FindByNameTag(Source, "Forages File");
            string path = FindFirst(forages, "string").Value;

            CSVtoPRN(path);

            files.Add(new FileCrop(clem)
            {
                Name = "FileForage",
                FileName = Path.ChangeExtension(path, "prn")
            });

            return files;
        }

        private void CSVtoPRN(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);

            FileStream csv = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(csv);

            FileStream prn = new FileStream($"Simulations/{filename}.prn", FileMode.Create);
            StreamWriter writer = new StreamWriter(prn);

            // Add header to document
            writer.WriteLine($"{"SoilNum",-36}{"CropName",-36}{"YEAR",-36}{"Month",-36}{"AmtKg",-36}");
            writer.WriteLine($"{"()",-36}{"()",-36}{"()",-36}{"()",-36}{"()",-36}");

            // Find the climate region
            string climate = FindFirst(Source, "ClimRegion").Value;

            // Find the list of grown forages
            XElement specs = FindByNameTag(Source, "Forage Crop Specs - General");
            var crops = specs.Elements().Skip(2);

            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                // data[0]: Climate region
                // data[1]: Soil number
                // data[2]: Forage number
                // data[7]: Year
                // data[9]: Month
                // data[10]: ?Growth amount?
                string[] data = line.Split(',');

                // Check the region matches
                if (data[0] != climate) continue;

                // Find the name of the forage from the number
                int.TryParse(data[2], out int num);
                string forage = crops.ElementAt(num).Element("string").Value;

                if (data[10] == "") data[10] = "0";
                writer.WriteLine($"{data[1],-36}{forage,-36}{data[7],-36}{data[9],-36}{data[10],-36}");
            }
            reader.Close();
            writer.Close();
        }

        private T GetValue<T>(XElement xml, int index)
        {
            string value = xml.Elements().ElementAt(index).Value;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        private T GetValue<T>(XElement xml, string name)
        {
            string value = xml.Element(name).Value;
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
