using Models.CLEM.Activities;
using Models.CLEM.Groupings;
using Models.CLEM.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Reader
{   
    public partial class NABSA
    {
        private void SetParameters(RuminantType ruminant)
        {
            List<Tuple<string, string, double>> parameters = new List<Tuple<string, string, double>>()
            {
                new Tuple<string, string, double>("concep_rate_assym", "ConceptionRateAsymptote", 1),
                new Tuple<string, string, double>("concep_rate_coeff", "ConceptionRateCoefficient", 1),
                new Tuple<string, string, double>("concep_rate_incpt", "ConceptionRateIntercept", 1),
                new Tuple<string, string, double>("birth_SRW", "SRWBirth", 1),
                new Tuple<string, string, double>("cashmere_coeff", "CashmereCoefficient", 1),
                new Tuple<string, string, double>("Critical_cow_wt", "CriticalCowWeight", 0.01),
                new Tuple<string, string, double>("grwth_coeff1", "AgeGrowthRateCoefficient", 1),
                new Tuple<string, string, double>("grwth_coeff2", "SRWGrowthScalar", 1),
                new Tuple<string, string, double>("intake_coeff", "IntakeCoefficient", 1),
                new Tuple<string, string, double>("intake_incpt", "IntakeIntercept", 1),
                new Tuple<string, string, double>("IPI_coeff", "InterParturitionIntervalCoefficient", 1),
                new Tuple<string, string, double>("IPI_incpt", "InterParturitionIntervalIntercept", 1),
                new Tuple<string, string, double>("Joining_age", "MinimumAge1stMating", 1),
                new Tuple<string, string, double>("Joining_size", "MinimumSize1stMating", 0.01),
                new Tuple<string, string, double>("juvenile_mort_coeff", "JuvenileMortalityCoefficient", 1),
                new Tuple<string, string, double>("juvenile_mort_exp", "JuvenileMortalityExponent", 1),
                new Tuple<string, string, double>("juvenile_mort_max", "JuvenileMortalityMaximum", 0.01),
                new Tuple<string, string, double>("kg_coeff", "EGrowthEfficiencyCoefficient", 1),
                new Tuple<string, string, double>("kg_incpt", "EGrowthEfficiencyIntercept", 1),
                new Tuple<string, string, double>("kl_coeff", "ELactationEfficiencyCoefficient", 1),
                new Tuple<string, string, double>("kl_incpt", "ELactationEfficiencyIntercept", 1),
                new Tuple<string, string, double>("km_coeff", "EMaintEfficiencyCoefficient", 1),
                new Tuple<string, string, double>("km_incpt", "EMaintEfficiencyIntercept", 1),
                new Tuple<string, string, double>("kme", "Kme", 1),
                new Tuple<string, string, double>("Milk_Curve_nonsuck", "MilkCurveNonSuckling", 1),
                new Tuple<string, string, double>("Milk_Curve_suck", "MilkCurveSuckling", 1),
                new Tuple<string, string, double>("Milk_end", "MilkingDays", 30),
                new Tuple<string, string, double>("Milk_intake_coeff", "MilkIntakeCoefficient", 1),
                new Tuple<string, string, double>("Milk_intake_incpt", "MilkIntakeIntercept", 1),
                new Tuple<string, string, double>("Milk_max", "MilkPeakYield", 1),
                new Tuple<string, string, double>("Milk_offset_day", "MilkOffsetDay", 1),
                new Tuple<string, string, double>("Milk_Peak_day", "MilkPeakDay", 1),
                new Tuple<string, string, double>("Mortality_base", "MortalityBase", 0.01),
                new Tuple<string, string, double>("protein_coeff", "ProteinCoefficient", 1),
                new Tuple<string, string, double>("Rum_gest_int", "GestationLength", 1),
                new Tuple<string, string, double>("SRW", "SRWFemale", 1),
                new Tuple<string, string, double>("Twin_rate", "TwinRate", 1),
                new Tuple<string, string, double>("wool_coeff", "WoolCoefficient", 1)
            };

            int index = Breeds.IndexOf(ruminant.Breed);
            
            foreach (var parameter in parameters)
            {
                double value = GetValue<double>(FindFirst(Source, parameter.Item1), index) * parameter.Item3;
                ruminant.GetType().GetProperty(parameter.Item2).SetValue(ruminant, value);
            }
        }

        public IEnumerable<RuminantType> GetRuminants(RuminantHerd herd)
        {
            List<RuminantType> types = new List<RuminantType>();

            // Iterate over all breeds, adding cohorts and pricing to each
            foreach (string breed in PresentBreeds)
            {
                RuminantType type = new RuminantType(herd, breed);
                SetParameters(type);
                types.Add(type);
            }

            return types;
        }

        public IEnumerable<RuminantTypeCohort> GetCohorts(RuminantInitialCohorts initials)
        {
            List<RuminantTypeCohort> list = new List<RuminantTypeCohort>();

            int index = Breeds.IndexOf((initials.Parent as RuminantType).Breed);
            var cohorts = GetElementNames(Numbers).Skip(1);

            foreach (string cohort in cohorts)
            {
                double num = GetValue<double>(Numbers.Element(cohort), index);
                if (num <= 0) continue;

                list.Add(new RuminantTypeCohort(initials)
                {
                    Name = cohort,
                    Number = num,
                    Age = GetValue<int>(Ages.Element(cohort), index),
                    Weight = GetValue<double>(Weights.Element(cohort), index),
                    Gender = cohort.Contains("F") ? 1 : 0,
                    Suckling = cohort.Contains("Calf") ? true : false,
                    Sire = cohort.Contains("ire") ? true : false
                });
            }

            return list.AsEnumerable();
        }       
        
        public IEnumerable<ActivityFolder> GetManageBreeds(ActivityFolder folder)
        {
            List<ActivityFolder> folders = new List<ActivityFolder>();

            foreach (string breed in PresentBreeds)
            {
                string name = breed.Replace(".", " ");
                int index = Breeds.IndexOf(breed);

                ActivityFolder manage = new ActivityFolder(folder)
                {
                    Name = name
                };

                manage.Add(new RuminantActivityWean(manage)
                {
                    WeaningAge = GetValue<double>(RumSpecs.Element("Weaning_age"), index),
                    WeaningWeight = GetValue<double>(RumSpecs.Element("Weaning_weight"), index),
                    GrazeFoodStoreName = "NativePasture"
                });

                string homemilk = GetValue<string>(RumSpecs.Element("Home_milk"), index);
                if (homemilk != "0")
                {                    
                    manage.Add(new RuminantActivityMilking(manage)
                    {
                        ResourceTypeName = "HumanFoodStore." + name + "_Milk" 
                    });
                }

                manage.Add(new RuminantActivityManage(manage)
                {
                    MaximumBreedersKept = GetValue<int>(RumSpecs.Element("Max_breeders"), index),
                    MaximumBreedingAge = GetValue<int>(RumSpecs.Element("Max_breeder_age"), index),
                    MaximumBullAge = GetValue<int>(RumSpecs.Element("Max_Bull_age"), index),
                    MaleSellingAge = GetValue<int>(RumSpecs.Element("Anim_sell_age"), index),
                    MaleSellingWeight = GetValue<int>(RumSpecs.Element("Anim_sell_wt"), index),
                    GrazeFoodStoreName = "GrazeFoodStore.NativePasture"
                });

                manage.Add(new RuminantActivitySellDryBreeders(manage)
                {
                    MinimumConceptionBeforeSell = 1,
                    MonthsSinceBirth = GetValue<int>(RumSpecs.Element("Joining_age"), index),
                    ProportionToRemove = GetValue<double>(RumSpecs.Element("Dry_breeder_cull_rate"), index) * 0.01
                });

                folders.Add(manage);
            }

            return folders;
        }        
 
        public IEnumerable<AnimalPriceGroup> GetAnimalPrices(AnimalPricing pricing)
        {
            List<AnimalPriceGroup> prices = new List<AnimalPriceGroup>();

            int index = Breeds.IndexOf((pricing.Parent as RuminantType).Breed);

            // List of all the present cohorts
            var cohorts = pricing.Parent.Children.First().Children;

            foreach(var cohort in cohorts)
            {
                AnimalPriceGroup price = new AnimalPriceGroup(pricing)
                {
                    Name = cohort.Name,
                    PricingStyle = 1,
                    Value = GetValue<double>(Prices.Element(cohort.Name), index)
                };

                price.Add(new RuminantFilter(price)
                {
                    Name = "GenderFilter",
                    Parameter = 2,
                    Value = (((RuminantTypeCohort)cohort).Gender == 0) ? "Male" : "Female"
                });

                price.Add(new RuminantFilter(price)
                {
                    Name = "AgeFilter",
                    Parameter = 3,
                    Operator = 5,
                    Value = ((RuminantTypeCohort)cohort).Age.ToString()
                });

                prices.Add(price);
            }

            return prices.AsEnumerable();
        }
    }
}
