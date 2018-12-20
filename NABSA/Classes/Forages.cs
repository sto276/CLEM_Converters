using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NABSA
{
    class Forages
    {
        /// <summary>
        /// Accesses the forage inputs of an IAT file and moves the data to an independent PRN file
        /// </summary>
        /// <param name="crop">The worksheet object containing forage inputs</param>
        /// <param name="name">The name of the output file</param>
        public static void FileForage(XElement nabsa)
        {
            // Find the file inputs
            var inputs = new XElement("Inputs", nabsa.Elements("Input"));

            // Find the forages file from the inputs
            var forages = Queries.FindByName(inputs, "Forages File");

            // Find all the saved file names
            var files = Queries.FindFirst(forages, "FileNames").Elements();

            foreach (var file in files)
            {
                // Path to the .csv file
                string path = file.Value;
                string filename = Path.GetFileNameWithoutExtension(path);

                // Input file
                FileStream csv = null;
                StreamReader reader = null;

                try
                {
                    csv = new FileStream(path, FileMode.Open);
                    reader = new StreamReader(csv);
                }
                catch (IOException)
                {
                    Console.WriteLine($"{filename}.prn could not be written.");
                    continue;
                }
                // Need to add additional error handling here

                // Output file                
                FileStream prn = new FileStream($"Simulations/{filename}.prn", FileMode.Create);
                StreamWriter writer = new StreamWriter(prn);

                CSVtoPRN(nabsa, reader, writer);
            }               
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        private static void CSVtoPRN(XElement nabsa, StreamReader reader, StreamWriter writer)
        {
            // Add header to document
            writer.WriteLine($"{"SoilNum",-36}{"CropName",-36}{"YEAR",-36}{"Month",-36}{"AmtKg",-36}");
            writer.WriteLine($"{"()",-36}{"()",-36}{"()",-36}{"()",-36}{"()",-36}");

            // Find the climate region
            string climate = Queries.FindFirst(nabsa, "ClimRegion").Value;

            // Find the list of grown forages
            XElement specs = Queries.FindByName(nabsa, "Forage Crop Specs - General");
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
    }
}
