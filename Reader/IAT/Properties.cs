/* 
 This file contains all the properties of the IAT class.
*/ 

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;

namespace Reader
{    
    public partial class IAT
    {
        public static bool GroupSheets { get; set; }

        public static bool GroupSims { get; set; }

        /// <summary>
        /// Name of the IAT
        /// </summary>
		public string Name { get; set; }      

        public SpreadsheetDocument Document { get; set; }

        /// <summary>
        /// Workbook information derived from a document
        /// </summary>
        public WorkbookPart Book { get; set; }

        /// <summary>
        /// The current parameter sheet
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
        /// Crops grown table
        /// </summary>
        private SubTable CropsGrown { get; set; }

        /// <summary>
        /// Crop specifications table
        /// </summary>
        private SubTable CropSpecs { get; set; }

        /// <summary>
        /// Forages grown table
        /// </summary>
        private SubTable ForagesGrown { get; set; }

        /// <summary>
        /// Forage specification table
        /// </summary>
        private SubTable ForageSpecs { get; set; }

        /// <summary>
        /// Land specification table
        /// </summary>
        private SubTable LandSpecs { get; set; }

        /// <summary>
        /// Labour supply/hire table
        /// </summary>
        private SubTable LabourSupply { get; set; }

        /// <summary>
        /// Startup ruminant numbers table
        /// </summary>
        private SubTable RumNumbers { get; set; }

        /// <summary>
        /// Startup ruminant ages table
        /// </summary>
        private SubTable RumAges { get; set; }

        /// <summary>
        /// Startup ruminant weights table
        /// </summary>
        private SubTable RumWeights { get; set; }

        /// <summary>
        /// Ruminant specifications
        /// </summary>
        public SubTable RumSpecs { get; set; }

        /// <summary>
        /// The Ruminant coefficients table
        /// </summary>
        /// <remarks>
        /// This property isn't referenced directly, but through reflection.
        /// DO NOT REMOVE.
        /// </remarks>
        public SubTable RumCoeffs { get; set; }

        /// <summary>
        /// Ruminant prices
        /// </summary>
        private SubTable RumPrices { get; set; }

        /// <summary>
        /// Financial overheads table
        /// </summary>
        private SubTable Overheads { get; set; }

        /// <summary>
        /// Bought fodder table
        /// </summary>
        private SubTable Fodder { get; set; }

        /// <summary>
        /// Bought fodder specifications table
        /// </summary>
        private SubTable FodderSpecs { get; set; }

        /// <summary>
        /// Climate region of the simulation
        /// </summary>
        private string Climate { get; set; } = "1";

        /// <summary>
        /// IDs of all grains present in the simulation
        /// </summary>
        private List<int> GrainIDs { get; set; }

        /// <summary>
        /// IDs of all ruminant types present in the simulation
        /// </summary>
        private List<int> RumIDs { get; set; }

        private bool disposed = false;
    }
}
