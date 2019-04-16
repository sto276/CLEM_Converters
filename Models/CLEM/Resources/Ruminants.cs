using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CLEM.Resources
{
    public class RuminantHerd : Node
    {
        public RuminantHerd(Node parent) : base(parent)
        {
            Name = "RuminantHerd";
        }
    }

    public class RuminantType : Node
    {
        public string Breed { get; set; }

        public double EMaintEfficiencyCoefficient { get; set; } = 0.0;

        public double EMaintEfficiencyIntercept { get; set; } = 0.0;

        public double EGrowthEfficiencyCoefficient { get; set; } = 0.0;

        public double EGrowthEfficiencyIntercept { get; set; } = 0.0;

        public double ELactationEfficiencyCoefficient { get; set; } = 0.0;

        public double ELactationEfficiencyIntercept { get; set; } = 0.0;

        public double EMaintExponent { get; set; } = 0.0;

        public double EMaintIntercept { get; set; } = 0.0;

        public double EMaintCoefficient { get; set; } = 0.0;

        public double EnergyMaintenanceMaximumAge { get; set; } = 0.0;

        public double Kme { get; set; } = 0.0;

        public double GrowthEnergyIntercept1 { get; set; } = 0.0;

        public double GrowthEnergyIntercept2 { get; set; } = 0.0;

        public double GrowthEfficiency { get; set; } = 0.0;

        public double NaturalWeaningAge { get; set; } = 0.0;

        public double SRWFemale { get; set; } = 0.0;

        public double SRWMaleMultiplier { get; set; } = 0.0;

        public double SRWBirth { get; set; } = 0.0;

        public double AgeGrowthRateCoefficient { get; set; } = 0.0;

        public double SRWGrowthScalar { get; set; } = 0.0;

        public double IntakeCoefficient { get; set; } = 0.0;

        public double IntakeIntercept { get; set; } = 0.0;

        public double ProteinCoefficient { get; set; } = 0.0;

        public double ProteinDegradability { get; set; } = 0.0;

        public double BaseAnimalEquivalent { get; set; } = 0.0;

        public double GreenDietMax { get; set; } = 0.0;

        public double GreenDietCoefficient { get; set; } = 0.0;

        public double GreenDietZero { get; set; } = 0.0;

        public double IntakeTropicalQuality { get; set; } = 0.0;

        public double IntakeCoefficientQuality { get; set; } = 0.0;

        public double IntakeCoefficientBiomass { get; set; } = 0.0;

        public bool StrictFeedingLimits { get; set; } = false;

        public double MilkIntakeCoefficient { get; set; } = 0.0;

        public double MilkIntakeIntercept { get; set; } = 0.0;

        public double MilkIntakeMaximum { get; set; } = 0.0;

        public double MilkLWTFodderSubstitutionProportion { get; set; } = 0.0;

        public double MaxJuvenileIntake { get; set; } = 0.0;

        public double ProportionalDiscountDueToMilk { get; set; } = 0.0;

        public double ProportionOfMaxWeightToSurvive { get; set; } = 0.0;

        public double LactatingPotentialModifierConstantA { get; set; } = 0.0;

        public double LactatingPotentialModifierConstantB { get; set; } = 0.0;

        public double LactatingPotentialModifierConstantC { get; set; } = 0.0;

        public double MaximumSizeOfIndividual { get; set; } = 0.0;

        public double MortalityBase { get; set; } = 0.0;

        public double MortalityCoefficient { get; set; } = 0.0;

        public double MortalityIntercept { get; set; } = 0.0;

        public double MortalityExponent { get; set; } = 0.0;

        public double JuvenileMortalityCoefficient { get; set; } = 0.0;

        public double JuvenileMortalityMaximum { get; set; } = 0.0;

        public double JuvenileMortalityExponent { get; set; } = 0.0;

        public double WoolCoefficient { get; set; } = 0.0;

        public double CashmereCoefficient { get; set; } = 0.0;

        public double MilkCurveSuckling { get; set; } = 0.0;

        public double MilkCurveNonSuckling { get; set; } = 0.0;

        public double MilkingDays { get; set; } = 0.0;

        public double MilkPeakYield { get; set; } = 0.0;

        public double MilkOffsetDay { get; set; } = 0.0;

        public double MilkPeakDay { get; set; } = 0.0;

        public double InterParturitionIntervalIntercept { get; set; } = 0.0;

        public double InterParturitionIntervalCoefficient { get; set; } = 0.0;

        public double GestationLength { get; set; } = 0.0;

        public double MinimumAge1stMating { get; set; } = 0.0;

        public double MinimumSize1stMating { get; set; } = 0.0;

        public double MinimumDaysBirthToConception { get; set; } = 0.0;

        public double MultipleBirthRate { get; set; } = 0.0;

        public double CriticalCowWeight { get; set; } = 0.0;

        public double ConceptionRateCoefficent { get; set; } = 0.0;

        public double ConceptionRateIntercept { get; set; } = 0.0;

        public double ConceptionRateAsymptote { get; set; } = 0.0;

        public double MaximumMaleMatingsPerDay { get; set; } = 0.0;

        public double PrenatalMortality { get; set; } = 0.0;

        public double MaximumConceptionUncontrolledBreeding { get; set; } = 0.0;

        public double MethaneProductionCoefficient { get; set; } = 0.0;

        public string Units { get; set; }

        public string SelectedTab { get; set; }

        public RuminantType(Node parent) : base(parent)
        {
            new RuminantInitialCohorts(this);
            new AnimalPricing(this);
        }
    }

    public class RuminantInitialCohorts : Node
    {
        public RuminantInitialCohorts(Node parent) : base(parent)
        {
            Name = "InitialCohorts";
        }
    }

    public class RuminantTypeCohort : Node
    {
        public int Gender { get; set; }

        public int Age { get; set; }

        public double Number { get; set; }

        public double Weight { get; set; }

        public double WeightSD { get; set; }

        public bool Suckling { get; set; }

        public bool Sire { get; set; }

        public RuminantTypeCohort(Node parent) : base(parent)
        {

        }
    }
}
