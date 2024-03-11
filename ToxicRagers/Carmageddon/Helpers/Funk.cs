using System;
using System.Collections.Generic;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Helpers
{
    public enum FunkMode
    {
        None,
        constant,
        distance,
        lastlap,
        otherlaps
    }

    public enum FunkMatrixMode
    {
        None,
        rock,
        roll,
        slither,
        spin,
        throb
    }

    public enum FunkAnimationType
    {
        None,
        frames,
        flic
    }

    public enum FrameRate
    {
        accurate,
        approximate
    }

    public enum FrameType
    {
        continuous,
        texturebits
    }

    public enum TexturebitMode
    {
        B,
        BV,
        V,
        VB
    }

    public class Funk
    {
        public string Material { get; set; }

        public FunkMode Mode { get; set; }

        public FunkMatrixMode MatrixModType { get; set; }

        public GroovePathMode MatrixModMode { get; set; }

        public Vector2 SlitherSpeed { get; set; }

        public Vector2 SlitherAmount { get; set; }
        public Vector2 ThrobCyclesPerSecond { get; set; }
        public Vector2 ThrobCenter { get; set; }
        public Vector2 ThrobExtends { get; set; }

        public float SpinPeriod { get; set; }

        public Vector2 RollPeriods { get; set; }

        public GroovePathMode LightingMode { get; set; }

        public FunkAnimationType AnimationType { get; set; }

        public FrameRate Framerate { get; set; }

        public FrameType FrameMode { get; set; }

        public TexturebitMode TextureBitMode { get; set; }

        public float FrameSpeed { get; set; }

        public List<string> Frames { get; set; } = new List<string>();
        public string FlicFilename { get; set; }

        public static Funk Load(DocumentParser file)
        {
            Funk funk = new Funk
            {
                Material = file.ReadLine(),
                Mode = file.ReadLine().ToEnum<FunkMode>(),
                MatrixModType = file.ReadLine().ToEnumWithDefault<FunkMatrixMode>(FunkMatrixMode.None)
            };

            if (funk.MatrixModType != FunkMatrixMode.None) { funk.MatrixModMode = file.ReadLine().ToEnum<GroovePathMode>(); }

            switch (funk.MatrixModType)
            {
                case FunkMatrixMode.None:
                    break;

                case FunkMatrixMode.roll:
                    funk.RollPeriods = file.ReadVector2();
                    break;

                case FunkMatrixMode.slither:
                    funk.SlitherSpeed = file.ReadVector2();
                    funk.SlitherAmount = file.ReadVector2();
                    break;

                case FunkMatrixMode.spin:
                    funk.SpinPeriod = file.ReadSingle();
                    break;

                case FunkMatrixMode.throb:
                    funk.ThrobCyclesPerSecond = file.ReadVector2();
                    funk.ThrobCenter = file.ReadVector2();
                    funk.ThrobExtends = file.ReadVector2();
                    break;
                default:
                    Console.WriteLine(file.ToString());
                    break;
            }

            funk.LightingMode = file.ReadLine().ToEnumWithDefault(GroovePathMode.None);
            if (funk.LightingMode != GroovePathMode.None)
            {
                Console.WriteLine(file.ToString());
            }

            funk.AnimationType = file.ReadLine().ToEnumWithDefault(FunkAnimationType.None);

            switch (funk.AnimationType)
            {
                case FunkAnimationType.None:
                    break;

                case FunkAnimationType.frames:
                    funk.Framerate = file.ReadEnum<FrameRate>();
                    funk.FrameMode = file.ReadEnum<FrameType>();

                    switch (funk.FrameMode)
                    {
                        case FrameType.texturebits:
                            funk.TextureBitMode = file.ReadEnum<TexturebitMode>();
                            break;

                        case FrameType.continuous:
                            funk.FrameSpeed = file.ReadSingle();
                            break;
                    }

                    int frameCount = file.ReadInt();

                    for (int i = 0; i < frameCount; i++)
                    {
                        funk.Frames.Add(file.ReadLine());
                    }
                    break;
                case FunkAnimationType.flic:
                    funk.Framerate = file.ReadEnum<FrameRate>();
                    funk.FlicFilename = file.ReadLine();
                    break;
                default:
                    throw new NotImplementedException($"funk type not supported!");
            }

            return funk;
        }
    }
}
