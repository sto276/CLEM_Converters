using Models.CLEM.Resources;
using Models.CLEM.Groupings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReadIAT
{
    public partial class IAT
    {
        private static class LabourData
        {
            public static SubTable Supply { get; private set; }            

            /// <summary>
            /// 
            /// </summary>
            public static readonly Dictionary<string, int> Ages = new Dictionary<string, int>()
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

            public static void Construct(IAT source)
            {
                Supply = new SubTable("Labour supply/hire", source);
            }        
        }

        public IEnumerable<LabourType> GetLabourTypes(Labour parent)
        {
            List<LabourType> types = new List<LabourType>();

            int row = -1;
            foreach (string item in LabourData.Supply.RowNames)
            {
                row++;
                if (LabourData.Supply.GetData<string>(row, 0) != "0")
                {
                    // Finds the current demographic
                    string demo = LabourData.Supply.ExtraNames[row] + " " + LabourData.Supply.RowNames[row];

                    // Tries to find an age for the demographic, defaults to 20
                    int age = 20;
                    LabourData.Ages.TryGetValue(demo, out age);

                    int gender = 0;
                    if (LabourData.Supply.RowNames[row].Contains("F")) gender = 1;

                    LabourType type = new LabourType(parent)
                    {
                        Name = demo,
                        InitialAge = age,
                        Gender = gender,
                        Individuals = LabourData.Supply.GetData<int>(row, 0)
                    };

                    types.Add(type);
                }
            }

            return types.AsEnumerable();
        }
        
        public IEnumerable<LabourAvailabilityItem> GetAvailabilityItems(LabourAvailabilityList parent)
        {
            List<LabourAvailabilityItem> items = new List<LabourAvailabilityItem>();

            int count = -1;
            foreach (var row in LabourData.Supply.RowNames)
            {
                count++;
                if (LabourData.Supply.GetData<string>(count, 0) != "0")
                {
                    string age = LabourData.Supply.ExtraNames[count];
                    string gender =  LabourData.Supply.RowNames[count];

                    double value = Math.Round(LabourData.Supply.GetData<double>(count, 2));

                    LabourAvailabilityItem item = new LabourAvailabilityItem(parent)
                    {
                        Name = age + gender,
                        Value = value
                    };

                    LabourFilter filter = new LabourFilter(item)
                    {
                        Value = age + gender
                    };
                    item.Children.Add(filter);

                    items.Add(item);
                }
            }
            return items.AsEnumerable();
        }
    }    
}