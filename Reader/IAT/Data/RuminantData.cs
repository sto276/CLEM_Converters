using Models;
using Models.CLEM.Activities;
using Models.CLEM.Groupings;
using Models.CLEM.Resources;
using System;
using System.Collections.Generic;

namespace Reader
{
    public partial class IAT
    {
        /// <summary>
        /// Tracks the breed that is having its data read
        /// </summary>
        private int RumActiveID { get; set; }

        /// <summary>
        /// Contains a mapping from one IAT parameter to one CLEM parameter
        /// </summary>
        private struct Map
        {
            public string Table;
            public string ParamIAT;
            public string ParamCLEM;
            public double Proportion;

            public Map(string table, string paramIAT, string paramCLEM, double proportion)
            {
                Table = table;
                ParamIAT = paramIAT;
                ParamCLEM = paramCLEM;
                Proportion = proportion;
            }
        }            

        /// <summary>
        /// Hardcoded list of all IAT to CLEM mappings.
        /// </summary>
        private readonly List<Map> Maps = new List<Map>()
        {
            new Map("RumCoeffs", "SRW", "SRWFemale", 1),
            new Map("RumCoeffs", "birth_SRW", "SRWBirth", 1),
            new Map("RumCoeffs", "Critical_cow_wt", "CriticalCowWeight", 0.01),
            new Map("RumCoeffs", "grwth_coeff1", "AgeGrowthRateCoefficient", 1),
            new Map("RumCoeffs", "grwth_coeff2", "SRWGrowthScalar", 1),
            new Map("RumCoeffs", "km_coeff", "EMaintEfficiencyCoefficient", 1),
            new Map("RumCoeffs", "km_incpt", "EMaintEfficiencyIntercept", 1),
            new Map("RumCoeffs", "kg_coeff", "EGrowthEfficiencyCoefficient", 1),
            new Map("RumCoeffs", "kg_incpt", "EGrowthEfficiencyIntercept", 1),
            new Map("RumCoeffs", "kl_coeff", "ELactationEfficiencyCoefficient", 1),
            new Map("RumCoeffs", "kl_incpt", "ELactationEfficiencyIntercept", 1),
            new Map("RumCoeffs", "kme", "Kme", 1),
            new Map("RumCoeffs", "intake_coeff", "IntakeCoefficient", 1),
            new Map("RumCoeffs", "intake_incpt", "IntakeIntercept", 1),
            new Map("RumCoeffs", "IPI_coeff", "InterParturitionIntervalCoefficient", 1),
            new Map("RumCoeffs", "IPI_incpt", "InterParturitionIntervalIntercept", 1),
            new Map("RumCoeffs", "birth_rate_coeff", "ConceptionRateCoefficient", 1),
            new Map("RumCoeffs", "birth_rate_incpt", "ConceptionRateIntercept", 1),
            new Map("RumCoeffs", "birth_rate_assym", "ConceptionRateAsymptote", 1),
            new Map("RumCoeffs", "juvenile_mort_coeff", "JuvenileMortalityCoefficient", 1),
            new Map("RumCoeffs", "juvenile_mort_exp", "JuvenileMortalityExponent", 1),
            new Map("RumCoeffs", "juvenile_mort_max", "JuvenileMortalityMaximum", 0.01),
            new Map("RumCoeffs", "wool_coeff", "WoolCoefficient", 1),
            new Map("RumCoeffs", "cashmere_coeff", "CashmereCoefficient", 1),
            new Map("RumCoeffs", "Rum_gest_int", "GestationLength", 1),
            new Map("RumCoeffs", "Milk_offset_day", "MilkOffsetDay", 1),
            new Map("RumCoeffs", "Milk_Peak_day", "MilkPeakDay", 1),
            new Map("RumCoeffs", "Milk_Curve_suck", "MilkCurveSuckling", 1),
            new Map("RumCoeffs", "Milk_Curve_nonsuck", "MilkCurveNonSuckling", 1),
            new Map("RumCoeffs", "protein_coeff", "ProteinCoefficient", 1),
            new Map("RumCoeffs", "protein_degrad", "ProteinDegradability", 1),
            new Map("RumCoeffs", "milk_intake_coeff", "MilkIntakeCoefficient", 1),
            new Map("RumCoeffs", "milk_intake_incpt", "MilkIntakeIntercept", 1),
            new Map("RumSpecs", "Mortality_base", "MortalityBase", 0.01),
            new Map("RumSpecs", "Twin_rate", "TwinRate", 1),
            new Map("RumSpecs", "Joining_age", "MinimumAge1stMating", 1),
            new Map("RumSpecs", "Joining_size", "MinimumSize1stMating", 0.01),
            new Map("RumSpecs", "Milk_max", "MilkPeakYield", 1),
            new Map("RumSpecs", "Milk_end", "MilkingDays", 30)
        };

        /// <summary>
        /// If a breed has any cohorts, add its column to the master list
        /// </summary>
        private void SetRuminants()
        {
            RumIDs = new List<int>();

            int col = -1;
            foreach (string breed in RumNumbers.ColumnNames)
            {
                col++;
                var n = RumNumbers.GetColData<double>(col);
                if (n.Exists(v => v > 0)) RumIDs.Add(col);
            }
        }                   

        /// <summary>
        /// Map the IAT parameters to their CLEM counterpart
        /// </summary>
        public void SetParameters(RuminantType ruminant)
        {                             
            foreach(var map in Maps)
            {                
                // Find the subtable which contains the parameter
                var table = this.GetType().GetProperty(map.Table).GetValue(this, null) as SubTable;

                // Find the row which contains the parameter (if it exists)
                int row = table.RowNames.FindIndex(s => s == map.ParamIAT);
                if (row < 0) continue;

                // Convert the value of the parameter to CLEM
                double value = table.GetData<double>(row, RumActiveID) * map.Proportion;

                // Set the value of the CLEM parameter
                ruminant.GetType().GetProperty(map.ParamCLEM).SetValue(ruminant, value);
            }                
        }        

