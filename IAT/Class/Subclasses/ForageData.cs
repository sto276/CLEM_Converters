using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace IATReader
{
    public partial class IAT
    {
        /// <summary>
        /// Provides methods for reading and converting
        /// the forage crop data contained within an IAT file
        /// </summary>
        private static class ForageData
        {
            private static IAT iat;

            public static SubTable CropsGrown { get; }

            public static SubTable CropSpecs { get; }

            public static void Construct(IAT source)
            {
                iat = source;
            }

            /// <summary>
            /// Writes the 'Manage forages' activity segment of an .apsimx file
            /// </summary>
            /// <param name="iat">The IAT file to access data from</param>
            public static XElement GetManage(IAT iat)
            {
                XElement manage = new XElement("ActivityFolder");
                manage.Add(new XElement("Name", "Manage forages"));

                int col = 0;
                // Check present forages
                while (CropsGrown.GetData<string>(0, col) != "0")
                {
                    double area = CropsGrown.GetData<double>(2, col);
                    if (area > 0)
                    {
                        int row = CropsGrown.GetData<int>(0, col);
                        string name = CropSpecs.GetRowNames()[row + 1];

                        XElement crop = new XElement("CropActivityManageCrop");
                        crop.Add(new XElement("Name", $"Manage {name}"));

                        XElement product = new XElement("CropActivityManageProduct");
                        product.Add(new XElement("Name", $"Cut and carry {name}"));
                        product.Add(new XElement("IncludeInDocumentation", "true"));
                        product.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));
                        product.Add(new XElement("ModelNameFileCrop", "FileForage"));
                        product.Add(new XElement("CropName", name));
                        product.Add(new XElement("StoreItemName", "AnimalFoodStore"));
                        product.Add(new XElement("ProportionKept", "1"));
                        product.Add(new XElement("TreesPerHa", "0"));
                        product.Add(new XElement("UnitsToHaConverter", "0"));
                        crop.Add(product);

                        int num = CropsGrown.GetData<int>(1, col);
                        crop.Add(new XElement("IncludeInDocumentation", "true"));
                        crop.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));
                        crop.Add(new XElement("LandItemNameToUse", $"Land.{LandSpecs.GetRowNames()[num]}"));
                        crop.Add(new XElement("AreaRequested", CropsGrown.GetData<string>(2, col)));
                        crop.Add(new XElement("UseAreaAvailable", "false"));
                        manage.Add(crop);
                    }

                    col++;
                }

                // Will not be included in final XML if there are no forages
                if (manage.Elements().Count() > 1)
                {
                    // Checks the feeding system for each breed, and adds the native pasture if appropriate
                    IATable rs = iat.tables["Ruminant specifications"];
                    foreach (int breed in iat.ruminants)
                    {
                        if (1 < rs.GetData<int>(28, breed))
                        {
                            GetNativePasture(manage);
                            break;
                        }
                    }
                    manage.Add(new XElement("IncludeInDocumentation", "true"));
                    manage.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));

                    return manage;
                }
                else
                    return null;
            }

            /// <summary>
            /// Writes the 'Manage Native Pasture' activity segment of a .apsimx file
            /// </summary>
            private static void GetNativePasture(XElement manage)
            {
                // Runs exactly twice
                bool common = false;
                do
                {
                    XElement crop = new XElement("CropActivityManageCrop");
                    if (common)
                        crop.Add(new XElement("Name", $"Native Pasture Common Land"));
                    else
                        crop.Add(new XElement("Name", "Native Pasture Farm"));

                    XElement product = new XElement("CropActivityManageProduct");
                    if (common)
                        product.Add(new XElement("Name", "Grazed Common Land"));
                    else
                        product.Add(new XElement("Name", "Cut and carry Native Pasture"));

                    product.Add(new XElement("IncludeInDocumentation", "true"));
                    product.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));
                    product.Add(new XElement("ModelNameFileCrop", "FileForage"));
                    product.Add(new XElement("CropName", "Native_grass"));

                    if (common)
                        product.Add(new XElement("StoreItemName", "GrazeFoodStore.NativePasture"));
                    else
                        product.Add(new XElement("StoreItemName", "AnimalFoodStore.NativePasture"));

                    product.Add(new XElement("ProportionKept", "1"));
                    product.Add(new XElement("TreesPerHa", "0"));
                    product.Add(new XElement("UnitsToHaConverter", "0"));
                    crop.Add(product);

                    crop.Add(new XElement("IncludeInDocumentation", "true"));
                    crop.Add(new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"));
                    crop.Add(new XElement("LandItemNameToUse", "Land"));
                    crop.Add(new XElement("AreaRequested", "0"));
                    crop.Add(new XElement("UseAreaAvailable", (!common).ToString().ToLower()));
                    manage.Add(crop);

                    common = !common;
                } while (common);
            }

            /// <summary>
            /// Accesses the forage inputs of an IAT file and moves the data to an independent PRN file
            /// </summary>
            /// <param name="crop">The worksheet object containing forage inputs</param>
            /// <param name="name">The name of the output file</param>
            public static void FileForage(IAT iat)
            {
                try
                {
                    FileStream stream = new FileStream($"{Toolbox.OutDir}/{iat.name}/{iat.name}_FileForage.prn", FileMode.Create);
                    StreamWriter swriter = new StreamWriter(stream);
                    WorksheetPart forage = (WorksheetPart)iat.book.GetPartById(iat.SearchSheets("forage_inputs").Id);

                    // Add header to document
                    swriter.WriteLine($"{"SoilNum",-36}{"CropName",-36}{"YEAR",-36}{"Month",-36}{"AmtKg",-36}Npct");
                    swriter.WriteLine($"{"()",-36}{"()",-36}{"()",-36}{"()",-36}{"()",-36}()");

                    // Iterate over spreadsheet data and copy to output stream
                    string SoilNum, ForageName, YEAR, Month, AmtKg, Npct;
                    var rows = forage.Worksheet.Descendants<Row>().Skip(1);
                    foreach (Row row in rows)
                    {
                        var cells = row.Descendants<Cell>();
                        if (cells.First().InnerText != iat.climate) continue;

                        SoilNum = iat.ParseCell(cells.ElementAt(1));
                        ForageName = iat.ParseCell(cells.ElementAt(3)).Replace(" ", "");
                        YEAR = iat.ParseCell(cells.ElementAt(5));
                        Month = iat.ParseCell(cells.ElementAt(7));
                        AmtKg = iat.ParseCell(cells.ElementAt(8));
                        Npct = iat.ParseCell(cells.ElementAt(9));

                        // Write row to file
                        if (AmtKg == "") AmtKg = "0";
                        swriter.WriteLine($"{SoilNum,-36}{ForageName,-36}{YEAR,-36}{Month,-36}{AmtKg,-36}{Npct}");
                    }

                    swriter.Close();
                }
                catch (IOException)
                {
                    // Will only be caught if the file exists and is in use by another program;
                    // Alerts the user then safely continues with the program.
                    Console.WriteLine("FileForage.prn is open in another application and couldn't be overwritten.");
                }
                // Need to add additional error handling here            
            }
        }
    }
}
