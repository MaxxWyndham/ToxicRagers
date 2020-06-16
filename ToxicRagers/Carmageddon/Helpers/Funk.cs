using System;
using System.Collections.Generic;
using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Helpers
{
    public enum FunkMode
    {
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

        public float SpinPeriod { get; set; }
        public Vector2 RollPeriods { get; set; }
        public GroovePathMode LightingMode { get; set; }
        public FunkAnimationType AnimationType { get; set; }
        public FrameRate Framerate { get; set; }
        public FrameType FrameMode { get; set; }
        public TexturebitMode TextureBitMode { get; set; }
        public float FrameSpeed { get; set; }
        public List<string> Frames { get; set; } = new List<string>();

        public Funk(DocumentParser file)
        {
            Material = file.ReadLine();
            Mode = file.ReadLine().ToEnum<FunkMode>();
            MatrixModType = file.ReadLine().ToEnumWithDefault<FunkMatrixMode>(FunkMatrixMode.None);
            if (MatrixModType != FunkMatrixMode.None) { MatrixModMode = file.ReadLine().ToEnum<GroovePathMode>(); }

            switch (MatrixModType)
            {
                case FunkMatrixMode.None:
                    break;

                case FunkMatrixMode.roll:
                    RollPeriods = file.ReadVector2();
                    break;

                case FunkMatrixMode.slither:
                    SlitherSpeed = file.ReadVector2();
                    SlitherAmount = file.ReadVector2();
                    break;

                case FunkMatrixMode.spin:
                    SpinPeriod = file.ReadSingle();
                    break;

                default:
                    Console.WriteLine(file.ToString());
                    break;
            }

            LightingMode = file.ReadLine().ToEnumWithDefault(GroovePathMode.None);
            if (LightingMode != GroovePathMode.None)
            {
                Console.WriteLine(file.ToString());
            }

            AnimationType = file.ReadLine().ToEnumWithDefault(FunkAnimationType.None);

            switch (AnimationType)
            {
                case FunkAnimationType.None:
                    break;

                case FunkAnimationType.frames:
                    Framerate = file.ReadEnum<FrameRate>();
                    FrameMode = file.ReadEnum<FrameType>();

                    switch (FrameMode)
                    {
                        case FrameType.texturebits:
                            TextureBitMode = file.ReadEnum<TexturebitMode>();
                            break;

                        case FrameType.continuous:
                            FrameSpeed = file.ReadSingle();
                            break;
                    }

                    int frameCount = file.ReadInt();

                    for (int i = 0; i < frameCount; i++)
                    {
                        Frames.Add(file.ReadLine());
                    }
                    break;

                default:
                    throw new NotImplementedException($"flic not supported!");
            }
        }
    }
}
