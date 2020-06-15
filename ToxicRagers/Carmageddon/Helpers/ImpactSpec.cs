﻿using System.Collections.Generic;

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
        public List<ImpactSpecClause> Clauses { get; set; } = new List<ImpactSpecClause>();

        public ImpactSpec(DocumentParser file)
        {
            int clauseCount = file.ReadInt();

            for (int i = 0; i < clauseCount; i++)
            {
                Clauses.Add(new ImpactSpecClause(file));
            }
        }
    }

    public class ImpactSpecClause
    {
        public string Clause { get; set; }
        public List<ImpactSpecClauseSystem> Systems { get; set; } = new List<ImpactSpecClauseSystem>();

        public ImpactSpecClause(DocumentParser file)
        {
            Clause = file.ReadLine();

            int systemCount = file.ReadInt();

            for (int i = 0; i < systemCount; i++)
            {
                Systems.Add(new ImpactSpecClauseSystem(file.ReadStrings()));
            }
        }
    }

    public class ImpactSpecClauseSystem
    {
        public ImpactSpecClauseSystemPart Part { get; set; }
        public float Damage { get; set; }

        public ImpactSpecClauseSystem(string[] parts)
        {
            Part = parts[0].ToEnum<ImpactSpecClauseSystemPart>();
            Damage = parts[1].ToSingle();
        }
    }
}
