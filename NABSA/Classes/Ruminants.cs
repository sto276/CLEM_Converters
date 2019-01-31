using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NABSA
{   
    using static Queries;
    static class Ruminants
    {
        /// <summary>
        /// A List containing all breeds present in the current set of NABSA data
        /// </summary>
        private static List<string> present = new List<string>();

        /// <summary>
        /// Constructs the ruminants section
        /// </summary>
        /// <param name="nabsa">Source NABSA data</param>
        public static XElement GetRuminants(XElement nabsa)
        {
            XElement herd = GetHerd(nabsa);

            // Adds any existing cohorts to each breed in the herd
            AddCohortsToHerd(nabsa, herd);

            // Searches through the herd and removes any breed which doesn't pass the validation test
            herd.Elements().Skip(1).Where(b => !ValidateBreed(nabsa, b)).Remove();

            herd.Add(new XElement("IncludeInDocumentation", "true"));
            return herd;
        }

        /// <summary>
        /// Finds all available ruminant breeds and builds an XML structure to store breed data in
        /// </summary>
        /// <param name="nabsa">Source NABSA data</param>
        private static XElement GetHerd(XElement nabsa)
        {
            // Names of the breeds are stored in the "SuppAllocs" table under the element "ColumnNames" 
            XElement allocs = FindFirst(nabsa, "SuppAllocs");
            var breeds = GetElementValues(allocs.Element("ColumnNames"));

            // Parent element of herd data
            XElement herd = new XElement
            (
                "RuminantHerd",
                new XElement("Name", "Ruminants")
            );

            // Initial Cohorts XML base
            XElement cohorts = new XElement
            (
                "RuminantInitialCohorts",
                new XElement("Name", "InitialCohorts")
            );

            // Animal Pricing XML base
            XElement pricing = new XElement
            (
                "AnimalPricing",
                new XElement("Name", "Prices"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("PricingStyle", "perKg")
            );

            // Iterate over all breeds, adding cohorts and pricing to each
            foreach (string breed in breeds)
            {
                XElement type = new XElement
                (
                    "RuminantType",
                    new XElement("Name", breed),
                    new XElement("Breed", breed),
                    cohorts,
                    pricing,
                    new XElement("IncludeInDocumentation", "true")
                );
                herd.Add(type);
            }

            return herd;
        }

        /// <summary>
        /// Searches the source data for all possible cohort data.
        /// Adds any data found to the parent node.
        /// </summary>
        /// <param name="nabsa">Source NABSA data</param>
        /// <param name="herd">Parent node to add data to</param>
        private static void AddCohortsToHerd(XElement nabsa, XElement herd)
        {
            // Search for the data tables in the source
            XElement nums = FindByName(nabsa, "Startup ruminant numbers");
            XElement ages = FindByName(nabsa, "Startup ruminant ages");
            XElement weights = FindByName(nabsa, "Startup ruminant weights");
            XElement prices = FindByName(nabsa, "Ruminant prices");

            // Names of each cohort
            var cohorts = GetElementNames(nums).Skip(1);

            foreach (string cohort in cohorts)
            {
                // Find the data for a given cohort in each table
                var nvalues = GetElementValues(nums.Element(cohort));
                var avalues = GetElementValues(ages.Element(cohort));
                var wvalues = GetElementValues(weights.Element(cohort));
                var pvalues = GetElementValues(prices.Element(cohort));

                // Look at the current cohort data for each breed
                foreach (var breed in herd.Elements().Skip(1))
                {
                    int index = breed.ElementsBeforeSelf().Count() - 1;

                    // Sire price is required even if there are no sires present,
                    // so it must be evaluated before the 'continue'
                    string price = pvalues.ElementAt(index);
                    if (cohort.Contains("ire"))
                    {
                        breed.Element("AnimalPricing").Add(new XElement("BreedingSirePrice", price));
                    }                    

                    // Skip the cohort if it has no members
                    string num = nvalues.ElementAt(index);
                    if (num == "0") continue;

                    // Add the cohort to the parent
                    present.Add(breed.Elements().First().Value);

                    // Find age/weight data
                    string age = avalues.ElementAt(index);
                    string weight = wvalues.ElementAt(index);
                    
                    // Construct the cohort node and add it to the herd
                    XElement type = GetCohort(cohort, age, num, weight);
                    breed.Element("RuminantInitialCohorts").Add(type);

                    // Construct the pricing node and add it to the herd
                    XElement pricing = GetPriceGroup(cohort, age, price);
                    breed.Element("AnimalPricing").Add(pricing);
                }
            }
        }

        /// <summary>
        /// For each cohort category, generate a filter for the pricing
        /// </summary>
        /// <param name="name">Name of the cohort</param>
        /// <param name="age">Average age of the cohort (months)</param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetCohortPricingFilters(string name, string age)
        {
            XElement filters = new XElement("Filters");

            // Construct the gender filter
            string gender = "Male";
            if (name.Contains("F")) gender = "Female";

            filters.Add(GetFilter(gender, "Gender", "Equal", gender));

            // Construct the Age filter
            string descriptor = "";
            string parameter = "Age";
            string comparer = "LessThan";
            string value = ShiftAge(age);

            // Check if the cohort is breeding sires
            if (name.Contains("ire"))
            {
                comparer = "Equal";
                parameter = "BreedingSire";
                value = "True";
            }

            // Check if the cohort is calves
            if (name.Contains("Calf"))
            {
                parameter = "Weaned";
                comparer = "NotEqual";
                value = "True";
            }
            
            descriptor = parameter + comparer + value;
            filters.Add(GetFilter(descriptor, parameter, comparer, value));

            return filters.Elements();
        }

        /// <summary>
        /// Constructs the XML representation of a filter
        /// </summary>
        /// <param name="name">Name of the filter</param>
        /// <param name="parameter">Parameter to find data for</param>
        /// <param name="comparer">Operator that compares the parameter and value</param>
        /// <param name="value">Value that the parameter is compared against</param>
        private static XElement GetFilter(string name, string parameter, string comparer, string value)
        {
            XElement filter = new XElement
            (
                "RuminantFilter",
                new XElement("Name", name),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("Parameter", parameter),
                new XElement("Operator", comparer),
                new XElement("Value", value)
            );

            return filter;
        }

        /// <summary>
        /// Shifts an age older by 6 months. 
        /// </summary>
        /// <param name="age">Age to shift</param>
        private static string ShiftAge(string age)
        {
            int.TryParse(age, out int n);
            n += 6;
            return n.ToString();
        }

        /// <summary>
        /// Construct the XML for a single cohort
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static XElement GetCohort(string name, string age, string number, string weight)
        {
            // Find the gender of the cohort
            string gender = "Male";
            if (name.Contains("F")) gender = "Female";

            // Check if the cohort is suckling
            string suckling = "false";
            if (name.Contains("Calf")) suckling = "true";

            // Check if the cohort is a breeding sire
            string sire = "false";
            if (name.Contains("ire")) sire = "true";

            XElement cohort = new XElement
            (
                "RuminantTypeCohort",
                new XElement("Name", name),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("Gender", gender),
                new XElement("Age", age),
                new XElement("Number", number),
                new XElement("Weight", weight),
                new XElement("WeightSD", "0"),
                new XElement("Suckling", suckling),
                new XElement("Sire", sire)
            );

            return cohort;
        }

        /// <summary>
        /// Construct the XML for a single price group
        /// </summary>
        private static XElement GetPriceGroup(string name, string age, string price)
        {
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XAttribute xa = new XAttribute(xsi + "type", "AnimalPriceGroup");

            XElement pricegroup = new XElement
            (
                "Model",
                xa,
                new XElement("Name", name),
                GetCohortPricingFilters(name, age),                
                new XElement("IncludeInDocumentation", "true"),
                new XElement("PurchaseValue", price),
                new XElement("SellValue", price)
            );

            return pricegroup;
        }

        /// <summary>
        /// Checks if a breed has present cohorts, and adds the breed parameters if it does
        /// </summary>
        /// <param name="nabsa">Source NABSA data</param>
        /// <param name="breed">Breed data to be validated</param>
        /// <returns></returns>
        private static bool ValidateBreed(XElement nabsa, XElement breed)
        {
            XElement cohorts = breed.Element("RuminantInitialCohorts");
            
            // A breed requires cohorts to be considered valid
            int count = cohorts.Elements().Count();
            if (count > 1)
            {
                // Invalid breeds are removed, so parameters are added now
                // to reduce complexity of indexing, as opposed to invoking
                // a separate method later
                int index = breed.ElementsBeforeSelf().Count() - 1;

                // A validated breed needs to add parameters
                var parameters = GetParameters(nabsa, index);
                breed.Add(parameters);

                return true;
            }
            else
            {
                return false;
            }            
        }        

        /// <summary>
        /// Constructs the XML form for the specifications and coefficients of a given breed
        /// </summary>
        /// <param name="nabsa">Source NABSA data</param>
        /// <param name="i">Breed id</param>
        private static IEnumerable<XElement> GetParameters(XElement nabsa, int i)
        {
            XElement parameters = new XElement("Parameters");

            StreamReader csv = new StreamReader($"{Directory.GetCurrentDirectory()}/Resources/csv/Params_NABSA.csv");
            string line = null;

            // Skip the head line
            csv.ReadLine();
            while ((line = csv.ReadLine()) != null)
            {
                // Determine which data to look for
                string[] data = line.Split(',');
                XElement item = FindFirst(nabsa, data[0]);

                // Location of specification value depends on the format of the data in the NABSA file
                string spec;
                if (item.HasElements) spec = item.Elements().ElementAt(i).Value;
                else spec = item.Value;

                // Convert data format and add to the element
                double.TryParse(spec, out double value);
                double.TryParse(data[2], out double ratio);
                parameters.Add(new XElement($"{data[1]}", $"{value * ratio}"));
            }

            // Write in data exclusive to CLEM with default values
            parameters.Add(new XElement("ProportionOfMaxWeightToSurvive", "0.5"));
            parameters.Add(new XElement("MaximumMaleMatingsPerDay", "30"));
            parameters.Add(new XElement("ProteinDegradability", "0.9"));

            return parameters.Elements();
        }
                
        /// <summary>
        /// Construct the Ruminant Manage XML
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        public static XElement Manage(XElement nabsa, XElement resources)
        {
            XElement manage = new XElement
            (
                "ActivityFolder",
                new XElement("Name", $"Manage herd"),
                ManageBreeds(nabsa),
                GetCutAndCarry(nabsa, resources),
                GetGrazeAll(nabsa),
                GetSuppCommonLand(),
                GetFeed(nabsa),
                GetGrow(),
                GetBreed(),
                GetBuySell(),
                GetMuster(),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop")
            );

            return manage;
        }

        /// <summary>
        /// Create an XML activity folder to manage the activities of a specific breed
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> ManageBreeds(XElement nabsa)
        {
            XElement breeds = new XElement("Breeds");

            // Find the data in the NABSA file
            XElement specs = FindByName(nabsa, "Ruminant specifications");
            XElement allocs = FindFirst(nabsa, "SuppAllocs");

            var names = GetElementValues(allocs.Element("ColumnNames"));

            int i = -1;
            foreach (string n in names)
            {
                i++;                
                
                // If the breed isn't listed as present, skip over it
                if (!present.Exists(s => s == n)) continue;

                // Sanitise the name for ApsimX
                string name = n.Replace(".", " ");

                // Check if any milk is used for home consumption
                XElement milking = null;
                string homemilk = specs.Element("Home_milk").Elements().ElementAt(i).Value;
                if (homemilk != "0") milking = GetMilking(name);

                XElement breed = new XElement
                (
                    "ActivityFolder",
                    new XElement("Name", $"Manage {name}"),
                    GetWean(nabsa, i),
                    milking,
                    GetNumbers(nabsa, i),
                    GetSellDry(nabsa, i)
                );

                breeds.Add(breed);
            }

            return breeds.Elements();
        }

        /// <summary>
        /// Writes the 'Wean' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetWean(XElement nabsa, int col)
        {
            XElement specs = FindByName(nabsa, "Ruminant specifications");

            XElement wean = new XElement
            (
                "RuminantActivityWean",
                new XElement("Name", "Wean"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("WeaningAge", specs.Element("Weaning_age").Elements().ElementAt(col).Value),
                new XElement("WeaningWeight", specs.Element("Weaning_weight").Elements().ElementAt(col).Value),
                new XElement("GrazeFoodStoreName", "NativePasture")
            );
            return wean;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static XElement GetMilking(string name)
        {
            XElement milking = new XElement
            (
                "RuminantActivityMilking",
                new XElement("Name", "Milk"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("ResourceTypeName", $"HumanFoodStore.Milk_{name}")
            );
            return milking;
        }

        /// <summary>
        /// Writes the 'Manage numbers' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetNumbers(XElement nabsa, int col)
        {
            XElement specs = FindByName(nabsa, "Ruminant specifications");

            XElement timer = new XElement
            (
                "ActivityTimerInterval",
                new XElement("Name", "ManageHerdTimer"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("MonthDue", "12")
            );

            XElement numbers = new XElement
            (
                "RuminantActivityManage",
                new XElement("Name", "Manage numbers"),
                timer,
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("MaximumBreedersKept", specs.Element("Max_breeders").Elements().ElementAt(col).Value),
                new XElement("MinimumBreedersKept", "0"),
                new XElement("MaximumBreederAge", specs.Element("Max_breeder_age").Elements().ElementAt(col).Value),
                new XElement("MaximumBreedersPerPurchase", "1"),
                new XElement("MaximumSiresKept", "0"),
                new XElement("MaximumBullAge", specs.Element("Max_Bull_age").Elements().ElementAt(col).Value),
                new XElement("AllowSireReplacement", "false"),
                new XElement("MaximumSiresPerPurchase", "0"),
                new XElement("MaleSellingAge", specs.Element("Anim_sell_age").Elements().ElementAt(col).Value),
                new XElement("MaleSellingWeight", specs.Element("Anim_sell_wt").Elements().ElementAt(col).Value),
                new XElement("GrazeFoodStoreName", "GrazeFoodStore.NativePasture"),
                new XElement("MinimumPastureBeforeRestock", "0"),
                new XElement("SellFemalesLikeMales", "false"),
                new XElement("ContinuousMaleSales", "false")
            );

            return numbers;
        }

        /// <summary>
        /// Construct the XML for the CutAndCarry element
        /// </summary>
        /// <param name="nabsa"></param>
        private static XElement GetCutAndCarry(XElement nabsa, XElement resources)
        {
            XElement cutcarry = new XElement
            (
                "ActivityFolder",
                new XElement("Name", "Cut and carry"),
                GetCutAndCarryFeeds(nabsa, resources),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop")
            );
            return cutcarry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetCutAndCarryFeeds(XElement nabsa, XElement resources)
        {                      
            XElement feeds = new XElement("Feeds");

            XElement foodstore = FindByName(resources, "AnimalFoodStore");
            foreach (var store in foodstore.Elements("AnimalFoodStoreType"))
            {
                string name = store.Element("Name").Value;
                XElement feed = new XElement
                (
                    "RuminantActivityFeed",
                    new XElement("Name", $"Feed {name}"),
                    GetFeedGroups(nabsa),
                    Labour.GetLabourRequirement(),
                    new XElement("IncludeInDocumentation", "true"),
                    new XElement("OnPartialResourcesAvailableAction", "UseResourcesAvailable"),
                    new XElement("HerdFilters", ""),
                    new XElement("FeedTypeName", $"AnimalFoodStore."),
                    new XElement("ProportionTramplingWastage", "0.3"),
                    new XElement("FeedStyle", "ProportionOfPotentialIntake")
                );
                feeds.Add(feed);
            }

            return feeds.Elements();
        }

        /// <summary>
        /// Construct the FeedGroup XML for each present breed
        /// </summary>
        /// <param name="breed"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetFeedGroups(XElement nabsa)
        {
            XElement groups = new XElement("Groups");

            foreach (string breed in present)
            {
                XElement group = new XElement
                (
                    "RuminantFeedGroup",
                    new XElement("Name", breed),
                    new XElement("IncludeInDocumentation", "true")
                );
            }

            return groups.Elements();
        }

        /// <summary>
        /// Writes the 'Graze All' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetGrazeAll(XElement nabsa)
        {
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XAttribute xa = new XAttribute(xsi + "type", "RuminantActivityGrazeAll");

            XElement grazeall = new XElement
            (
                "Model",
                xa,
                new XElement("Name", "RuminantActivityGrazeAll"),
                GetLabourRequirement(),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "UseResourcesAvailable"),
                new XElement("HerdFilters", null),
                new XElement("HoursGrazed", "8")
            );

            return grazeall;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static XElement GetLabourRequirement()
        {
            XElement requirement = new XElement
            (
                "LabourRequirement",
                new XElement("Name", "LabourRequirement"),
                Labour.GetLabourGroup(),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("LabourPerUnit", "4"),
                new XElement("UnitSize", "1"),
                new XElement("WholeUnitBlocks", "false"),
                new XElement("UnitType", "perAE"),
                new XElement("MinimumPerPerson", "0"),
                new XElement("MaximumPerPerson", "30"),
                new XElement("LabourShortfallAffectsActivity", "false")
            );

            return requirement;
        }

        /// <summary>
        /// Writes the 'Supp common land' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetSuppCommonLand()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static XElement GetFeed(XElement nabsa)
        {
            return null;
        }

        /// <summary>
        /// Writes the 'Grow' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetGrow()
        {
            XElement grow = new XElement
            (
                "RuminantActivityGrow",
                new XElement("Name", "Grow all ruminants"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "UseResourcesAvailable")
            );
            return grow;
        }

        /// <summary>
        /// Writes the 'Breed' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetBreed()
        {
            XElement breed = new XElement
            (
                "RuminantActivityBreed",
                new XElement("Name", "Breed"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("MaximumConceptionRateUncontrolled", "0.8"),
                new XElement("UseAI", "true")
            );
            return breed;
        }
                
        /// <summary>
        /// Writes the 'Buy/Sell' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetBuySell()
        {
            XElement buysell = new XElement
            (
                "RuminantActivityBuySell",
                new XElement("Name", "RuminantActivityBuySell"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("BankAccountName", "Finances.Bank")
            );
            return buysell;
        }

        /// <summary>
        /// Writes the 'Sell Dry' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetSellDry(XElement nabsa, int col)
        {
            XElement specs = FindByName(nabsa, "Ruminant specifications");

            string value = specs.Element("Dry_breeder_cull_rate").Elements().ElementAt(col).Value;
            double.TryParse(value, out double cullrate);
            cullrate *= 0.01;

            XElement selldry = new XElement
            (
                "RuminantActivitySellDryBreeders",
                new XElement("Name", "RuminantActivitySellDryBreeders"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("MinimumConceptionBeforeSell", "1"),
                new XElement("MonthsSinceBirth", specs.Element("Joining_age").Elements().ElementAt(col).Value),
                new XElement("ProportionToRemove", cullrate.ToString())
            );

            return selldry;
        }

        /// <summary>
        /// Writes the 'Ruminant Muster' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetMuster()
        {
            XElement muster = new XElement
            (
                "RuminantActivityMuster",
                new XElement("Name", "RuminantActivityMuster"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("ManagedPastureName", "GrazeFoodStore.NativePasture"),
                new XElement("PerformAtStartOfSimulation", "true"),
                new XElement("MoveSucklings", "true")
            );
            return muster;
        }

    }
}
