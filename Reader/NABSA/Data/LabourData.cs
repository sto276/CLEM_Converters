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

        public IEnumerable<LabourType> GetLabourTypes(Labour labour)
        {
            List<LabourType> types = new List<LabourType>();          
            var number = Priority.Elements().ToArray();

            for (int i = 2; i < number.Length; i++)
            {
                var ndata = number[i].Elements().ToArray();
                if (number[i].Elements().First().Value != "0")
                {
                    string name = number[i].Name.LocalName;
                    
                    // Find the age of the labourer
                    int age = name.Contains("Elderly") ? 65 
                        : (name.Contains("Teenager") ? 15 
                        : (name.Contains("Child") ? 8 : 40));

                    // Find the gender of the labourer
                    int gender = name.Contains("F") ? 1 : 0;

                    types.Add(new LabourType(labour)
                    {
                        Name = number[i].Name.LocalName,
                        InitialAge = age,
                        Gender = gender,
                        Individuals = Convert.ToInt32(ndata[0].Value)
                    });
                }
            }
            return types.AsEnumerable();
        }

        public IEnumerable<LabourAvailabilityItem> GetAvailabilityItems(LabourAvailabilityList list)
        {
            List<LabourAvailabilityItem> items = new List<LabourAvailabilityItem>();

            for (int i = 2; i < Supply.Elements().Count(); i++)
            {
                XElement group = Supply.Elements().ElementAt(i);

                // Skip this item if it has zero-value
                string value = group.Elements().First().Value;
                if (value == "0") continue;

                string name = group.Name.LocalName;

                LabourAvailabilityItem item = new LabourAvailabilityItem(list)
                {
                    Name = name,
                    Value = Convert.ToDouble(value)
                };

                item.Add(new LabourFilter(item)
                {
                    Value = name
                });

                items.Add(item);
            }

            return items.AsEnumerable();
        }

    }
}
