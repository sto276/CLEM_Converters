using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Reader
{    
    /// <summary>
    /// Data sourced from an IAT file
    /// </summary>
    public partial class IAT : IApsimX
	{       
        /// <summary>
        ///Constructs a new IAT object with the given name
        /// </summary>
        /// <param name="path">Path to the IAT file</param>
        public IAT(string path)
        {
            Name = Path.GetFileNameWithoutExtension(path);
            Directory.CreateDirectory($"{Shared.OutDir}/{Name}");

            // Load the workbook
            Book = SpreadsheetDocument.Open(path, false).WorkbookPart;

            // Find the string table
            StringTable = Book.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();            

            // Write PRN files
            WriteCropPRN();
            WriteForagePRN();
            WriteResiduePRN();

            // Update the Progress bar
            Progress?.Invoke(this, EventArgs.Empty);

            // Find a parameter sheet to use
            SetSheet("param");                      
        }
        
        /// <summary>
        /// Changes which sheet in the source document IAT data is taken from
        /// </summary>
        /// <param name="newsheet">The name of the new sheet</param>
        public void SetSheet(string name)
        {
            // This assumes a valid IAT is provided as input and will break otherwise (need to fix)
            ParameterSheet = FindSheet(name);
            if (ParameterSheet == null) ParameterSheet = SearchSheets(name);
            Part = (WorksheetPart)Book.GetPartById(ParameterSheet.Id);

            // Initialise the data for the new sheet
            PrepareData();
        }

        /// <summary>
        /// Looks through the ExcelPackage for the first sheet containing the given string.
        /// </summary>
        /// <param name="sheetname">The sheet to search for</param>
        /// <returns></returns>
        public Sheet FindSheet(string name)
        {
            return Book.Workbook.Descendants<Sheet>().
                Where(s => s.Name.ToString() == name).
                FirstOrDefault();
        }

        public Sheet SearchSheets(string name)
        {
            return Book.Workbook.Descendants<Sheet>().
                Where(s => s.Name.ToString().ToLower().Contains(name.ToLower())).
                FirstOrDefault();
        }

        public void PrepareData()
        {           
            // Load all the sub tables
            CropsGrown = new SubTable("Grain Crops Grown", this);
            CropSpecs = new SubTable("Grain Crop Specifications", this);
            ForagesGrown = new SubTable("Forage Crops Grown", this);
            ForageSpecs = new SubTable("Forage Crop Specifications", this);
            LandSpecs = new SubTable("Land specifications", this);
            LabourSupply = new SubTable("Labour supply/hire", this);
            RumNumbers = new SubTable("Startup ruminant numbers", this);
            RumAges = new SubTable("Startup ruminant ages", this);
            RumWeights = new SubTable("Startup ruminant weights", this);
            RumCoeffs = new SubTable("Ruminant coefficients", this);
            RumSpecs = new SubTable("Ruminant specifications", this);
            RumPrices = new SubTable("Ruminant prices", this);
            Overheads = new SubTable("Overheads", this);
            Fodder = new SubTable("Bought fodder", this);
            FodderSpecs = new SubTable("Bought fodder specs", this);

            // Find climate data (assumed to exist in specific cell)
            Climate = GetCellValue(Part, 4, 6);

            // Find total land area
            TotalArea = LandSpecs.GetColData<double>(0).Sum();

            // Set values            
            SetGrains();
            SetRuminants();

            Pools = new Dictionary<int, string>();            
            GetGrownFodderPools();
            GetBoughtFodderPools();
        }       
    } 
}