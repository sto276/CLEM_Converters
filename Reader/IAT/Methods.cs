﻿using Models;
using Models.CLEM;
using Models.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Reader
{
    public partial class IAT
    {
        public static void Run(IEnumerable<string> files, bool join, bool split)
        {
            Shared.OpenLog("ErrorLog");

            Simulations simulations = new Simulations(null);

            foreach (string file in files)
            {
                IAT iat = new IAT(file);

                Folder folder = new Folder(simulations) { Name = iat.Name };

                // Find all the parameter sheets in the IAT
                List<string> sheets = new List<string>();
                foreach (Sheet sheet in iat.Book.Workbook.Sheets)
                {
                    // Ensure a parameter sheet is selected
                    string name = sheet.Name.ToString();
                    if (!name.ToLower().Contains("param")) continue;
                    iat.SetSheet(name);

                    // Write the resulting simulation to its own .apsimx file
                    if (split)
                    {
                        AttachIAT(simulations, iat);
                        Shared.WriteApsimX(simulations, iat.Name);

                        // Reset the simulations node after writing out
                        simulations = new Simulations(null);
                    }
                    // Or collect all the simulations in the IAT
                    else
                    {
                        if (join) AttachIAT(folder, iat);
                        else AttachIAT(simulations, iat);
                    }
                }

                // Files will already be written if split is true
                if (split) continue;

                // Collect all the IAT files in the same .apsimx file
                if (join) simulations.Children.Add(folder);
                // Only gather parameter sets into the same .apsimx file
                else
                {
                    Shared.WriteApsimX(simulations, iat.Name);
                    simulations = new Simulations(null);
                }
            }
            if (join) Shared.WriteApsimX(simulations, "Simulations");

            Shared.CloseLog();
        }

        private static void AttachIAT(Node node, IAT iat)
        {
            node.Source = iat;
            Simulation simulation = new Simulation(node);
            simulation.Name = iat.ParameterSheet.Name;
            node.Children.Add(simulation);
        }

        public Clock GetClock(Simulation simulation)
        {
            int start_year = Convert.ToInt32(GetCellValue(Part, 44, 4));
            int start_month = Convert.ToInt32(GetCellValue(Part, 45, 4));
            int run_time = Convert.ToInt32(GetCellValue(Part, 46, 4));

            DateTime start = new DateTime(start_year, start_month, 1, 0, 0, 0, 0);
            DateTime end = start.AddYears(run_time);

            return new Clock(simulation)
            {
                StartDate = start,
                EndDate = end
            };
        }

        /// <summary>
        /// IAT does not use GRASP files, so an empty element is returned
        /// </summary>
        public IEnumerable<Node> GetFiles(ZoneCLEM clem)
        {
            List<Node> files = new List<Node>();

            // Add the crop
            files.Add(new FileCrop(clem)
            {
                FileName = clem.Source.Name + "\\FileCrop.prn",
                Name = "FileCrop"
            });

            // Add the crop residue
            files.Add(new FileCrop(clem)
            {
                FileName = clem.Source.Name + "\\FileCropResidue.prn",
                Name = "FileCropResidue"
            });

            // Add the forage crop
            files.Add(new FileCrop(clem)
            {
                FileName = clem.Source.Name + "\\FileForage.prn",
                Name = "FileForage"
            });

            return files.AsEnumerable();
        }

        /// <summary>
        /// Accesses the crop inputs of an IAT file and moves the crop data to an independent PRN file
        /// </summary>
        /// <param name="crop">The worksheet object containing crop inputs</param>
        /// <param name="name">The name of the output file</param>
        public void WriteCropPRN()
        {
            try
            {
                // Overwrite any exisiting PRN file
                FileStream stream = new FileStream($"{Shared.OutDir}/{Name}/FileCrop.prn", FileMode.Create);
                StreamWriter swriter = new StreamWriter(stream);

                // Find the data set
                WorksheetPart crop = (WorksheetPart)Book.GetPartById(SearchSheets("crop_inputs").Id);
                var rows = crop.Worksheet.Descendants<Row>().Skip(1);

                // Write file header to output stream
                swriter.WriteLine($"{"SoilNum",-35}{"CropName",-35}{"YEAR",-35}{"Month",-35}{"AmtKg",-35}");
                swriter.WriteLine($"{"()",-35}{"()",-35}{"()",-35}{"()",-35}()");

                // Iterate over data, writing to output stream
                string SoilNum, CropName, YEAR, Month, AmtKg;
                foreach (Row row in rows)
                {
                    var cells = row.Descendants<Cell>();
                    if (cells.First().InnerText != Climate) continue;

                    SoilNum = ParseCell(cells.ElementAt(1));
                    CropName = ParseCell(cells.ElementAt(3)).Replace(" ", "");
                    YEAR = ParseCell(cells.ElementAt(5));
                    Month = ParseCell(cells.ElementAt(6));
                    AmtKg = ParseCell(cells.ElementAt(7));

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
                Shared.Write(new ConversionError()
                {
                    FileName = Name,
                    FileType = "IAT",
                    Message = "FileCrop.prn was open in another application.",
                    Severity = "Low",
                    Table = "N/A",
                    Sheet = "crop_inputs"
                });
            }
            catch (Exception e)
            {
                Shared.Write(new ConversionError()
                {
                    FileName = Name,
                    FileType = "IAT",
                    Message = e.Message,
                    Severity = "Moderate",
                    Table = "N/A",
                    Sheet = "crop_inputs"
                });
            }    
        }

        /// <summary>
        /// Accesses the crop inputs of an IAT file and moves the residue data to an independent PRN file
        /// </summary>
        /// <param name="crop">The worksheet object containing crop inputs</param>
        /// <param name="name">The name of the output file</param>
        public void WriteResiduePRN()
        {
            try
            {
                FileStream stream = new FileStream($"{Shared.OutDir}/{Name}/FileCropResidue.prn", FileMode.Create);
                StreamWriter swriter = new StreamWriter(stream);
                WorksheetPart residue = (WorksheetPart)Book.GetPartById(SearchSheets("crop_inputs").Id);

                // Add header to document
                swriter.WriteLine($"{"SoilNum",-35}{"CropName",-35}{"YEAR",-35}{"Month",-35}{"AmtKg",-35}Npct");
                swriter.WriteLine($"{"()",-35}{"()",-35}{"()",-35}{"()",-35}{"()",-35}()");

                // Iterate over spreadsheet data and copy to output stream
                string SoilNum, ForageName, YEAR, Month, AmtKg, Npct;
                var rows = residue.Worksheet.Descendants<Row>().Skip(1);
                foreach (Row row in rows)
                {
                    var cells = row.Descendants<Cell>();
                    if (cells.First().InnerText != Climate) continue;

                    SoilNum = ParseCell(cells.ElementAt(1));
                    ForageName = ParseCell(cells.ElementAt(3)).Replace(" ", "");
                    YEAR = ParseCell(cells.ElementAt(5));
                    Month = ParseCell(cells.ElementAt(6));
                    AmtKg = ParseCell(cells.ElementAt(8));
                    Npct = ParseCell(cells.ElementAt(9));

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

        /// <summary>
        /// Accesses the forage inputs of an IAT file and moves the data to an independent PRN file
        /// </summary>
        /// <param name="crop">The worksheet object containing forage inputs</param>
        /// <param name="name">The name of the output file</param>
        public void WriteForagePRN()
        {
            try
            {
                FileStream stream = new FileStream($"{Shared.OutDir}/{Name}/FileForage.prn", FileMode.Create);
                StreamWriter swriter = new StreamWriter(stream);
                WorksheetPart forage = (WorksheetPart)Book.GetPartById(SearchSheets("forage_inputs").Id);

                // Add header to document
                swriter.WriteLine($"{"SoilNum",-36}{"CropName",-36}{"YEAR",-36}{"Month",-36}{"AmtKg",-36}Npct");
                swriter.WriteLine($"{"()",-36}{"()",-36}{"()",-36}{"()",-36}{"()",-36}()");

                // Iterate over spreadsheet data and copy to output stream
                string SoilNum, ForageName, YEAR, Month, AmtKg, Npct;
                var rows = forage.Worksheet.Descendants<Row>().Skip(1);
                foreach (Row row in rows)
                {
                    var cells = row.Descendants<Cell>();
                    if (cells.First().InnerText != Climate) continue;

                    SoilNum = ParseCell(cells.ElementAt(1));
                    ForageName = ParseCell(cells.ElementAt(3)).Replace(" ", "");
                    YEAR = ParseCell(cells.ElementAt(5));
                    Month = ParseCell(cells.ElementAt(7));
                    AmtKg = ParseCell(cells.ElementAt(8));
                    Npct = ParseCell(cells.ElementAt(9));

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
        
        /// <summary>
        /// Parses a cell and returns its value in string representation
        /// </summary>
        /// <param name="cell">The cell to parse</param>
        public string ParseCell(Cell cell)
        {
            // Access the cell contents
            string value = "";
            if (cell.CellValue != null) value = cell.CellValue.InnerText;

            if (cell.DataType != null)
            {
                // If the CellValue is a shared string, look through the shared table for its value
                if (cell.DataType.Value == CellValues.SharedString)
                {
                    value = StringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                }
                // If the CellValue is a bool, convert it into true/false text
                else if (cell.DataType.Value == CellValues.Boolean)
                {
                    if (value == "0") value = "False";
                    else value = "True";
                }
                // else the CellValue must be numeric, so return the contents directly 
            }
            return value;
        }

        /// <summary>
        /// Return the value of a worksheet cell by position
        /// </summary>
        /// <param name="part">The part of the worksheet which contains the cells</param>
        /// <param name="row">The 1-based row index of the cell in the spreadsheet</param>
        /// <param name="col">The 1-based column index of the cell in the spreadsheet</param>
        public string GetCellValue(WorksheetPart part, int row, int col)
        {
            // Convert the cell position into a CellReference (e.g. 3, 2 to B3)
            string coord = GetColReference(col) + row.ToString();

            // Find the cell by its reference
            Cell cell = part.Worksheet.Descendants<Cell>().
                Where(c => c.CellReference == coord).FirstOrDefault();
            if (cell == null) return "";

            // Access the data in the cell
            return ParseCell(cell);
        }

        /// <summary>
        /// Converts an integer from base-10 to base-26 (represented by the English alphabet)
        /// </summary>
        /// <param name="i">Integer to convert</param>
        private static string GetColReference(int i)
        {
            // Consider Z as the 0th letter, rather than the 26th, for safer indexing
            const string alphabet = "ZABCDEFGHIJKLMNOPQRSTUVWXY";
            string result = "";
            char digit;

            while (i > 0)
            {
                // Find the lowest order digit
                digit = alphabet[i % 26];

                // Prepend the digit to the result
                result = digit + result;

                // Use integer division to 'delete' the lowest order digit
                i = i / 26;
            }

            return result;
        }
    }
}
