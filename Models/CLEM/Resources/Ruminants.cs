namespace Models.CLEM.Resources
{
    public class RuminantHerd : Node
    {
        public RuminantHerd(Node parent) : base(parent)
        {
            Name = "Ruminants";
            Add(Source.GetRuminants(this));
        }
    }

    public class RuminantType : Node
    {
        public string Breed { get; set; }

        public double EMaintEfficiencyCoefficient { get; set; } = 0.37;

        public double EMaintEfficiencyIntercept { get; set; } = 0.538;

        public double EGrowthEfficiencyCoefficient { get; set; } = 1.33;

        public double EGrowthEfficiencyIntercept { get; set; } = -0.308;

        public double ELactationEfficiencyCoefficient { get; set; } = 0.35;

        public double ELactationEfficiencyIntercept { get; set; } = 0.42;

        public double EMaintExponent { get; set; } = 0.000082;

        public double EMaintIntercept { get; set; } = 0.09;

        public double EMaintCoefficient { get; set; } = 0.26;

        public double EnergyMaintenanceMaximumAge { get; set; } = 6;

        public double Kme { get; set; } = 1;

        public double GrowthEnergyIntercept1 { get; set; } = 6.7;

        public double GrowthEnergyIntercept2 { get; set; } = 20.3;

        public double GrowthEfficiency { get; set; } = 1.09;

        public double NaturalWeaningAge { get; set; } = 3;

        public double SRWFemale { get; set; } = 342;

        public double SRWMaleMultiplier { get; set; } = 1.2;

        public double SRWBirth { get; set; } = 0.0472;

        public double AgeGrowthRateCoefficient { get; set; } = 0.01126;

        public double SRWGrowthScalar { get; set; } = 0.32;

        public double IntakeCoefficient { get; set; } = 0.024;

        public double IntakeIntercept { get; set; } = 1.7;

        public double ProteinCoefficient { get; set; } = 110;

        public double ProteinDegradability { get; set; } = 0.9;

        public double BaseAnimalEquivalent { get; set; } = 450;

        public double GreenDietMax { get; set; } = 0.98;

        public double GreenDietCoefficient { get; set; } = 0.15;

        public double GreenDietZero { get; set; } = 0.04;

        public double IntakeTropicalQuality { get; set; } = 0.16;

        public double IntakeCoefficientQuality { get; set; } = 1.7;

        public double IntakeCoefficientBiomass { get; set; } = 0.01;

        public bool StrictFeedingLimits { get; set; } = true;

        public double MilkIntakeCoefficient { get; set; } = 0.1206;

        public double MilkIntakeIntercept { get; set; } = 3.8146;

        public double MilkIntakeMaximum { get; set; } = 20;

        public double MilkLWTFodderSubstitutionProportion { get; set; } = 0.2;

        public double MaxJuvenileIntake { get; set; } = 0.04;

        public double ProportionalDiscountDueToMilk { get; set; } = 0.3;

        public double ProportionOfMaxWeightToSurvive { get; set; } = 0.5;

        public double LactatingPotentialModifierConstantA { get; set; } = 0.42;

        public double LactatingPotentialModifierConstantB { get; set; } = 0.61;

        public double LactatingPotentialModifierConstantC { get; set; } = 1.7;

        public double MaximumSizeOfIndividual { get; set; } = 1.2;

        public double MortalityBase { get; set; } = 0.03;

        public double MortalityCoefficient { get; set; } = 2.5;

        public double MortalityIntercept { get; set; } = 0.05;

        public double MortalityExponent { get; set; } = 3;

        public double JuvenileMortalityCoefficient { get; set; } = 3;

        public double JuvenileMortalityMaximum { get; set; } = 0.36;

        public double JuvenileMortalityExponent { get; set; } = 3;

        public double WoolCoefficient { get; set; } = 0.0;

        public double CashmereCoefficient { get; set; } = 0.0;

        public double MilkCurveSuckling { get; set; } = 0.6;

        public double MilkCurveNonSuckling { get; set; } = 0.11;

        public double MilkingDays { get; set; } = 304;

        public double MilkPeakYield { get; set; } = 8;

        public double MilkOffsetDay { get; set; } = 4;

        public double MilkPeakDay { get; set; } = 30;

        public double InterParturitionIntervalIntercept { get; set; } = 10.847;

        public double InterParturitionIntervalCoefficient { get; set; } = -0.7994;

        public double GestationLength { get; set; } = 9;

        public double MinimumAge1stMating { get; set; } = 24;

        public double MinimumSize1stMating { get; set; } = 0.5;

        public double MinimumDaysBirthToConception { get; set; } = 61;

        public double TwinRate { get; set; } = 0.01;

        public double CriticalCowWeight { get; set; } = 0.5;

        public double ConceptionRateCoefficient { get; set; } = -9.3353;

        public double ConceptionRateIntercept { get; set; } = 5.182;

        public double ConceptionRateAsymptote { get; set; } = 81.54;

        public double MaximumMaleMatingsPerDay { get; set; } = 30;

        public double PrenatalMortality { get; set; } = 0.08;

        public double MaximumConceptionUncontrolledBreeding { get; set; } = 0.9;

        public double MethaneProductionCoefficient { get; set; } = 20.7;        

        public string Units { get; set; }

        public RuminantType(Node parent) : base(parent)
        {
            Add(new RuminantInitialCohorts(this));
            Add(new AnimalPricing(this));
        }
    }

    public class RuminantInitialCohorts : Node
    {
        public RuminantInitialCohorts(Node parent) : base(parent)
        {
            Name = "InitialCohorts";
            Add(Source.GetCohorts(this));
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
