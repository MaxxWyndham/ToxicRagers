using System.Collections.Generic;

namespace ToxicRagers.Carmageddon2
{
    public static class Powerups
    {
        static List<C2Powerup> powerups = new List<C2Powerup>()
        {
            new C2Powerup { Name = "Credits" },
            new C2Powerup { Name = "CreditsL" },
            new C2Powerup { Name = "PedsWithGreasedShoes", InMD = false },
            new C2Powerup { Name = "GiantPedestrians", InMD = false },
            new C2Powerup { Name = "ExplodingPeds" },
            new C2Powerup { Name = "HotRod" },
            new C2Powerup { Name = "TurboPeds" },
            new C2Powerup { Name = "Invulnerability" },
            new C2Powerup { Name = "FreeRepairs" },
            new C2Powerup { Name = "InstantRepair" },
            new C2Powerup { Name = "TimerFrozen", InMD=false },
            new C2Powerup { Name = "UnderwaterAbility" },
            new C2Powerup { Name = "TimeBonus" },
            new C2Powerup { Name = "BodyworkTrashed" },
            new C2Powerup { Name = "Mine" },
            new C2Powerup { Name = "FrozenOpponents" },
            new C2Powerup { Name = "FrozenCops", InMD = false },
            new C2Powerup { Name = "TurboOpponents" },
            new C2Powerup { Name = "TurboCops" },
            new C2Powerup { Name = "LunarGravity" },
            new C2Powerup { Name = "Pinball" },
            new C2Powerup { Name = "WallClimber", InMD = false },
            new C2Powerup { Name = "BouncyBouncy" },
            new C2Powerup { Name = "JellySuspension", InMD =false },
            new C2Powerup { Name = "PedsShowOnMap" },
            new C2Powerup { Name = "ElectrobastardRay" },
            new C2Powerup { Name = "GreasedTyres", InMD = false },
            new C2Powerup { Name = "AcmeDamageMagnifier" },
            new C2Powerup { Name = "Random", InMD = false },
            new C2Powerup { Name = "Random", InMD = false },
            new C2Powerup { Name = "Random", InMD = false },
            new C2Powerup { Name = "Random", InMD = false },
            new C2Powerup { Name = "InstantHandBrake" },
            new C2Powerup { Name = "ImmortalPedestrians", InMD = false },
            new C2Powerup { Name = "Turbo" },
            new C2Powerup { Name = "MegaTurbo" },
            new C2Powerup { Name = "StupidPedestrians", InMD = false },
            new C2Powerup { Name = "SuicidalPeds" },
            new C2Powerup { Name = "FreeRecoveryVouchers" },
            new C2Powerup { Name = "SolidGraniteCar" },
            new C2Powerup { Name = "RockSprings", InMD = false },
            new C2Powerup { Name = "Drugs", InMD = false },
            new C2Powerup { Name = "GripoMatricTyres", InMD = false },
            new C2Powerup { Name = "PedHead" },
            new C2Powerup { Name = "MutantCorpses", InMD = false },
            new C2Powerup { Name = "GravityFromJupiter" },
            new C2Powerup { Name = "Mine" },
            new C2Powerup { Name = "SlowMoPeds" },
            new C2Powerup { Name = "MiniaturePedestrians", InMD =false },
            new C2Powerup { Name = "TurboNutterBastardNitrous", InMD = false },
            new C2Powerup { Name = "GotimintheBollocks", InMD = false },
            new C2Powerup { Name = "AfterBurner" },
            new C2Powerup { Name = "MineShitting" },
            new C2Powerup { Name = "OilSlicksfromyourArse", InMD= false },
            new C2Powerup { Name = "KangarooOnCommand" },
            new C2Powerup { Name = "PedAnnihilator" },
            new C2Powerup { Name = "OpponentRepulsificator" },
            new C2Powerup { Name = "Dismemberfest" },
            new C2Powerup { Name = "EtherealPedestrians", InMD =false },
            new C2Powerup { Name = "GroovingPeds" },
            new C2Powerup { Name = "PedPanic" },
            new C2Powerup { Name = "HeliumFilledPeds" },
            new C2Powerup { Name = "PissArtistPedestrians", InMD=false },
            new C2Powerup { Name = "FatBastards", InMD=false },
            new C2Powerup { Name = "StickInsects", InMD=false },
            new C2Powerup { Name = "PedRepulsificator" },
            new C2Powerup { Name = "ExtraArmor", InMD=false },
            new C2Powerup { Name = "ExtraPower", InMD=false },
            new C2Powerup { Name = "ExtraOffensive", InMD=false },
            new C2Powerup { Name = "ExtraEverything", InMD=false },
            new C2Powerup { Name = "DoubleExtraArmor", InMD=false },
            new C2Powerup { Name = "DoubleExtraPower", InMD=false },
            new C2Powerup { Name = "DoubleExtraOffensive", InMD=false },
            new C2Powerup { Name = "DoubleExtraEverything", InMD=false },
            new C2Powerup { Name = "MaxArmor", InMD=false },
            new C2Powerup { Name = "MaxPower", InMD=false },
            new C2Powerup { Name = "MaxOffensive", InMD=false },
            new C2Powerup { Name = "MaxEverything", InMD=false },
            new C2Powerup { Name = "ExtraArmorSlot", InMD=false },
            new C2Powerup { Name = "ExtraPowerSlot", InMD=false },
            new C2Powerup { Name = "ExtraOffensiveSlot", InMD=false },
            new C2Powerup { Name = "ExtraEverythingSlot", InMD=false },
            new C2Powerup { Name = "BonusArmorSlots", InMD=false },
            new C2Powerup { Name = "BonusPowerSlots", InMD=false },
            new C2Powerup { Name = "BonusOffensiveSlots", InMD=false },
            new C2Powerup { Name = "BonusEverythingSlots", InMD=false },
            new C2Powerup { Name = "RandomAPO", InMD=false },
            new C2Powerup { Name = "RandomAPOPotential", InMD=false },
            new C2Powerup { Name = "RandomAPOGood", InMD=false },
            new C2Powerup { Name = "RandomAPOVeryGood", InMD=false },
            new C2Powerup { Name = "DrinkDriving", InMD=false },
            new C2Powerup { Name = "PedestrianFlameThrower", InMD=false },
            new C2Powerup { Name = "PedsOnDiazepam" },
            new C2Powerup { Name = "Cancellificatinizer" },
            new C2Powerup { Name = "MutantTailThing", InMD = false },
            new C2Powerup { Name = "SeverTail", InMD = false },
            new C2Powerup { Name = "SlaughterMortar" }
        };

        public static C2Powerup LookupID(int id)
        {
            return powerups[id];
        }
    }

    public class C2Powerup
    {
        public string Name { get; set; }

        public bool InMD { get; set; } = true;

        public string Model { get; set; } = "drum";
    }
}