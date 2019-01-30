using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NABSA
{
    using static Queries;
    static class Foodstores
    {
        /// <summary>
        /// Builds the 'Graze Food Store' subsection
        /// </summary>
        /// <param name="nabsa">Source NABSA element</param>
        public static XElement GetGraze(XElement nabsa)
        {
            // Element containing individual parameter data in NABSA
            XElement param = nabsa.Element("SingleParams");

            // Picking out specific parameters from the element
            XElement type = new XElement
            (
                "GrazeFoodStoreType",
                new XElement("Name", "NativePasture"),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("NToDMDCoefficient", param.Element("Coeff_DMD").Value),
                new XElement("NToDMDIntercept", param.Element("Incpt_DMD").Value),
                new XElement("NToDMDCrudeProteinDenominator", "0"),
                new XElement("GreenNitrogen", param.Element("Native_N").Value),
                new XElement("DecayNitrogen", param.Element("Decay_N").Value),
                new XElement("MinimumNitrogen", param.Element("Native_N_min").Value),
                new XElement("DecayDMD", param.Element("Decay_DMD").Value),
                new XElement("MinimumDMD", param.Element("Native_DMD_min").Value),
                new XElement("DetachRate", param.Element("Native_detach").Value),
                new XElement("CarryoverDetachRate", param.Element("Carryover_detach").Value),
                new XElement("IntakeTropicalQualityCoefficient", param.Element("Intake_trop_quality").Value),
                new XElement("IntakeQualityCoefficient", param.Element("Intake_coeff_quality").Value)
            );

            // Adding the parameters to the food store
            XElement store = new XElement(
                "GrazeFoodStore",
                new XElement("Name", "GrazeFoodStore"),
                type,
                new XElement("IncludeInDocumentation", "true")
                );

            return store;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        public static XElement GetAnimal(XElement nabsa)
        {
            XElement animal = new XElement
            (
                "AnimalFoodStore",
                new XElement("Name", "AnimalFoodStore"),
                GetSupplements(nabsa),
                GetBought(nabsa),
                new XElement("IncludeInDocumentation", "true")
            );

            return animal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetSupplements(XElement nabsa)
        {
            var allocs = FindByName(nabsa, "Supplement allocations").Elements().Skip(2);
            var specs = FindByName(nabsa, "Supplement specifications").Elements().Skip(2);

            XElement supplements = new XElement("Supplements");

            foreach (XElement supplement in allocs)
            {
                specs = specs.Skip(1);

                var amounts = GetElementValues(supplement).ToList();
                if (!amounts.Exists(s => s != "0")) continue;
                
                XElement type = GetAnimalFoodStoreType(specs.First());
                supplements.Add(type);                                   
            }

            return supplements.Elements();
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nabsa"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetBought(XElement nabsa)
        {
            var fodder = FindByName(nabsa, "Bought fodder").Elements().Skip(2);
            var specs = FindByName(nabsa, "Bought fodder specs").Elements().Skip(2);

            XElement bought = new XElement("Bought");

            foreach (var item in fodder)
            {
                specs = specs.Skip(1);

                if (item.Elements().ElementAt(3).Value == "FALSE") continue;

                XElement type = GetAnimalFoodStoreType(specs.First());
                bought.Add(type);
            }

            return bought.Elements();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static XElement GetAnimalFoodStoreType(XElement data)
        {
            XElement type = new XElement
            (
                "AnimalFoodStoreType",
                new XElement("Name", data.Name.LocalName),
                new XElement("IncludeInDocumentation", "true"),
                new XElement("DMD", data.Elements().ElementAt(1).Value),
                new XElement("Nitrogen", data.Elements().ElementAt(2).Value),
                new XElement("StartingAmount", "0")
            );

            return type;
        }

    }
}
