using Models;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ReadIAT
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
            Directory.CreateDirectory($"{OutDir}/{Name}");

            // Load the .xlsx into various objects
            Doc = SpreadsheetDocument.Open(path, false);
            Book = Doc.WorkbookPart;
            StringTable = Book.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();            

            // Write PRN files
            WriteCropPRN();
            WriteForagePRN();
            WriteResiduePRN();

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
            // Find climate data (assumed to exist in specific cell)
            Climate = GetCellValue(Part, 4, 6);

            CropData.Construct(this);
            FinanceData.Construct(this);
            ForageData.Construct(this);
            LabourData.Construct(this);
            LandData.Construct(this);            
            StoreData.Construct(this);
            RuminantData.Construct(this);
        }       
    } 
}