namespace IAT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;

    /// <summary>
    /// File information and metadata regarding an IAT file
    /// </summary>
    public class IAT
	{
        /// <summary>
        /// Name of the IAT file
        /// </summary>
		public string name;

        /// <summary>
        /// Climate region of the simulation
        /// </summary>
        public string climate = "1";

        /// <summary>
        /// Tables in the IAT file.
        /// Keys are the table names.
        /// Values are an object representation of the table.
        /// </summary>
        public Dictionary<string, IATable> tables = new Dictionary<string, IATable>();

        /// <summary>
        /// Fodder pools used in the simulation.
        /// Keys are the pool ID.
        /// Values are the crop names in the pool
        /// </summary>
        public Dictionary<int, string> pools = new Dictionary<int, string>();

        /// <summary>
        /// Column ID of each present ruminant type
        /// </summary>
        public List<int> ruminants = new List<int>();

        /// <summary>
        /// Numeric ID of each grain grown
        /// </summary>
        public List<int> grains = new List<int>();

        /// <summary>
        /// Object representation of the .xlsx
        /// </summary>
        public SpreadsheetDocument doc;

        /// <summary>
        /// Workbook information from the document object
        /// </summary>
        public WorkbookPart book;

        /// <summary>
        /// Information about the current parameter sheet
        /// </summary>
        public Sheet sheet;

        /// <summary>
        /// Data from the current parameter sheet
        /// </summary>
        public WorksheetPart part;

        /// <summary>
        /// Contains all unique strings used in cells (accessed via numeric ID)
        /// </summary>
        public SharedStringTablePart string_table;

        /// <summary>
        ///Constructs a new IAT object with the given name
        /// </summary>
        /// <param name="path">Path to the IAT file</param>
        public IAT(string path)
        {
            name = Path.GetFileNameWithoutExtension(path);
            Console.WriteLine($"Converting {name}");

            // Sanitise the filename so it is compatible with XML tags
            name = Toolbox.SanitiseXName(name);

            // Load the .xlsx into various objects
            doc = SpreadsheetDocument.Open(path, false);
            book = doc.WorkbookPart;
            string_table = book.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

            // Find a parameter sheet to use
            SetSheet("param");

            // Find climate data (assumed to exist in specific cell)
            climate = GetCellValue(part, 4, 6);

            // Look through the .xlsx for necessary data tables
            GetTables();

            // Determine which fodder is stored where
            FoodStores.GetGrownFodderPools(this);
            FoodStores.GetBoughtFodderPools(this);

            // Determine which ruminants are present in the simulation
            FindRuminants();
        }      

        /// <summary>
        /// Looks through the ExcelPackage for the first sheet containing the given string.
        /// </summary>
        /// <param name="sheetname">The sheet to search for</param>
        /// <returns></returns>
        public Sheet FindSheet(string name)
        {
            return book.Workbook.Descendants<Sheet>().
                Where(s => s.Name.ToString().ToLower().Contains(name.ToLower())).
                FirstOrDefault();
        }

        /// <summary>
        /// Changes which sheet in the source document IAT data is taken from
        /// </summary>
        /// <param name="newsheet">The name of the new sheet</param>
        public void SetSheet(string name)
        {
            // This assumes a valid IAT is provided as input and will break otherwise (need to fix)
            sheet = FindSheet(name);
            part = (WorksheetPart)book.GetPartById(sheet.Id);
            return;
        }

        /// <summary>
        /// Parses a cell and returns its value in string representation
        /// </summary>
        /// <param name="cell">The cell to parse</param>
        public string ParseCell(Cell cell)
        {
            // Access the cell contents
            string value = cell.InnerText;
            if (cell.DataType != null)
            {
                // If the CellValue is a shared string, look through the shared table for its value
                if (cell.DataType.Value == CellValues.SharedString)
                {
                    value = string_table.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
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
        /// Search the Cells in the the worksheet by position
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

        /// <summary>
        /// Loads the required tables in the IAT
        /// </summary>
        private void GetTables()
        {
            List<string> names = new List<string>()
            {
                "Land specifications",
                "Overheads",
                "Labour supply/hire",
                "Grain Crops Grown",
                "Forage Crops Grown",
                "Grain Crop Specifications",
                "Forage Crop Specifications",
                "Bought fodder",
                "Bought fodder specs",
                "Ruminant coefficients",
                "Ruminant specifications",
                "Startup ruminant numbers",
                "Startup ruminant ages",
                "Startup ruminant weights",
                "Ruminant prices"
            };
            foreach(var n in names) tables.Add(n, new IATable(n, this));
        }

        /// <summary>
        /// If a ruminant has any cohorts, add it to the list of tracked ruminants
        /// </summary>
        private void FindRuminants()
        {
            IATable nums = tables["Startup ruminant numbers"];

            int col = -1;
            foreach (string breed in nums.GetColNames())
            {
                col++;
                var x = nums.GetColData<string>(col);
                if (x.Exists(v => v != "0")) ruminants.Add(col);
            }
        }

        /// <summary>
        /// Creates the XML structure for the Resources section of a CLEM simulation
        /// </summary>
        /// <returns>XML structure of CLEM Activities</returns>
        public XElement GetResources()
        {
            XElement resources = new XElement("ResourcesHolder");
            resources.Add(new XElement("Name", "Resources"));
            resources.Add(Land.GetLand(this));
            resources.Add(Labour.GetLabour(this));
            resources.Add(Ruminants.GetRuminants(this));
            resources.Add(Finances.GetBank(this));
            resources.Add(FoodStores.GetAnimalFoodStore(this));
            resources.Add(FoodStores.GetHumanFoodStore(this));
            resources.Add(FoodStores.GetGrazeFoodStore());
            resources.Add(FoodStores.GetProductStore(this));
            resources.Add(new XElement("IncludeInDocumentation", "true"));
            return resources;
        }

        /// <summary>
        /// Creates the XML structure for the Activities section of a CLEM simulation
        /// </summary>
        /// <returns>XML structure of CLEM Activities</returns>
        public XElement GetActivities()
        {
            XElement activities = new XElement("ActivitiesHolder");
            activities.Add(new XElement("Name", "Activities"));
            activities.Add(Finances.CashFlow(this));
            activities.Add(Grains.GetManage(this));
            activities.Add(Forages.GetManage(this));
            activities.Add(Ruminants.Manage(this));

            // Only if ruminants are present
            if (ruminants.Count > 0)
            {
                activities.Add(Reports.GetHerdSummary());
                activities.Add(Reports.GetHerdReport());
            }

            activities.Add(new XElement("IncludeInDocumentation", "true"));
            return activities;
        }
    } 
}