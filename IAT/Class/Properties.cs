using System.Collections.Generic;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace IATReader
{
    public partial class IAT
    {
        /// <summary>
        /// Name of the IAT file
        /// </summary>
		public string name;

        /// <summary>
        /// The directory to look for IAT files in
        /// </summary>
        public string InDir { get; set; } = "Simulations";

        /// <summary>
        /// The directory to write CLEM simulations to
        /// </summary>
        public string OutDir { get; set; } = "Simulations";

        /// <summary>
        /// Climate region of the simulation
        /// </summary>
        public string climate = "1";

        /// <summary>
        /// Fodder pools used in the simulation.
        /// Keys are the pool ID.
        /// Values are the crop names in the pool
        /// </summary>
        public Dictionary<int, string> pools = new Dictionary<int, string>();

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


    }
}
