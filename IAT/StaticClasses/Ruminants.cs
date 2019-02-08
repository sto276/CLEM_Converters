using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace IAT
{
    /// <summary>
    /// Provides methods for reading and converting
    /// the ruminant data contained within an IAT file
    /// </summary>
    class Ruminants
    {
        /// <summary>
        /// Writes the 'Ruminants' resource segment of an .apsimx file
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement GetRuminants(IAT iat)
        {
            IATable numbers = iat.tables["Startup ruminant numbers"];
            IATable ages = iat.tables["Startup ruminant ages"];
            IATable weights = iat.tables["Startup ruminant weights"];            

            XElement herd = new XElement("RuminantHerd");
            herd.Add(new XElement("Name", "RuminantHerd"));

            // Iterate over all the present breeds
            foreach (int col in iat.ruminants)
            {
                string breed = numbers.GetColNames()[col].Replace(".", "");

                XElement type = new XElement("RuminantType");
                type.Add(new XElement("Name", breed));
                type.Add(new XElement("Breed", breed));

                // Find the parameters for the breed
                XElement iats = GetIATParams(iat, col);
                XElement clems = GetCLEMParams();

                // Build a memo to track which coefficients are using default values (everything in clems)
                StringBuilder cdata = new StringBuilder();
                cdata.Append("These parameters are new and may default to incorrect values.");
                cdata.Append(" Double check these numbers before attempting to run a simulation: \n\n");
                foreach(XElement item in clems.Elements()) cdata.Append($"{item.Name,10} - {item.Value}\n");

                XElement memo = new XElement("Memo");
                memo.Add(new XElement("Name", "Memo"));
                memo.Add(new XElement("IncludeInDocumentation", "true"));
                memo.Add(new XElement("MemoText", new XCData(cdata.ToString())));
                //type.Add(memo);

                // Write the Initial Cohorts section
                XElement cohorts = new XElement("RuminantInitialCohorts");
                cohorts.Add(new XElement("Name", "InitialCohorts"));

                int row = -1;
                foreach (string group in numbers.GetRowNames())
                {
                    row++;
                    if (numbers.GetData<string>(row, col) != "0")
                    {
                        XElement cohort = new XElement("RuminantTypeCohort");
                        cohort.Add(new XElement("Name", group));
                        cohort.Add(new XElement("IncludeInDocumentation", "true"));

                        // Write gender to file based on the name of the cohort
                        if (group.Contains("F"))
                            cohort.Add(new XElement("Gender", "Female"));                            
                        else
                            cohort.Add(new XElement("Gender", "Male"));                            

                        // Format data correctly
                        double number = Math.Ceiling(numbers.GetData<double>(row, col));
                        double age = Math.Ceiling(ages.GetData<double>(row, col));

                        cohort.Add(new XElement("Age", $"{age}"));
                        cohort.Add(new XElement("Number", $"{number}"));
                        cohort.Add(new XElement("Weight", weights.GetData<string>(row, col)));
                        cohort.Add(new XElement("WeightSD", "0"));

                        // Checks if suckling and writes to file
                        if (group.Contains("Calf"))
                            cohort.Add(new XElement("Suckling", "true"));                            
                        else
                            cohort.Add(new XElement("Suckling", "false"));
                            

                        // Checks if breeding sire and writes to file
                        if (group == "Breeding Sires")
                            cohort.Add(new XElement("Sire", "true"));
                        else
                            cohort.Add(new XElement("Sire", "false"));

                        cohorts.Add(cohort);
                    }                        
                }
                type.Add(cohorts);
                type.Add(GetPricing(iat, col));
                type.Add(iats.Elements());
                type.Add(clems.Elements());                
                type.Add(new XElement("IncludeInDocumentation", "true"));

                herd.Add(type);
            }

            if (herd.Elements().Count() > 1) return herd;
            else return null;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iat"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static XElement GetPricing(IAT iat, int col)
        {
            IATable ruminants = iat.tables["Startup ruminant numbers"];
            IATable prices = iat.tables["Ruminant prices"];

            XElement pricing = new XElement("AnimalPricing");
            pricing.Add(new XElement("Name", "AnimalPricing"));

            string sire_price = "0";
            int row = -1;
            foreach (string group in ruminants.GetRowNames())
            {
                row++;
                if (ruminants.GetData<double>(row, col) != 0)
                {                    
                    string price = prices.GetData<string>(row, col);
                    if (!group.ToLower().Contains("sire")) sire_price = prices.GetData<string>(row, col);

                    XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                    XAttribute xa = new XAttribute(xsi + "type", "AnimalPriceGroup");

                    XElement model = new XElement
                    (
                        "Model",
                        xa,
                        new XElement("Name", group),
                        new XElement("IncludeInDocumentation", "true"),
                        new XElement("Value", price),
                        new XElement("PurchaseOrSale", "0")
                    );

                    pricing.Add(model);
                }                           
            }
            pricing.Add(new XElement("IncludeInDocumentation", "true"));
            pricing.Add(new XElement("PricingStyle", "perKg"));
            pricing.Add(new XElement("BreedingSirePrice", sire_price));

            return pricing;
        }

        /// <summary>
        /// Writes the 'Ruminant Manage' activity section
        /// </summary>
        /// <param name="iat">Source IAT</param>
        public static XElement Manage(IAT iat)
        {
            // Don't need to do anything if no ruminants are present
            if (iat.ruminants.Count == 0) return null;                     

            //Section: ActivityFolder - Manage herd
            XElement manage = new XElement
            (
                "ActivityFolder",
                new XElement("Name", $"Manage herd"),
                ManageBreeds(iat),           
                GetCutAndCarry(iat),
                GetGrazeAll(iat),
                GetSuppCommonLand(),
                GetFeed(iat),
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
        /// <param name="iat"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> ManageBreeds(IAT iat)
        {
            XElement breeds = new XElement("Breeds");

            IATable rs = iat.tables["Ruminant specifications"];
            foreach (int col in iat.ruminants)
            {
                string name = rs.GetColNames()[col];

                XElement milking = null;
                if (rs.GetData<double>(18, col) > 0) milking = GetMilking(name);

                XElement breed = new XElement
                (
                    "ActivityFolder",
                    new XElement("Name", $"Manage {name}"),
                    GetWean(iat, col),
                    milking,
                    GetNumbers(iat, col),
                    GetSellDry(iat, col)
                );

                breeds.Add(breed);
            }

            return breeds.Elements();
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
                    Labour.GetLabourRequirement(),
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
            IATable rs = iat.tables["Ruminant specifications"];

            XElement groups = new XElement("Groups");

            foreach (int id in iat.ruminants)
            {
                string breed = rs.GetColNames()[id];

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
        /// <param name="iat">Source IAT</param>
        private static XElement GetGrazeAll(IAT iat)
        {
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XAttribute xa = new XAttribute(xsi + "type", "RuminantActivityGrazeAll");

            XElement grazeall = new XElement
            (
                "Model",
                xa,
                new XElement("Name", "RuminantActivityGrazeAll"),
                Labour.GetLabourRequirement(),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "UseResourcesAvailable"),
                new XElement("HerdFilters", null),
                new XElement("HoursGrazed", "8")
            );

            return grazeall;
        }

        /// <summary>
        /// Writes the 'Supp common land' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="iat">Source IAT</param>
        private static XElement GetSuppCommonLand()
        {
            return null;
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iat"></param>
        /// <returns></returns>
        private static XElement GetFeed(IAT iat)
        {
            // Temporarily blocking this method
            return null;

            XElement feed = new XElement
            (
                "RuminantActivityFeed",
                new XElement("Name", "RuminantActivityFeed"),
                new XElement("IncludeInDocumentation", "true")
            );

            foreach(int pool in iat.pools.Keys)
            {
                XElement group = new XElement
                (
                    "RuminantFeedGroup",
                    new XElement("Name", "Feed All"),
                    new XElement("IncludeInDocumentation", "true")
                );

                XElement monthlies = new XElement("MonthlyValues");
                for (int i = 1; i <= 12; i++)
                {
                    monthlies.Add(new XElement("double", "0.02"));
                }
                group.Add(monthlies);

                XElement type = new XElement
                (
                    "RuminantActivityFeed",
                    new XElement("Name", $"{iat.pools[pool]}"),
                    group,
                    new XElement("IncludeInDocumentation", "true"),
                    new XElement("OnPartialResourcesAvailableAction", "UseResourcesAvailable"),
                    new XElement("HerdFilters", ""),
                    new XElement("FeedTypeName", $"AnimalFoodStore.Fodder pool {pool}"),
                    new XElement("ProportionTramplingWastage", "0"),
                    new XElement("FeedStyle", "ProportionOfWeight")
                );

                feed.Add(type);
            }

            return feed;
        }

        /// <summary>
        /// Writes the 'Grow' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="iat">Source IAT</param>
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
        /// <param name="iat">Source IAT</param>
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
        /// <param name="iat">Source IAT</param>
        private static XElement GetWean(IAT iat, int col)
        {
            IATable specs = iat.tables["Ruminant specifications"];

            XElement wean = new XElement
            (
                "RuminantActivityWean",
                new XElement("Name", "Wean"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("WeaningAge", specs.GetData<string>(7, col)),
                new XElement("WeaningWeight", specs.GetData<string>(8, col))
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
        /// <param name="iat">Source IAT</param>
        private static XElement GetNumbers(IAT iat, int col)
        {
            IATable specs = iat.tables["Ruminant specifications"];

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
                new XElement("MaximumBreedersKept", specs.GetData<string>(2, col)),
                new XElement("MinimumBreedersKept", specs.GetData<string>(38, col)),
                new XElement("MaximumBreederAge", specs.GetData<string>(3, col)),
                new XElement("MaximumBreedersPerPurchase", "1"),
                new XElement("MaximumSiresKept", "0"),
                new XElement("MaximumBullAge", specs.GetData<string>(25, col)),
                new XElement("AllowSireReplacement", "false"),
                new XElement("MaximumSiresPerPurchase", "0"),
                new XElement("MaleSellingAge", specs.GetData<string>(5, col)),
                new XElement("MaleSellingWeight", specs.GetData<string>(6, col)),
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
        /// <param name="iat">Source IAT</param>
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
        /// <param name="iat">Source IAT</param>
        private static XElement GetSellDry(IAT iat, int col)
        {
            IATable specs = iat.tables["Ruminant specifications"];
            double ptr = specs.GetData<double>(4, col) * 0.01;

            XElement selldry = new XElement
            (
                "RuminantActivitySellDryBreeders",
                new XElement("Name", "RuminantActivitySellDryBreeders"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("OnPartialResourcesAvailableAction", "ReportErrorAndStop"),
                new XElement("HerdFilters", null),
                new XElement("MinimumConceptionBeforeSell", "1"),
                new XElement("MonthsSinceBirth", specs.GetData<string>(32, col)),
                new XElement("ProportionToRemove", ptr.ToString())
            );

            return selldry;
        }

        /// <summary>
        /// Writes the 'Ruminant Muster' Activity section of a CLEM simulation
        /// </summary>
        /// <param name="iat">Source IAT</param>
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
