namespace NABSA
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    static class Ruminants
    {
        private static List<string> present = new List<string>();

        /// <summary>
        /// Constructs the ruminants section
        /// </summary>
        /// <param name="nabsa">Source NABSA</param>
        public static XElement GetRuminants(XElement nabsa)
        {
            XElement herd = GetHerd(nabsa);
            AddCohortsToHerd(nabsa, herd);

            // Removes any breed which doesn't pass the validation test
            herd.Elements().Skip(1).Where(b => !ValidateBreed(nabsa, b)).Remove();

            herd.Add(new XElement("IncludeInDocumentation", "true"));
            return herd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static XElement GetHerd(XElement nabsa)
        {
            // Names of the breeds are stored in the "SuppAllocs" table under the element "ColumnNames" 
            XElement allocs = Queries.FindFirst(nabsa, "SuppAllocs");
            var breeds = Queries.GetElementValues(allocs.Element("ColumnNames"));

            XElement herd = new XElement
            (
                "RuminantHerd",
                new XElement("Name", "Ruminants")
            );

            // Iterate over all breeds
            foreach (string breed in breeds)
            {
                //string name = breed.Replace('.', ' ');
                string name = breed;
                XElement type = new XElement
                (
                    "RuminantType",
                    new XElement("Name", name),
                    new XElement("Breed", name),
                    new XElement
                    (
                        "RuminantInitialCohorts",
                        new XElement("Name", "InitialCohorts")
                    ),
                    new XElement
                    (
                        "AnimalPricing",
                        new XElement("Name", "Prices"),
                        new XElement("IncludeInDocumentation", "true"),
                        new XElement("PricingStyle", "perKg")
                    ),
                    new XElement("IncludeInDocumentation", "true")
                );
                herd.Add(type);
            }

            return herd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <param name="herd"></param>
        private static void AddCohortsToHerd(XElement nabsa, XElement herd)
        {
            XElement nums = Queries.FindByName(nabsa, "Startup ruminant numbers");
            XElement ages = Queries.FindByName(nabsa, "Startup ruminant ages");
            XElement weights = Queries.FindByName(nabsa, "Startup ruminant weights");
            XElement prices = Queries.FindByName(nabsa, "Ruminant prices");

            // Names of each cohort
            var cohorts = Queries.GetElementNames(nums).Skip(1);
            foreach (string cohort in cohorts)
            {
                var nvalues = Queries.GetElementValues(nums.Element(cohort));
                var avalues = Queries.GetElementValues(ages.Element(cohort));
                var wvalues = Queries.GetElementValues(weights.Element(cohort));
                var pvalues = Queries.GetElementValues(prices.Element(cohort));

                foreach (var breed in herd.Elements().Skip(1))
                {
                    int index = breed.ElementsBeforeSelf().Count() - 1;

                    string price = pvalues.ElementAt(index);
                    if (cohort.Contains("ire"))
                    {
                        breed.Element("AnimalPricing").Add(new XElement("BreedingSirePrice", price));
                    }                    

                    string num = nvalues.ElementAt(index);
                    if (num == "0") continue;

                    present.Add(breed.Elements().First().Value);

                    string age = avalues.ElementAt(index);
                    string weight = wvalues.ElementAt(index);
                    
                    XElement type = GetCohort(cohort, age, num, weight);
                    breed.Element("RuminantInitialCohorts").Add(type);

                    XElement pricing = GetPriceGroup(cohort, age, price);
                    breed.Element("AnimalPricing").Add(pricing);
                }
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetFilters(string name, string age)
        {
            XElement filters = new XElement("Filters");

            // Find the gender filter
            string gender = "Male";
            if (name.Contains("F")) gender = "Female";

            filters.Add(GetFilter(gender, "Gender", "Equal", gender));

            // Find the Age filter

            string descriptor = "";
            string parameter = "Age";
            string comparer = "LessThan";
            string value = age;

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
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameter"></param>
        /// <param name="comparer"></param>
        /// <param name="value"></param>
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
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        private static XElement GetPriceGroup(string name, string age, string price)
        {
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XAttribute xa = new XAttribute(xsi + "type", "AnimalPriceGroup");

            XElement pricegroup = new XElement
            (
                "Model",
                xa,
                new XElement("Name", name),
                GetFilters(name, age),                
                new XElement("IncludeInDocumentation", "true"),
                new XElement("PurchaseValue", price),
                new XElement("SellValue", price)
            );

            return pricegroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <param name="herd"></param>
        //private static void AddParametersToHerd(XElement nabsa, XElement herd)
        //{
        //    int j = -1;
        //    foreach (var breed in herd.Elements().Skip(1))
        //    {
        //        j++;
        //        int count = breed.Element("RuminantInitialCohorts").Elements().Count();
        //        if (count > 0)
        //        {
        //            var parameters = GetParameters(nabsa, j);
        //            breed.Add(parameters);
        //        }
        //        else
        //        {
        //            breed.Remove();
        //            breed.
        //        }
        //    }

        //    return;
        //}

        private static bool ValidateBreed(XElement nabsa, XElement breed)
        {
            XElement cohorts = breed.Element("RuminantInitialCohorts");
            // A breed needs to have cohorts to be considered valid
            int count = cohorts.Elements().Count();
            if (count > 1)
            {
                // Note that ElementsBeforeSelf.Count() still considers siblings
                // that were skipped
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
        /// Writes the specifications and coefficients for a given breed to CLEM
        /// </summary>
        /// <param name="nabsa">Source NABSA</param>
        /// <param name="i">Breed identifier</param>
        private static IEnumerable<XElement> GetParameters(XElement nabsa, int i)
        {
            XElement parameters = new XElement("Parameters");

            StreamReader csv = new StreamReader("params.csv");
            string line = null;

            // Skips the header
            csv.ReadLine();
            while ((line = csv.ReadLine()) != null)
            {
                string[] data = line.Split(',');
                XElement item = Queries.FindFirst(nabsa, data[0]);
                string s;
                if (item.HasElements) s = item.Elements().ElementAt(i).Value;
                else s = item.Value;

                double.TryParse(s, out double value);
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
        /// Writes the 'Ruminant Manage' activity section
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        public static XElement Manage(XElement nabsa)
        {
            // Don't need to do anything if no ruminants are present
            //if (nabsa.ruminants.Count == 0) return null;

            //Section: ActivityFolder - Manage herd
            XElement manage = new XElement
            (
                "ActivityFolder",
                new XElement("Name", $"Manage herd"),
                ManageBreeds(nabsa),
                GetCutAndCarry(nabsa),
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
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> ManageBreeds(XElement nabsa)
        {
            XElement breeds = new XElement("Breeds");

            XElement specs = Queries.FindByName(nabsa, "Ruminant specifications");

            XElement allocs = Queries.FindFirst(nabsa, "SuppAllocs");
            var names = Queries.GetElementValues(allocs.Element("ColumnNames"));

            int i = -1;
            foreach (string name in names)
            {
                i++;
                if (!present.Exists(s => s == name)) continue;

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
        /// Writes the 'Cut and carry' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa"></param>
        private static XElement GetCutAndCarry(XElement nabsa)
        {
            // Checks the feeding system for each breed, and adds cut & carry if appropriate
            XElement cutcarry = new XElement
            (
                "ActivityFolder",
                new XElement("Name", "Cut and carry"),
                GetCCFeeds(nabsa),
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
        private static IEnumerable<XElement> GetCCFeeds(XElement nabsa)
        {
            return null;

            XElement feeds = new XElement("feeds");

            //foreach (int pool in nabsa.pools.Keys)
            {
                XElement feed = new XElement
                (
                    "RuminantActivityFeed",
                    new XElement("Name", $"Feed "),
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
        /// 
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
                    new XElement("IncludeInDocumentation", "false")
                );
            }

            return groups.Elements();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="breed"></param>
        /// <returns></returns>
        private static XElement GetRuminantFilter(string breed)
        {
            XElement filter = new XElement
            (
                "RuminantFilter",
                new XElement("Name", "RuminantFilter"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("Parameter", "Breed"),
                new XElement("Operator", "Equal"),
                new XElement("Value", breed)
            );

            return filter;
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
        /// Writes the 'Wean' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="nabsa">Source XElement</param>
        private static XElement GetWean(XElement nabsa, int col)
        {
            XElement specs = Queries.FindByName(nabsa, "Ruminant specifications");

            XElement wean = new XElement
            (
                "RuminantActivityWean",
                new XElement("Name", "Wean"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("WeaningAge", specs.Element("Weaning_age").Elements().ElementAt(col).Value),
                new XElement("WeaningWeight", specs.Element("Weaning_weight").Elements().ElementAt(col).Value)
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
            XElement specs = Queries.FindByName(nabsa, "Ruminant specifications");

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
                new XElement("GrazeFoodStoreName", "Not specified - general yards"),
                new XElement("MinimumPastureBeforeRestock", "0"),
                new XElement("SellFemalesLikeMales", "false"),
                new XElement("ContinuousMaleSales", "false")
            );

            return numbers;
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
            XElement specs = Queries.FindByName(nabsa, "Ruminant specifications");

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
