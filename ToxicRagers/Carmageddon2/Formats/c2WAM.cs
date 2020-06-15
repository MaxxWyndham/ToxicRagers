using System.Collections.Generic;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Carmageddon2.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class WAM
    {
        public int Version { get; set; }

        public List<float> XMins { get; set; } = new List<float>();
        public List<float> XMaxs { get; set; } = new List<float>();
        public List<float> YMins { get; set; } = new List<float>();
        public List<float> YMaxs { get; set; } = new List<float>();
        public List<float> ZMins { get; set; } = new List<float>();
        public List<float> ZMaxs { get; set; } = new List<float>();
        public float BendabilityFactor { get; set; }
        public float BendPointZMin { get; set; }
        public float BendPointZMax { get; set; }
        public float SnappabilityFactor { get; set; }
        public float YSplitPosition { get; set; }
        public Vector3 DriverPosition { get; set; } = Vector3.Zero;
        public List<CrushData> CrushEntries { get; set; } = new List<CrushData>();

        public static WAM Load(string path)
        {
            DocumentParser file = new DocumentParser(path);
            WAM wam = new WAM();

            string version = file.ReadLine();

            if (!version.StartsWith("VERSION"))
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 .wam file");
                return null;
            }

            wam.Version = version.Replace("VERSION ", "").ToInt();

            int xMinCount = file.ReadInt();
            for (int i = 0; i < xMinCount; i++) { wam.XMins.Add(file.ReadSingle()); }

            int xMaxCount = file.ReadInt();
            for (int i = 0; i < xMaxCount; i++) { wam.XMaxs.Add(file.ReadSingle()); }

            int yMinCount = file.ReadInt();
            for (int i = 0; i < yMinCount; i++) { wam.YMins.Add(file.ReadSingle()); }

            int yMaxCount = file.ReadInt();
            for (int i = 0; i < yMaxCount; i++) { wam.YMaxs.Add(file.ReadSingle()); }

            int zMinCount = file.ReadInt();
            for (int i = 0; i < zMinCount; i++) { wam.ZMins.Add(file.ReadSingle()); }

            int zMaxCount = file.ReadInt();
            for (int i = 0; i < zMaxCount; i++) { wam.ZMaxs.Add(file.ReadSingle()); }

            wam.BendabilityFactor = file.ReadSingle();
            wam.BendPointZMin = file.ReadSingle();
            wam.BendPointZMax = file.ReadSingle();
            wam.SnappabilityFactor = file.ReadSingle();
            wam.YSplitPosition = file.ReadSingle();
            wam.DriverPosition = file.ReadVector3();

            int crushEntries = file.ReadInt();

            for (int i = 0; i < crushEntries; i++)
            {
                CrushData crush = new CrushData { Actor = file.ReadLine() };

                crush.Softness = file.ReadEnum<CrushData.CrushSoftness>();
                crush.Type = file.ReadEnum<CrushData.CrushType>();

                switch (crush.Type)
                {
                    case CrushData.CrushType.detach:
                        crush.EaseOfDetach = file.ReadEnum<CrushData.Ease>();
                        crush.DetachmentType = file.ReadEnum<CrushData.DetachType>();
                        crush.CrushShape = file.ReadEnum<CrushData.BoxShape>();
                        break;

                    case CrushData.CrushType.flap:
                        crush.HingePoints.Add(file.ReadInt());
                        crush.HingePoints.Add(file.ReadInt());
                        crush.HingePoints.Add(file.ReadInt());
                        crush.KevOFlap = file.ReadInt() == 1;
                        crush.EaseOfFlap = file.ReadEnum<CrushData.Ease>();
                        crush.CrushShape = file.ReadEnum<CrushData.BoxShape>();
                        break;
                }

                if (crush.CrushShape == CrushData.BoxShape.poly)
                {
                    int polyPoints = file.ReadInt();

                    for (int j = 0; j < polyPoints; j++)
                    {
                        crush.ShapePoints.Add(file.ReadInt()); ;
                    }
                }

                int smashEntries = file.ReadInt();

                for (int j = 0; j < smashEntries; j++)
                {
                    SmashData smash = new SmashData
                    {
                        Trigger = file.ReadLine(),
                        TriggerMode = SmashData.SmashTriggerMode.TextureChange
                    };

                    smash.IntactMaterial = file.ReadLine();
                    int textureLevels = file.ReadInt();

                    for (int k = 0; k < textureLevels; k++)
                    {
                        SmashDataTextureLevel textureLevel = new SmashDataTextureLevel()
                        {
                            TriggerThreshold = file.ReadSingle(),
                            Flags = file.ReadInt()
                        };

                        textureLevel.Connotations.Load(file);

                        int pixelmaps = file.ReadInt();
                        for (int l = 0; l < pixelmaps; l++)
                        {
                            textureLevel.Pixelmaps.Add(file.ReadLine());
                        }

                        smash.Levels.Add(textureLevel);
                    }

                    crush.SmashEntries.Add(smash);
                }

                wam.CrushEntries.Add(crush);
            }

            return wam;
        }
    }

    public class CrushData
    {
        public enum CrushSoftness
        {
            soft,
            normal,
            hard,
            very_hard,
            uncrushable
        }

        public enum CrushType
        {
            boring,
            flap,
            detach
        }

        public enum Ease
        {
            very_soft,
            very_easy,
            easy,
            normal,
            hard
        }

        public enum DetachType
        {
            normal,
            stubborn,
            fully_detach
        }

        public enum BoxShape
        {
            box,
            poly
        }

        public string Actor { get; set; }
        public CrushSoftness Softness { get; set; } = CrushSoftness.normal;
        public CrushType Type { get; set; } = CrushType.boring;

        public List<int> HingePoints { get; set; } = new List<int>();
        public bool KevOFlap { get; set; }
        public Ease EaseOfFlap { get; set; }

        public Ease EaseOfDetach { get; set; }
        public DetachType DetachmentType { get; set; }

        public BoxShape CrushShape { get; set; }
        public List<int> ShapePoints { get; set; } = new List<int>();

        public List<SmashData> SmashEntries { get; set; } = new List<SmashData>();
    }
}
