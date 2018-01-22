using System.Collections.Generic;

namespace ToxicRagers.Carmageddon2
{
    public static class Powerups
    {
        static List<C2Powerup> powerups = new List<C2Powerup>()
        {
            new C2Powerup { Name = "Credits" },
            new C2Powerup { Name = "CreditsL" },
            new C2Powerup { Name = "PedsWithGreasedShoes", InCR = false },
            new C2Powerup { Name = "GiantPedestrians", InCR = false },
            new C2Powerup { Name = "ExplodingPeds" },
            new C2Powerup { Name = "HotRod" },
            new C2Powerup { Name = "TurboPeds" },
            new C2Powerup { Name = "Invulnerability" },
            new C2Powerup { Name = "FreeRepairs" },
            new C2Powerup { Name = "InstantRepair" },
            new C2Powerup { Name = "TimerFrozen", InCR=false },
            new C2Powerup { Name = "UnderwaterAbility" },
            new C2Powerup { Name = "TimeBonus" },
            new C2Powerup { Name = "BodyworkTrashed" },
            new C2Powerup { Name = "Mine" },
            new C2Powerup { Name = "FrozenOpponents" },
            new C2Powerup { Name = "FrozenCops", InCR = false },
            new C2Powerup { Name = "TurboOpponents" },
            new C2Powerup { Name = "TurboCops" },
            new C2Powerup { Name = "LunarGravity" },
            new C2Powerup { Name = "Pinball" },
            new C2Powerup { Name = "WallClimber", InCR = false },
            new C2Powerup { Name = "BouncyBouncy" },
            new C2Powerup { Name = "JellySuspension", InCR =false },
            new C2Powerup { Name = "PedsShowOnMap" },
            new C2Powerup { Name = "ElectrobastardRay" },
            new C2Powerup { Name = "GreasedTyres", InCR = false },
            new C2Powerup { Name = "AcmeDamageMagnifier" },
            new C2Powerup { Name = "Random", InCR = false },
            new C2Powerup { Name = "Random", InCR = false },
            new C2Powerup { Name = "Random", InCR = false },
            new C2Powerup { Name = "Random", InCR = false },
            new C2Powerup { Name = "InstantHandBrake" },
            new C2Powerup { Name = "ImmortalPedestrians", InCR = false },
            new C2Powerup { Name = "Turbo" },
            new C2Powerup { Name = "MegaTurbo" },
            new C2Powerup { Name = "StupidPedestrians", InCR = false },
            new C2Powerup { Name = "SuicidalPeds" },
            new C2Powerup { Name = "FreeRecoveryVouchers" },
            new C2Powerup { Name = "SolidGraniteCar" },
            new C2Powerup { Name = "RockSprings", InCR = false },
            new C2Powerup { Name = "Drugs", InCR = false },
            new C2Powerup { Name = "GripoMatricTyres", InCR = false },
            new C2Powerup { Name = "PedHead" },
            new C2Powerup { Name = "MutantCorpses", InCR = false },
            new C2Powerup { Name = "GravityFromJupiter" },
            new C2Powerup { Name = "Mine" },
            new C2Powerup { Name = "SlowMoPeds" },
            new C2Powerup { Name = "MiniaturePedestrians", InCR =false },
            new C2Powerup { Name = "TurboNutterBastardNitrous", InCR = false },
            new C2Powerup { Name = "GotimintheBollocks", InCR = false },
            new C2Powerup { Name = "AfterBurner" },
            new C2Powerup { Name = "MineShitting" },
            new C2Powerup { Name = "OilSlicksfromyourArse", InCR= false },
            new C2Powerup { Name = "KangarooOnCommand" },
            new C2Powerup { Name = "PedAnnihilator" },
            new C2Powerup { Name = "OpponentRepulsificator" },
            new C2Powerup { Name = "Dismemberfest" },
            new C2Powerup { Name = "EtherealPedestrians", InCR =false },
            new C2Powerup { Name = "GroovingPeds" },
            new C2Powerup { Name = "PedPanic" },
            new C2Powerup { Name = "HeliumFilledPeds" },
            new C2Powerup { Name = "PissArtistPedestrians", InCR=false },
            new C2Powerup { Name = "FatBastards", InCR=false },
            new C2Powerup { Name = "StickInsects", InCR=false },
            new C2Powerup { Name = "PedRepulsificator" },
            new C2Powerup { Name = "ExtraArmor", InCR=false },
            new C2Powerup { Name = "ExtraPower", InCR=false },
            new C2Powerup { Name = "ExtraOffensive", InCR=false },
            new C2Powerup { Name = "ExtraEverything", InCR=false },
            new C2Powerup { Name = "DoubleExtraArmor", InCR=false },
            new C2Powerup { Name = "DoubleExtraPower", InCR=false },
            new C2Powerup { Name = "DoubleExtraOffensive", InCR=false },
            new C2Powerup { Name = "DoubleExtraEverything", InCR=false },
            new C2Powerup { Name = "MaxArmor", InCR=false },
            new C2Powerup { Name = "MaxPower", InCR=false },
            new C2Powerup { Name = "MaxOffensive", InCR=false },
            new C2Powerup { Name = "MaxEverything", InCR=false },
            new C2Powerup { Name = "ExtraArmorSlot", InCR=false },
            new C2Powerup { Name = "ExtraPowerSlot", InCR=false },
            new C2Powerup { Name = "ExtraOffensiveSlot", InCR=false },
            new C2Powerup { Name = "ExtraEverythingSlot", InCR=false },
            new C2Powerup { Name = "BonusArmorSlots", InCR=false },
            new C2Powerup { Name = "BonusPowerSlots", InCR=false },
            new C2Powerup { Name = "BonusOffensiveSlots", InCR=false },
            new C2Powerup { Name = "BonusEverythingSlots", InCR=false },
            new C2Powerup { Name = "RandomAPO", InCR=false },
            new C2Powerup { Name = "RandomAPOPotential", InCR=false },
            new C2Powerup { Name = "RandomAPOGood", InCR=false },
            new C2Powerup { Name = "RandomAPOVeryGood", InCR=false },
            new C2Powerup { Name = "DrinkDriving", InCR=false },
            new C2Powerup { Name = "PedestrianFlameThrower", InCR=false },
            new C2Powerup { Name = "PedsOnDiazepam" },
            new C2Powerup { Name = "Cancellificatinizer" },
            new C2Powerup { Name = "MutantTailThing", InCR = false },
            new C2Powerup { Name = "SeverTail", InCR = false },
            new C2Powerup { Name = "SlaughterMortar" }
        };

        public static C2Powerup LookupID(int id)
        {
            return powerups[id];
        }
    }

    public class C2Powerup
    {
        string name;
        string model = "drum";
        bool bCanCR = true;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public bool InCR
        {
            get => bCanCR;
            set => bCanCR = value;
        }

        public string Model
        {
            get => model;
            set => model = value;
        }
    }
}