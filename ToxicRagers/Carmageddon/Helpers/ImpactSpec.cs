using System.Collections.Generic;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Helpers
{
    public enum ImpactSpecClauseSystemPart
    {
        driver,
        engine,
        lf_brake,
        lf_wheel,
        lr_brake,
        lr_wheel,
        rf_brake,
        rf_wheel,
        rr_brake,
        rr_wheel,
        steering,
        transmission
    }

    public class ImpactSpec
    {
        public string Description { get; set; }

        public List<ImpactSpecClause> Clauses { get; set; } = new List<ImpactSpecClause>();

        public static ImpactSpec Load(string description, DocumentParser file)
        {
            ImpactSpec spec = new ImpactSpec { Description = description };

            int clauseCount = file.ReadInt();

            for (int i = 0; i < clauseCount; i++)
            {
                spec.Clauses.Add(ImpactSpecClause.Load(file));
            }

            return spec;
        }
    }

    public class ImpactSpecClause
    {
        public string Clause { get; set; }

        public List<ImpactSpecClauseSystem> Systems { get; set; } = new List<ImpactSpecClauseSystem>();

        public static ImpactSpecClause Load(DocumentParser file)
        {
            ImpactSpecClause clause = new ImpactSpecClause
            {
                Clause = file.ReadLine()
            };

            int systemCount = file.ReadInt();

            for (int i = 0; i < systemCount; i++)
            {
                clause.Systems.Add(ImpactSpecClauseSystem.Load(file.ReadStrings()));
            }

            return clause;
        }
    }

    public class ImpactSpecClauseSystem
    {
        public ImpactSpecClauseSystemPart Part { get; set; }

        public float Damage { get; set; }

        public static ImpactSpecClauseSystem Load(string[] parts)
        {
            return new ImpactSpecClauseSystem
            {
                Part = parts[0].ToEnum<ImpactSpecClauseSystemPart>(),
                Damage = parts[1].ToSingle()
            };
        }
    }
}
