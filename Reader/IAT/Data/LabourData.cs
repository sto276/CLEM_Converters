using Models.CLEM.Resources;
using Models.CLEM.Groupings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reader
{
    public partial class IAT
    { 
        private static readonly Dictionary<string, int> LabourAges = new Dictionary<string, int>()
        {
            {"Elderly Male", 72},
            {"Elderly Female", 68},
            {"Adult Male", 42},
            {"Adult Female", 31},
            {"Teenager Male", 13},
            {"Teenager Female", 12},
            {"Child Male", 7},
            {"Child Female", 7},
        };    

        /// <summary>
        /// Creates the a model for each Labour Type
        /// </summary>
        public IEnumerable<LabourType> GetLabourTypes(Labour parent)
        {
            List<LabourType> types = new List<LabourType>();

            int row = -1;
            foreach (string item in LabourSupply.RowNames)
            {
                row++;
                if (LabourSupply.GetData<string>(row, 0) != "0")
                {
                    // Finds the current demographic
                    string demo = LabourSupply.ExtraNames[row] + " " + LabourSupply.RowNames[row];

                    // Tries to find an age for the demographic, defaults to 20
                    int age = 20;
                    LabourAges.TryGetValue(demo, out age);

                    int gender = 0;
                    if (LabourSupply.RowNames[row].Contains("F")) gender = 1;

                    LabourType type = new LabourType(parent)
                    {
                        Name = demo,
                        InitialAge = age,
                        Gender = gender,
                        Individuals = LabourSupply.GetData<int>(row, 0)
                    };

                    types.Add(type);
                }
            }

            return types.AsEnumerable();
        }
        
        /// <summary>
        /// Models the availability of each Labour Type
        /// </summary>
        public IEnumerable<LabourAvailabilityItem> GetAvailabilityItems(LabourAvailabilityList parent)
        {
            List<LabourAvailabilityItem> items = new List<LabourAvailabilityItem>();

            int count = -1;
            foreach (var row in LabourSupply.RowNames)
            {
                count++;
                if (LabourSupply.GetData<string>(count, 0) != "0")
                {
                    string age = LabourSupply.ExtraNames[count];
                    string gender =  LabourSupply.RowNames[count];
                    double value = Math.Round(LabourSupply.GetData<double>(count, 2));

                    LabourAvailabilityItem item = new LabourAvailabilityItem(parent)
                    {
                        Name = age + " " + gender,
                        Value = value
                    };

                    LabourFilter GenderFilter = new LabourFilter(item)
                    {
                        Name = "GenderFilter",
                        Parameter = 1,
                        Value = gender
                    };

                    LabourAges.TryGetValue(item.Name, out int years);
                    LabourFilter AgeFilter = new LabourFilter(item)
                    {
                        Name = "AgeFilter",
                        Parameter = 2,
                        Operator = 5,
                        Value = years.ToString()
                    };

                    item.Children.Add(GenderFilter);
                    item.Children.Add(AgeFilter);

                    items.Add(item);
                }
            }
            return items.AsEnumerable();
        }
    }    
}