using Models;
using Models.CLEM.Activities;
using Models.CLEM.Groupings;
using Models.CLEM.Resources;
using System;
using System.Collections.Generic;

namespace ReadIAT
{
    public partial class IAT
    {
        /// <summary>
        /// Provides methods for reading and converting
        /// the ruminant data contained within an IAT file
        /// </summary>
        private static class RuminantData
        {
            public static int Col { get; set; }

            public static List<int> Columns { get; private set; }            

            public static SubTable Numbers { get; private set; }

            public static SubTable Ages { get; private set; }

            public static SubTable Weights { get; private set; }

            public static SubTable Specs { get; private set; }

            public static SubTable Coeffs { get; private set; }

            public static SubTable Prices { get; private set; }

            /// <summary>
            /// Hardcoded mapping of IAT variables to CLEM.
            /// See declaration for definition of items.
            /// </summary>
            /// <remarks>
            ///    - Item1, Table containing data
            ///    - Item2, Name of variable in IAT
            ///    - Item3, Name of variable in CLEM
            ///    - Item4, Conversion factor
            /// </remarks>
            private static List<Tuple<SubTable, string, string, double>> maps;

            public static void Construct(IAT source)
            {
                Columns = new List<int>();

                Numbers = new SubTable("Startup ruminant numbers", source);
                Ages = new SubTable("Startup ruminant ages", source);
                Weights = new SubTable("Startup ruminant weights", source);
                Coeffs = new SubTable("Ruminant coefficients", source);
                Specs = new SubTable("Ruminant specifications", source);
                Prices = new SubTable("Ruminant prices", source);

                maps = new List<Tuple<SubTable, string, string, double>>()
                {
                    new Tuple<SubTable, string, string, double>(Coeffs, "SRW", "SRWFemale", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "birth_SRW", "SRWBirth", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "Critical_cow_wt", "CriticalCowWeight", 0.01),
                    new Tuple<SubTable, string, string, double>(Coeffs, "grwth_coeff1", "AgeGrowthRateCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "grwth_coeff2", "SRWGrowthScalar", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "km_coeff", "EMaintEfficiencyCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "km_incpt", "EMaintEfficiencyIntercept", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "kg_coeff", "EGrowthEfficiencyCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "kg_incpt", "EGrowthEfficiencyIntercept", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "kl_coeff", "ELactationEfficiencyCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "kl_incpt", "ELactationEfficiencyIntercept", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "kme", "Kme", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "intake_coeff", "IntakeCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "intake_incpt", "IntakeIntercept", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "IPI_coeff", "InterParturitionIntervalCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "IPI_incpt", "InterParturitionIntervalIntercept", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "birth_rate_coeff", "ConceptionRateCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "birth_rate_incpt", "ConceptionRateIntercept", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "birth_rate_assym", "ConceptionRateAsymptote", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "juvenile_mort_coeff", "JuvenileMortalityCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "juvenile_mort_exp", "JuvenileMortalityExponent", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "juvenile_mort_max", "JuvenileMortalityMaximum", 0.01),
                    new Tuple<SubTable, string, string, double>(Coeffs, "wool_coeff", "WoolCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "cashmere_coeff", "CashmereCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "Rum_gest_int", "GestationLength", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "Milk_offset_day", "MilkOffsetDay", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "Milk_Peak_day", "MilkPeakDay", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "Milk_Curve_suck", "MilkCurveSuckling", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "Milk_Curve_nonsuck", "MilkCurveNonSuckling", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "protein_coeff", "ProteinCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "protein_degrad", "ProteinDegradability", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "milk_intake_coeff", "MilkIntakeCoefficient", 1),
                    new Tuple<SubTable, string, string, double>(Coeffs, "milk_intake_incpt", "MilkIntakeIntercept", 1),
                    new Tuple<SubTable, string, string, double>(Specs, "Mortality_base", "MortalityBase", 0.01),
                    new Tuple<SubTable, string, string, double>(Specs, "Twin_rate", "TwinRate", 1),
                    new Tuple<SubTable, string, string, double>(Specs, "Joining_age", "MinimumAge1stMating", 1),
                    new Tuple<SubTable, string, string, double>(Specs, "Joining_size", "MinimumSize1stMating", 0.01),
                    new Tuple<SubTable, string, string, double>(Specs, "Milk_max", "MilkPeakYield", 1),
                    new Tuple<SubTable, string, string, double>(Specs, "Milk_end", "MilkingDays", 30)
                };

                FindRuminants();
            }

            /// <summary>
            /// If a breed has any cohorts, add its column to the master list
            /// </summary>
            private static void FindRuminants()
            {
                int col = -1;
                foreach (string breed in Numbers.ColumnNames)
                {
                    col++;
                    var n = Numbers.GetColData<double>(col);
                    if (n.Exists(v => v > 0)) Columns.Add(col);
                }
            }                   

            /// <summary>
            /// Finds the coefficients/specifications of a single breed in an IAT
            /// </summary>
            /// <param name="iat">Source IAT</param>
            /// <param name="col">Column containing desired breed data</param>
            /// <returns></returns>
            public static void SetParams(RuminantType ruminant)
            {                             
                foreach(var map in maps)
                {
                    int row = map.Item1.RowNames.FindIndex(s => s == map.Item2);
                    if (row < 0) continue;
                    double value = map.Item1.GetData<double>(row, Col) * map.Item4;
                    ruminant.GetType().GetProperty(map.Item3).SetValue(ruminant, value);
                }                
            }
        }

