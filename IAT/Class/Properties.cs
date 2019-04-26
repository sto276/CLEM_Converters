using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ReadIAT
{
    public partial class IAT
    {
        /// <summary>
        /// Name of the IAT file
        /// </summary>
		public string Name { get; set; }

        /// <summary>
        /// The directory to write CLEM simulations to
        /// </summary>
        public static string OutDir { get; set; } = "Simulations";                       

        /// <summary>
        /// Object representation of the .xlsx
        /// </summary>
        private SpreadsheetDocument Doc { get; set; }

        /// <summary>
        /// Workbook information from the document object
        /// </summary>
        public WorkbookPart Book { get; set; }

        /// <summary>
        /// Information about the current parameter sheet
        /// </summary>
        public Sheet ParameterSheet { get; set; }

        /// <summary>
        /// Data from the current parameter sheet
        /// </summary>
        private WorksheetPart Part { get; set; }

        /// <summary>
        /// Contains all unique strings used in cells (accessed via numeric ID)
        /// </summary>
        private SharedStringTablePart StringTable { get; set; }

        /// <summary>
        /// Climate region of the simulation
        /// </summary>
        private string Climate { get; set; } = "1";
    }
}
