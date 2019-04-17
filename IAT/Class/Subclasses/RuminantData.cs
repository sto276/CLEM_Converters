using Models;
using Models.CLEM.Activities;
using Models.CLEM.Groupings;
using Models.CLEM.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace IATReader
{
    public partial class IAT
    {
        /// <summary>
        /// Provides methods for reading and converting
        /// the ruminant data contained within an IAT file
        /// </summary>
        private static class RuminantData
        {
            private static IAT iat;

            public static List<int> Columns = new List<int>();

            private static int col { get; set; }

            public static SubTable Numbers { get; private set; }

            public static SubTable Ages { get; private set; }

            public static SubTable Weights { get; private set; }

            public static SubTable Specs { get; private set; }

            public static SubTable Coeffs { get; private set; }

            public static SubTable Prices { get; private set; }

            public static void Construct(IAT source)
            {
                iat = source;

                Numbers = new SubTable("Startup ruminant numbers", source);
                Ages = new SubTable("Startup ruminant ages", source);
                Weights = new SubTable("Startup ruminant weights", source);
                Specs = new SubTable("Ruminant specifications", source);
                Prices = new SubTable("Ruminant prices", source);
            }

            /// <summary>
            /// Writes the 'Ruminants' resource segment of an .apsimx file
            /// </summary>
            /// <param name="iat">Source IAT</param>
            public static IEnumerable<RuminantType> GetRuminants(RuminantHerd parent)
            {
                List<RuminantType> ruminants = new List<RuminantType>();

                // Iterate over all the present breeds
                foreach (int id in Columns)
                {                   
                    string breed = Numbers.ColumnNames[col].Replace(".", "");

                    RuminantType ruminant = new RuminantType(parent)
                    {
                        Name = breed,
                        Breed = breed
                    };

                    GetParams(ruminant);
                    new Memo(parent) { Text = GetMemoText() };

                    ruminants.Add(ruminant);
                }

                return ruminants;
            }

            private static string GetMemoText()
            {
                StringBuilder text = new StringBuilder();
                text.Append("These parameters are new and may default to incorrect values.");
                text.Append(" Double check these numbers before attempting to run a simulation: \n\n");
                foreach (XElement item in clems.Elements()) text.Append($"{item.Name,10} - {item.Value}\n");

                return text.ToString();
            }

            public static IEnumerable<RuminantTypeCohort> GetCohorts(RuminantInitialCohorts parent)
            {
                List<RuminantTypeCohort> cohorts = new List<RuminantTypeCohort>();

                int row = -1;
                int col = parent.id;

                foreach (string cohort in Numbers.RowNames)
                {
                    row++;
                    if (Numbers.GetData<string>(row, col) != "0")
                    {
                        // Check gender
                        int gender = 0;
                        if (cohort.Contains("F")) gender = 1;

                        // Check suckling
                        bool suckling = false;
                        if (cohort.Contains("Calf")) suckling = true;

                        // Check breeding sire
                        bool sire = false;
                        if (cohort == "Breeding Sires") sire = true;

                        cohorts.Add(new RuminantTypeCohort(parent)
                        {
                            Name = cohort,
                            Gender = gender,
                            Age = (int)Math.Ceiling(Ages.GetData<double>(row, col)),
                            Number = (int)Math.Ceiling(Numbers.GetData<double>(row, col)),
                            Weight = Weights.GetData<double>(row, col),
                            Suckling = suckling,
                            Sire = false
                        });
                    }
                }

                return cohorts;
            }

            /// <summary>
            /// Finds the coefficients/specifications of a single breed in an IAT
            /// </summary>
            /// <param name="iat">Source IAT</param>
            /// <param name="col">Column containing desired breed data</param>
            /// <returns></returns>
            private static XElement GetIATParams(IAT iat, int col)
            {
                XElement parameters = new XElement("Parameters");

                // Read the .csv mapping IAT names to CLEM names
                StreamReader csvIAT = new StreamReader($"{Directory.GetCurrentDirectory()}/IAT/Resources/Resources_IAT.csv");

                // Reads the map line by line, finding IAT data (skipping the title row)
                csvIAT.ReadLine();
                string line;
                while ((line = csvIAT.ReadLine()) != null)
                {
                    string[] items = line.Split(',');

                    // First column in the map contains IAT table name
                    IATable data = iat.tables[items[0]];

                    // Second column in the map contains the standard row number for that value in IAT (shifted to 0 based array)
                    int row = Convert.ToInt32(items[1]) - 1;

                    double value = 0;
                    // Different IAT versions may not contain some resources in the list, this conditional checks that
                    if (row < data.GetRowNames().Count)
                    {
                        value = (data.GetData<double>(row, col)) * Convert.ToDouble(items[4]);
                    }
                    parameters.Add(new XElement(items[3], value.ToString()));
                }
                csvIAT.Close();
                csvIAT.Dispose();

                return parameters;
            }

            private static XElement GetCLEMParams()
            {
                XElement parameters = new XElement("CLEMParams");
                // Reads in parameters which are new to CLEM and don't exist in IAT (Defaults to B. Indicus values)            
                StreamReader csvCLEM = new StreamReader($"{Directory.GetCurrentDirectory()}/IAT/Resources/Resources_CLEM.csv");
                csvCLEM.ReadLine();
                string line;
                while ((line = csvCLEM.ReadLine()) != null)
                {
                    string[] items = line.Split(',');
                    parameters.Add(new XElement(items[0], items[1]));
                }
                csvCLEM.Close();

                return parameters;
            }

            public static IEnumerable<AnimalPriceGroup> GetAnimalPrices(AnimalPricing parent)
            {
                List<AnimalPriceGroup> prices = new List<AnimalPriceGroup>();

                double sire_price = 0;
                int row = -1;
                foreach (string cohort in Numbers.RowNames)
                {
                    row++;
                    if (Numbers.GetData<double>(row, col) != 0)
                    {
                        if (!cohort.ToLower().Contains("sire")) sire_price = Prices.GetData<double>(row, col);

                        prices.Add(new AnimalPriceGroup(parent)
                        {
                            Name = cohort,
                            Value = Prices.GetData<double>(row, col)
                        });

                    }
                }
                parent.SirePrice = sire_price;

                return prices;
            }           

            private static ActivityFolder GetManageBreeds(ActivityFolder parent)
            {
                ActivityFolder breeds = new ActivityFolder(parent)
                {
                    Name = "Breeds"
                };

                foreach (int col in Columns)
                {                 
                    // Add a new folder for individual breed
                    ActivityFolder breed = new ActivityFolder(breeds)
                    {
                        Name = "Manage " + Specs.ColumnNames[col]
                    };

                    // Manage breed weaning
                    new RuminantActivityWean(breed)
                    {
                        WeaningAge = Specs.GetData<double>(7, col),
                        WeaningWeight = Specs.GetData<double>(8, col)
                    }; 

                    // Manage breed milking
                    if (Specs.GetData<double>(18, col) > 0) new RuminantActivityMilking(breed);

                    // Manage breed numbers
                    RuminantActivityManage numbers = new RuminantActivityManage(breed)
                    {
                        MaximumBreedersKept = Specs.GetData<int>(2, col),
                        MinimumBreedersKept = Specs.GetData<int>(38, col),
                        MaximumBreedingAge = Specs.GetData<double>(3, col),
                        MaximumBullAge = Specs.GetData<double>(25, col),
                        MaleSellingAge = Specs.GetData<double>(5, col),
                        MaleSellingWeight = Specs.GetData<double>(6, col)
                    };

                    new ActivityTimerInterval(numbers)
                    {
                        Interval = 12,
                        MonthDue = 12
                    };

                    // Manage sale of dry breeders
                    new RuminantActivitySellDryBreeders(breed)
                    {
                        MonthsSinceBirth = Specs.GetData<double>(32, col),
                        ProportionToRemove = Specs.GetData<double>(4, col) * 0.01
                    };

                }

                return breeds;
            }

            /// <summary>
            /// Writes the 'Ruminant Manage' activity section
            /// </summary>
            /// <param name="iat">Source IAT</param>
            public static XElement Manage(IAT iat)
            {
                // Don't need to do anything if no ruminants are present
                if (Columns.Count == 0) return null;

                //Section: ActivityFolder - Manage herd
                XElement manage = new XElement
                (
                    "ActivityFolder",
                    new XElement("Name", $"Manage herd"),
                    GetCutAndCarry(iat),
                    new XElement("IncludeInDocumentation", "true"),
                    new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop")
                );

                return manage;
            }

            /// <summary>
            /// Writes the 'Cut and carry' Activity section of a CLEM simulation
            /// </summary>
            /// <param name="iat"></param>
            private static XElement GetCutAndCarry(IAT iat)
            {
                // Checks the feeding system for each breed, and adds cut & carry if appropriate


                XElement cutcarry = new XElement
                (
                    "ActivityFolder",
                    new XElement("Name", "Cut and carry"),
                    GetCCFeeds(iat),
                    new XElement("IncludeInDocumentation", "true"),
                    new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop")
                );
                return cutcarry;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="iat"></param>
            /// <returns></returns>
            private static IEnumerable<XElement> GetCCFeeds(IAT iat)
            {
                XElement feeds = new XElement("feeds");

                foreach (int pool in iat.pools.Keys)
                {
                    XElement feed = new XElement
                    (
                        "RuminantActivityFeed",
                        new XElement("Name", $"Feed {iat.pools[pool]}"),
                        GetFeedGroups(iat),
                        LabourData.GetLabourRequirement(),
                        new XElement("IncludeInDocumentation", "true"),
                        new XElement("OnPartialResourcesAvailableAction", "UseResourcesAvailable"),
                        new XElement("HerdFilters", ""),
                        new XElement("FeedTypeName", $"AnimalFoodStore.{iat.pools[pool]}"),
                        new XElement("ProportionTramplingWastage", "0.3"),
                        new XElement("FeedStyle", "ProportionOfPotentialIntake")
                    );
                    feeds.Add(feed);
                }

                return feeds.Elements();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="breed"></param>
            /// <returns></returns>
            private static IEnumerable<XElement> GetFeedGroups(IAT iat)
            {
                XElement groups = new XElement("Groups");

                foreach (int col in Columns)
                {
                    string breed = Specs.ColumnNames[col];

                    XElement group = new XElement
                    (
                        "RuminantFeedGroup",
                        new XElement("Name", breed),
                        new XElement("IncludeInDocumentation", "false")
                    );
                }

                return groups.Elements();
            }           

        }
    }
}