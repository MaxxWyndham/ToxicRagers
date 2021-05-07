using System;
using System.Collections.Generic;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public enum RaceType
    {
        Carma,
        Cars,
        Peds,
        Checkpoints,
        Smash,
        SmashnPed
    }

    public class RacesTXT
    {
        public int RaceToStartNewGameWith { get; set; }

        public DefaultRaceParameters FirstRace { get; set; } = new DefaultRaceParameters();

        public DefaultRaceParameters LastRace { get; set; } = new DefaultRaceParameters();

        public List<RaceDetails> Races { get; set; } = new List<RaceDetails>();

        public static RacesTXT Load(string path)
        {
            DocumentParser file = new DocumentParser(path);
            RacesTXT races = new RacesTXT
            {
                RaceToStartNewGameWith = file.ReadInt()
            };

            races.FirstRace.DefaultNumberOfOpponents = file.ReadInt();
            races.LastRace.DefaultNumberOfOpponents = file.ReadInt();
            races.FirstRace.SoftnessHardnessOfOpponents = file.ReadVector2();
            races.LastRace.SoftnessHardnessOfOpponents = file.ReadVector2();
            races.FirstRace.OpponentNastinessInfluencer = file.ReadSingle();
            races.LastRace.OpponentNastinessInfluencer = file.ReadSingle();

            for (int i = 0; i < 40; i++)
            {
                RaceDetails race = new RaceDetails
                {
                    Name = file.ReadLine(),
                    RaceFilename = file.ReadLine(),
                    NameOfInterfaceElement = file.ReadLine(),
                    NumberOfOpponents = file.ReadInt(),
                    NumberOfExplicitOpponents = file.ReadInt()
                };

                for (int j = 0; j < race.NumberOfExplicitOpponents; j++)
                {
                    race.ExplicitOpponents.Add(file.ReadInt());
                }

                race.OpponentNastinessLevel = file.ReadInt();
                race.PowerupExclusions = file.ReadInts();
                race.DisableTimeAwards = file.ReadInt() == 1;
                race.BoundaryRace = file.ReadInt() == 1;
                race.RaceType = file.ReadEnum<RaceType>();
                race.InitialTimer = file.ReadVector3();

                switch (race.RaceType)
                {
                    case RaceType.Carma:
                    case RaceType.Checkpoints:
                        race.Laps = file.ReadInt();
                        break;

                    case RaceType.Cars:
                        race.NumberOfOpponentsThatMustBeKilled = file.ReadInt();

                        for (int j = 0; j < race.NumberOfOpponentsThatMustBeKilled; j++)
                        {
                            race.OpponentsThatMustBeKilled.Add(file.ReadInt());
                        }
                        break;

                    case RaceType.Peds:
                        race.NumberOfPedGroups = file.ReadInt();

                        for (int j = 0; j < race.NumberOfPedGroups; j++)
                        {
                            race.PedGroupsThatMustBeKilled.Add(file.ReadInt());
                        }
                        break;

                    case RaceType.Smash:
                    case RaceType.SmashnPed:
                        race.SmashVariableNumber = file.ReadInt();
                        race.SmashVariableTarget = file.ReadInt();
                        if (race.RaceType == RaceType.SmashnPed) { race.PedGroupsThatMustBeKilled.Add(file.ReadInt()); }
                        break;
                }

                if (race.RaceType != RaceType.Carma)
                {
                    race.RaceCompletedBonus = file.ReadVector3();
                }
                else
                {
                    race.RaceCompletedBonusAllLaps = file.ReadVector3();
                    race.RaceCompletedBonusAllPeds = file.ReadVector3();
                    race.RaceCompletedBonusAllOpps = file.ReadVector3();
                }

                race.Description = file.ReadLine();
                race.Expansion = file.ReadInt();

                races.Races.Add(race);
            }

            if (file.ReadLine() != "END") { return null; }

            return races;
        }

        public void Save(string path)
        {
            if (Races.Count != 40) { throw new InvalidOperationException("There should be exactly 40 races!"); }

            using (DocumentWriter dw = new DocumentWriter(path))
            {
                dw.WriteLine($"{RaceToStartNewGameWith}", "Race to start new game with (development only)");

                dw.WriteLine();
                dw.WriteLine("// DEFAULT PARAMETERS");
                dw.WriteLine();

                dw.WriteLine($"{FirstRace.DefaultNumberOfOpponents}", "Default number of opponents in first race");
                dw.WriteLine($"{LastRace.DefaultNumberOfOpponents}", "Default number of opponents in last race");
                dw.WriteLine($"{FirstRace.SoftnessHardnessOfOpponents.X},{FirstRace.SoftnessHardnessOfOpponents.Y}", "Softness,hardest rank of opponents in first race");
                dw.WriteLine($"{LastRace.SoftnessHardnessOfOpponents.X},{FirstRace.SoftnessHardnessOfOpponents.Y}", "Softness,hardest rank of opponents in last race");
                dw.WriteLine($"{FirstRace.OpponentNastinessInfluencer}", "Opponent nastiness influencer for first race");
                dw.WriteLine($"{LastRace.OpponentNastinessInfluencer}", "Opponent nastiness influencer for last race");

                dw.WriteLine();
                dw.WriteLine("// RACE-BY-RACE DEFINITIONS");
                dw.WriteLine();

                int group = 1;

                for (int i = 0; i < 40; i++)
                {
                    RaceDetails race = Races[i];

                    if (i % 4 == 0)
                    {
                        dw.WriteLine($"// ====== GROUP {group++} ======");
                        dw.WriteLine();
                    }

                    if (race.BoundaryRace)
                    {
                        dw.WriteLine($"// ---- MISSION ----");
                        dw.WriteLine();
                    }

                    dw.WriteLine($"// Race {i}");
                    dw.WriteLine();

                    dw.WriteLine(race.Name);
                    dw.WriteLine(race.RaceFilename, "Text file name");
                    dw.WriteLine(race.NameOfInterfaceElement, "Name of interface element");
                    dw.WriteLine($"{race.NumberOfOpponents}", "Number of opponents (-1 = use default)");
                    dw.WriteLine($"{race.NumberOfExplicitOpponents}", "Number of explicit opponents");
                    foreach (int opponent in race.ExplicitOpponents) { dw.WriteLine($"{opponent}"); }
                    dw.WriteLine($"{race.OpponentNastinessLevel}", "Opponent nastiness level (-1 = use default)");
                    dw.WriteLine($"{string.Join(",", race.PowerupExclusions)}", "Powerup exclusions");
                    dw.WriteLine($"{race.DisableTimeAwards}", "Disable time awards");
                    dw.WriteLine($"{race.BoundaryRace}", "Boundary race (mission)");
                    dw.WriteLine($"{race.RaceType}", "Race type (0 = Carma1, 1 = Cars, 2 = Peds, 3 = Checkpoints, 4 = Smash, 5 = smash'n'ped)");
                    dw.WriteLine($"{race.InitialTimer.X},{race.InitialTimer.Y},{race.InitialTimer.Z}", "Initial timer count for each skill level");

                    switch (race.RaceType)
                    {
                        case RaceType.Carma:
                        case RaceType.Checkpoints:
                            dw.WriteLine($"{race.Laps}", "# laps");
                            break;

                        case RaceType.Cars:
                            dw.WriteLine($"{race.NumberOfOpponentsThatMustBeKilled}", "Number of opponents that must be killed (-1 means all)");
                            foreach (int opponent in race.OpponentsThatMustBeKilled) { dw.WriteLine($"{opponent}"); }
                            break;

                        case RaceType.Peds:
                            dw.WriteLine($"{race.NumberOfPedGroups}", "Number of ped groups (-1 means all)");
                            foreach (int pedGroup in race.PedGroupsThatMustBeKilled) { dw.WriteLine($"{pedGroup}"); }
                            break;

                        case RaceType.Smash:
                        case RaceType.SmashnPed:
                            dw.WriteLine($"{race.SmashVariableNumber}", "Smash variable number");
                            dw.WriteLine($"{race.SmashVariableTarget}", "Smash variable target");
                            if (race.RaceType == RaceType.SmashnPed) { dw.WriteLine($"{race.PedGroupsThatMustBeKilled[0]}", "Ped group index for required extra kills"); }
                            break;
                    }

                    if (race.RaceType == RaceType.Carma)
                    {
                        dw.WriteLine($"{race.RaceCompletedBonusAllLaps.X},{race.RaceCompletedBonusAllLaps.Y},{race.RaceCompletedBonusAllLaps.Z}", "Race completed bonus (all laps raced) for each skill level");
                        dw.WriteLine($"{race.RaceCompletedBonusAllPeds.X},{race.RaceCompletedBonusAllPeds.Y},{race.RaceCompletedBonusAllPeds.Z}", "Race completed bonus (all peds killed) for each skill level");
                        dw.WriteLine($"{race.RaceCompletedBonusAllOpps.X},{race.RaceCompletedBonusAllOpps.Y},{race.RaceCompletedBonusAllOpps.Z}", "Race completed bonus (all oppos wasted) for each skill level");
                    }
                    else
                    {
                        dw.WriteLine($"{race.RaceCompletedBonus.X},{race.RaceCompletedBonus.Y},{race.RaceCompletedBonus.Z}", "Race completed bonus for each skill level");
                    }

                    dw.WriteLine($"// Race description");
                    dw.WriteLine(race.Description);
                    dw.WriteLine($"{race.Expansion}", "Expansion");

                    dw.WriteLine();
                }
            }
        }
    }

    public class RaceDetails
    {
        public string Name { get; set; }

        public string RaceFilename { get; set; }

        public string NameOfInterfaceElement { get; set; }

        public int NumberOfOpponents { get; set; }

        public int NumberOfExplicitOpponents { get; set; }

        public List<int> ExplicitOpponents { get; set; } = new List<int>();

        public int OpponentNastinessLevel { get; set; }

        public int[] PowerupExclusions { get; set; }

        public bool DisableTimeAwards { get; set; }

        public bool BoundaryRace { get; set; }

        public RaceType RaceType { get; set; }

        public Vector3 InitialTimer { get; set; } = Vector3.Zero;

        public int Laps { get; set; }

        public Vector3 RaceCompletedBonus { get; set; } = Vector3.Zero;

        public Vector3 RaceCompletedBonusAllLaps { get; set; } = Vector3.Zero;

        public Vector3 RaceCompletedBonusAllPeds { get; set; } = Vector3.Zero;

        public Vector3 RaceCompletedBonusAllOpps { get; set; } = Vector3.Zero;

        public string Description { get; set; }

        public int Expansion { get; set; }

        public int NumberOfOpponentsThatMustBeKilled { get; set; }

        public List<int> OpponentsThatMustBeKilled { get; set; } = new List<int>();

        public int NumberOfPedGroups { get; set; }

        public List<int> PedGroupsThatMustBeKilled { get; set; } = new List<int>();

        public int SmashVariableNumber { get; set; }

        public int SmashVariableTarget { get; set; }
    }

    public class DefaultRaceParameters
    {
        public int DefaultNumberOfOpponents { get; set; }

        public Vector2 SoftnessHardnessOfOpponents { get; set; } = Vector2.Zero;

        public float OpponentNastinessInfluencer { get; set; }
    }
}