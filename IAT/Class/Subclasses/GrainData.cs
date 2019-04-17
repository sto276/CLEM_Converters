using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace IATReader
{
    /// <summary>
    /// Provides methods for reading and converting
    /// the grain crop data contained within an IAT file
    /// </summary>
    class GrainData
    {
        /// <summary>
        /// Writes the 'Manage Crops' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="iat"></param>
        public static XElement GetManage(IAT iat)
        {            
            IATable grown = iat.tables["Grain Crops Grown"];
            IATable specs = iat.tables["Grain Crop Specifications"];
            IATable landspecs = iat.tables["Land specifications"];

            XElement manage = new XElement("ActivityFolder");
            manage.Add(new XElement("Name", $"Manage crops"));

            int[] ids = grown.GetRowData<int>(0).ToArray();
            foreach (int id in iat.grains)
            {
                // Find the name of the crop in the file
                Sheet sheet = iat.SearchSheets("crop_inputs");
                WorksheetPart inputs = (WorksheetPart)iat.book.GetPartById(sheet.Id);
                string inputname = "Unknown";
                int row = 1;

                IEnumerable<Row> rows = sheet.Elements<Row>();
                while(row < rows.Count())
                {
                    if (iat.GetCellValue(inputs, row, 3) == id.ToString())
                        inputname = iat.GetCellValue(inputs, row, 4);
                    row++;
                }

                // Find which column holds the data for a given crop ID
                int col = Array.IndexOf(ids, id);

                // Find land data
                int land = grown.GetData<int>(1, col);
                string landname = landspecs.GetRowNames()[land - 1];
                string area = grown.GetData<string>(2, col);

                // Find crop retention data
                double grainprop = 1.0 - grown.GetData<double>(5, col) / 100.0;
                double resprop = grown.GetData<double>(4, col) / 100.0;

                // Find the storage pool
                string cropname = specs.GetRowNames()[id + 1];
                string grainpool = iat.pools.Values.ToList().Find(s => s.Contains(cropname));

                // Construct XML 
                XElement crop = new XElement("CropActivityManageCrop");
                crop.Add(new XElement("Name", $"Manage " + cropname));
                crop.Add(GetGrain(grainprop, inputname, grainpool));
                crop.Add(GetResidue(resprop, inputname, grainpool));
                crop.Add(new XElement("IncludeInDocumentation", "true"));
                crop.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));                
                crop.Add(new XElement("LandItemNameToUse", "Land." + landname));
                crop.Add(new XElement("AreaRequested", area));
                crop.Add(new XElement("UseAreaAvailable", "false"));
                manage.Add(crop);
            }
            manage.Add(new XElement("IncludeInDocumentation", "true"));
            manage.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));

            // Will not be included in final XML if there are no crops
            if (manage.Elements().Count() > 3) return manage;
            else return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop">Proportion of the grain kept</param>
        /// <param name="name">Name of the crop in the file</param>
        /// <returns></returns>
        private static XElement GetGrain(double prop, string name, string poolname)
        {                       
            XElement grain = new XElement("CropActivityManageProduct");
            grain.Add(new XElement("Name", $"Manage grain"));
            grain.Add(new XElement("ModelNameFileCrop", "FileCrop"));
            grain.Add(new XElement("CropName", name));
            grain.Add(new XElement("ProportionKept", $"{prop}"));
            grain.Add(new XElement("StoreItemName", "HumanFoodStore." + poolname));
            grain.Add(new XElement("TreesPerHa", "0"));
            grain.Add(new XElement("UnitsToHaConverter", "0"));
            grain.Add(new XElement("IncludeInDocumentation", "true"));
            grain.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));

            return grain;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop">Proportion of the residue kepy</param>
        /// <param name="name">Name of the crop in the file</param>
        /// <returns></returns>
        private static XElement GetResidue(double prop, string name, string poolname)
        {
            XElement residue = new XElement("CropActivityManageProduct");
            residue.Add(new XElement("Name", "Manage residue"));
            residue.Add(new XElement("ModelNameFileCrop", "FileCropResidue"));
            residue.Add(new XElement("CropName", name));
            residue.Add(new XElement("ProportionKept", $"{prop}"));
            residue.Add(new XElement("StoreItemName", "AnimalFoodStore." + poolname));
            residue.Add(new XElement("TreesPerHa", "0"));
            residue.Add(new XElement("UnitsToHaConverter", "0"));
            residue.Add(new XElement("IncludeInDocumentation", "true"));
            residue.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));

            return residue;
        }

        /// <summary>
        /// Accesses the crop inputs of an IAT file and moves the crop data to an independent PRN file
        /// </summary>
        /// <param name="crop">The worksheet object containing crop inputs</param>
        /// <param name="name">The name of the output file</param>
        public static void FileCrop(IAT iat)
        {
            try
            {
                // Overwrite any exisiting PRN file
                FileStream stream = new FileStream($"{Toolbox.OutDir}/{iat.name}/{iat.name}_FileCrop.prn", FileMode.Create);
                StreamWriter swriter = new StreamWriter(stream);

                // Find the data set
                WorksheetPart crop = (WorksheetPart)iat.book.GetPartById(iat.SearchSheets("crop_inputs").Id);              
                var rows = crop.Worksheet.Descendants<Row>().Skip(1);                

                // Write file header to output stream
                swriter.WriteLine($"{"SoilNum",-35}{"CropName",-35}{"YEAR",-35}{"Month",-35}{"AmtKg",-35}");
                swriter.WriteLine($"{"()",-35}{"()",-35}{"()",-35}{"()",-35}()");

                // Iterate over data, writing to output stream
                string SoilNum, CropName, YEAR, Month, AmtKg;
                foreach (Row row in rows)
                {
                    var cells = row.Descendants<Cell>();
                    if (cells.First().InnerText != iat.climate) continue;

                    SoilNum = iat.ParseCell(cells.ElementAt(1));
                    CropName = iat.ParseCell(cells.ElementAt(3)).Replace(" ", "");
                    YEAR = iat.ParseCell(cells.ElementAt(5));
                    Month = iat.ParseCell(cells.ElementAt(6));
                    AmtKg = iat.ParseCell(cells.ElementAt(7));

                    // Writing row to file
                    if (AmtKg == "") AmtKg = "0";
                    swriter.WriteLine($"{SoilNum,-35}{CropName,-35}{YEAR,-35}{Month,-35}{AmtKg}");
                }
                swriter.Close();
            }
            catch (IOException)
            {
                // Will only be caught if the file exists and is in use by another program;
                // Alerts the user then safely continues with the program.
                Console.WriteLine("FileCrop.prn is open in another application and couldn't be overwritten.");
            }
            // Need to add additional error handling here     
        }

        /// <summary>
        /// Accesses the crop inputs of an IAT file and moves the residue data to an independent PRN file
        /// </summary>
        /// <param name="crop">The worksheet object containing crop inputs</param>
        /// <param name="name">The name of the output file</param>
        public static void FileResidue(IAT iat)
        {
            try
            {
                FileStream stream = new FileStream($"{Toolbox.OutDir}/{iat.name}/{iat.name}_FileCropResidue.prn", FileMode.Create);
                StreamWriter swriter = new StreamWriter(stream);
                WorksheetPart residue = (WorksheetPart)iat.book.GetPartById(iat.SearchSheets("crop_inputs").Id);

                // Add header to document
                swriter.WriteLine($"{"SoilNum",-35}{"CropName",-35}{"YEAR",-35}{"Month",-35}{"AmtKg",-35}Npct");
                swriter.WriteLine($"{"()",-35}{"()",-35}{"()",-35}{"()",-35}{"()",-35}()");

                // Iterate over spreadsheet data and copy to output stream
                string SoilNum, ForageName, YEAR, Month, AmtKg, Npct;
                var rows = residue.Worksheet.Descendants<Row>().Skip(1);
                foreach (Row row in rows)
                {
                    var cells = row.Descendants<Cell>();
                    if (cells.First().InnerText != iat.climate) continue;

                    SoilNum = iat.ParseCell(cells.ElementAt(1));
                    ForageName = iat.ParseCell(cells.ElementAt(3)).Replace(" ", "");
                    YEAR = iat.ParseCell(cells.ElementAt(5));
                    Month = iat.ParseCell(cells.ElementAt(6));
                    AmtKg = iat.ParseCell(cells.ElementAt(8));
                    Npct = iat.ParseCell(cells.ElementAt(9));

                    // Writing row to file
                    if (AmtKg == "") AmtKg = "0";
                    swriter.WriteLine($"{SoilNum,-35}{ForageName,-35}{YEAR,-35}{Month,-35}{AmtKg,-35}{Npct}");
                }
                swriter.Close();
            }
            catch (IOException)
            {
                // Will only be caught if the file exists and is in use by another program;
                // Alerts the user then safely continues with the program.
                Console.WriteLine("FileCropResidue.prn is open in another application and couldn't be overwritten.");
            }
            // Need to add additional error handling here
        }

    }
}