        /// <summary>
        /// Model all present ruminant breeds
        /// </summary>
        public IEnumerable<RuminantType> GetRuminants(RuminantHerd parent)
        {          
            List<RuminantType> ruminants = new List<RuminantType>();

            // Iterate over all the present breeds
            foreach (int id in RumIDs)
            {
                RumActiveID = id;

                string breed = RumNumbers.ColumnNames[id].Replace(".", "");

                RuminantType ruminant = new RuminantType(parent)
                {
                    Name = breed,
                    Breed = breed
                };
                SetParameters(ruminant);

                ruminants.Add(ruminant);
            }

            return ruminants;
        }

        /// <summary>
        /// Model all present cohorts of a given ruminant breed
        /// </summary>
        public IEnumerable<RuminantTypeCohort> GetCohorts(RuminantInitialCohorts parent)
        {
            List<RuminantTypeCohort> cohorts = new List<RuminantTypeCohort>();

            int row = -1;

            foreach (string cohort in RumNumbers.RowNames)
            {
                row++;
                if (RumNumbers.GetData<string>(row, RumActiveID) != "0")
                {
                    // Check gender
                    int gender = 0;
                    if (cohort.Contains("F")) gender = 1;

                    // Check suckling
                    bool suckling = false;
                    if (cohort.Contains("Calf")) suckling = true;

                    // Check breeding sire
                    bool sire = false;
                    if (cohort.Contains("ires")) sire = true;

                    cohorts.Add(new RuminantTypeCohort(parent)
                    {
                        Name = cohort,
                        Gender = gender,
                        Age = (int)Math.Ceiling(RumAges.GetData<double>(row, RumActiveID)),
                        Number = (int)Math.Ceiling(RumNumbers.GetData<double>(row, RumActiveID)),
                        Weight = RumWeights.GetData<double>(row, RumActiveID),
                        Suckling = suckling,
                        Sire = sire
                    });
                }
            }

            return cohorts;
        }

        /// <summary>
        /// Model the price of each present cohort for a given breed
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<AnimalPriceGroup> GetAnimalPrices(AnimalPricing parent)
        {
            List<AnimalPriceGroup> prices = new List<AnimalPriceGroup>();

            double sire_price = 0;
            int row = -1;
            foreach (string cohort in RumNumbers.RowNames)
            {
                row++;
                if (RumNumbers.GetData<double>(row, RumActiveID) != 0)
                {
                    if (!cohort.ToLower().Contains("sire")) sire_price = RumPrices.GetData<double>(row, RumActiveID);

                    var group = new AnimalPriceGroup(parent)
                    {
                        Name = cohort,
                        Value = RumPrices.GetData<double>(row, RumActiveID)
                    };

                    // Filter cohort based on gender
                    group.Add(new RuminantFilter(group)
                    { 
                        Name = "GenderFilter",
                        Parameter = 2,
                        Value = cohort.ToLower().Contains("f") ? "Female" : "Male"
                    });

                    // Filter cohort based on age
                    group.Add(new RuminantFilter(group)
                    {
                        Name = "AgeFilter",
                        Parameter = 3,
                        Operator = 5,
                        Value = RumAges.GetData<string>(row, RumActiveID)
                    });

                    prices.Add(group);
                }
            }
            parent.SirePrice = sire_price;

            return prices;
        }

        /// <summary>
        /// Model the management activities for each breed
        /// </summary>
        public IEnumerable<ActivityFolder> GetManageBreeds(ActivityFolder herd)
        {
            List<ActivityFolder> breeds = new List<ActivityFolder>();

            foreach (int id in RumIDs)
            {
                // Add a new folder for individual breed
                ActivityFolder breed = new ActivityFolder(herd)
                {
                    Name = "Manage " + RumSpecs.ColumnNames[id]
                };                

                // Manage breed numbers
                RuminantActivityManage numbers = new RuminantActivityManage(breed)
                {
                    MaximumBreedersKept = RumSpecs.GetData<int>(2, id),
                    MinimumBreedersKept = RumSpecs.GetData<int>(38, id),
                    MaximumBreedingAge = RumSpecs.GetData<int>(3, id),
                    MaximumBullAge = RumSpecs.GetData<double>(25, id),
                    MaleSellingAge = RumSpecs.GetData<double>(5, id),
                    MaleSellingWeight = RumSpecs.GetData<double>(6, id)
                };

                numbers.Add(new ActivityTimerInterval(numbers)
                {
                    Name = "NumbersTimer",
                    Interval = 12,
                    MonthDue = 12
                });
                breed.Add(numbers);

                // Manage breed weaning
                breed.Add(new RuminantActivityWean(breed)
                {
                    WeaningAge = RumSpecs.GetData<double>(7, id),
                    WeaningWeight = RumSpecs.GetData<double>(8, id)
                });

                // Manage breed milking
                if (RumSpecs.GetData<double>(18, id) > 0) breed.Add(new RuminantActivityMilking(breed)
                {
                    Name = "Milk",
                    ResourceTypeName = "HumanFoodStore." + RumSpecs.ColumnNames[id] + "_Milk"
                });

                // Manage sale of dry breeders
                breed.Add(new RuminantActivitySellDryBreeders(breed)
                {
                    MonthsSinceBirth = RumSpecs.GetData<double>(32, id),
                    ProportionToRemove = RumSpecs.GetData<double>(4, id) * 0.01
                });
                breeds.Add(breed);
            }

            return breeds;
        }

    }
}