        public IEnumerable<RuminantType> GetRuminants(RuminantHerd parent)
        {          
            List<RuminantType> ruminants = new List<RuminantType>();

            // Iterate over all the present breeds
            foreach (int col in RuminantData.Columns)
            {
                RuminantData.Col = col;

                string breed = RuminantData.Numbers.ColumnNames[col].Replace(".", "");

                RuminantType ruminant = new RuminantType(parent)
                {
                    Name = breed,
                    Breed = breed
                };
                RuminantData.SetParams(ruminant);

                ruminants.Add(ruminant);
            }

            return ruminants;
        }

        public IEnumerable<RuminantTypeCohort> GetCohorts(RuminantInitialCohorts parent)
        {
            List<RuminantTypeCohort> cohorts = new List<RuminantTypeCohort>();

            int row = -1;

            foreach (string cohort in RuminantData.Numbers.RowNames)
            {
                row++;
                if (RuminantData.Numbers.GetData<string>(row, RuminantData.Col) != "0")
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
                        Age = (int)Math.Ceiling(RuminantData.Ages.GetData<double>(row, RuminantData.Col)),
                        Number = (int)Math.Ceiling(RuminantData.Numbers.GetData<double>(row, RuminantData.Col)),
                        Weight = RuminantData.Weights.GetData<double>(row, RuminantData.Col),
                        Suckling = suckling,
                        Sire = sire
                    });
                }
            }

            return cohorts;
        }

        public IEnumerable<AnimalPriceGroup> GetAnimalPrices(AnimalPricing parent)
        {
            List<AnimalPriceGroup> prices = new List<AnimalPriceGroup>();

            double sire_price = 0;
            int row = -1;
            foreach (string cohort in RuminantData.Numbers.RowNames)
            {
                row++;
                if (RuminantData.Numbers.GetData<double>(row, RuminantData.Col) != 0)
                {
                    if (!cohort.ToLower().Contains("sire")) sire_price = RuminantData.Prices.GetData<double>(row, RuminantData.Col);

                    var group = new AnimalPriceGroup(parent)
                    {
                        Name = cohort,
                        Value = RuminantData.Prices.GetData<double>(row, RuminantData.Col)
                    };

                    group.Add(new RuminantFilter(group)
                    { 
                        Name = "GenderFilter",
                        Parameter = 2,
                        Value = cohort.ToLower().Contains("f") ? "Female" : "Male"
                    });

                    group.Add(new RuminantFilter(group)
                    {
                        Name = "AgeFilter",
                        Parameter = 2,
                        Operator = 5,
                        Value = RuminantData.Ages.GetData<string>(row, RuminantData.Col)
                    });

                    prices.Add(group);
                }
            }
            parent.SirePrice = sire_price;

            return prices;
        }

        public IEnumerable<ActivityFolder> GetManageBreeds(ActivityFolder herd)
        {
            SubTable specs = RuminantData.Specs;

            List<ActivityFolder> breeds = new List<ActivityFolder>();

            foreach (int col in RuminantData.Columns)
            {
                // Add a new folder for individual breed
                ActivityFolder breed = new ActivityFolder(herd)
                {
                    Name = "Manage " + specs.ColumnNames[col]
                };

                // Manage breed weaning
                breed.Add(new RuminantActivityWean(breed)
                {
                    WeaningAge = specs.GetData<double>(7, col),
                    WeaningWeight = specs.GetData<double>(8, col)
                });

                // Manage breed milking
                if (specs.GetData<double>(18, col) > 0) breed.Add(new RuminantActivityMilking(breed));

                // Manage breed numbers
                RuminantActivityManage numbers = new RuminantActivityManage(breed)
                {
                    MaximumBreedersKept = specs.GetData<int>(2, col),
                    MinimumBreedersKept = specs.GetData<int>(38, col),
                    MaximumBreedingAge = specs.GetData<double>(3, col),
                    MaximumBullAge = specs.GetData<double>(25, col),
                    MaleSellingAge = specs.GetData<double>(5, col),
                    MaleSellingWeight = specs.GetData<double>(6, col)
                };

                numbers.Add(new ActivityTimerInterval(numbers)
                {
                    Interval = 12,
                    MonthDue = 12
                });

                breed.Add(numbers);

                // Manage sale of dry breeders
                breed.Add(new RuminantActivitySellDryBreeders(breed)
                {
                    MonthsSinceBirth = specs.GetData<double>(32, col),
                    ProportionToRemove = specs.GetData<double>(4, col) * 0.01
                });

                breeds.Add(breed);
            }

            return breeds;
        }

    }
}