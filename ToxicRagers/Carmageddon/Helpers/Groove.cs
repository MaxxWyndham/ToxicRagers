using System;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Helpers
{
    public enum LollipopMode
    {
        NotALollipop,
        xlollipop,
        ylollipop,
        zlollipop
    }

    public enum GrooveMode
    {
        constant,
        distance
    }

    public enum GroovePathNames
    {
        None,
        straight,
        circular
    }

    public enum GroovePathMode
    {
        None,
        linear,
        harmonic,
        flash,
        controlled,
        absolute,
        continuous
    }

    public enum GrooveAnimation
    {
        None,
        rock,
        shear,
        spin,
        throb
    }

    public enum GrooveAnimationAxis
    {
        x,
        y,
        z
    }
    public class Groove
    {
        public string Part { get; set; }
        public LollipopMode LollipopMode { get; set; }
        public GrooveMode Mode { get; set; }
        public GroovePathNames PathType { get; set; }
        public GroovePathMode PathMode { get; set; }
        public Vector3 PathCentre { get; set; }
        public float PathPeriod { get; set; }
        public Vector3 PathDelta { get; set; }
        public GrooveAnimation AnimationType { get; set; }
        public GroovePathMode AnimationMode { get; set; }
        public float AnimationPeriod { get; set; }
        public Vector3 AnimationCentre { get; set; }
        public GrooveAnimationAxis AnimationAxis { get; set; }
        public float RockMaxAngle { get; set; }
        public Vector3 ShearPeriod { get; set; }
        public Vector3 ShearMagnitude { get; set; }

        public Groove(DocumentParser file)
        {
            Part = file.ReadLine();
            LollipopMode = file.ReadLine().ToEnumWithDefault(LollipopMode.NotALollipop);
            Mode = file.ReadLine().ToEnum<GrooveMode>();
            PathType = file.ReadLine().ToEnumWithDefault(GroovePathNames.None);
            if (PathType != GroovePathNames.None) { PathMode = file.ReadLine().ToEnum<GroovePathMode>(); }

            switch (PathType)
            {
                case GroovePathNames.None:
                    break;

                case GroovePathNames.straight:
                    PathCentre = file.ReadVector3();
                    PathPeriod = file.ReadSingle();
                    PathDelta = file.ReadVector3();
                    break;

                default:
                    Console.WriteLine();
                    break;
            }

            AnimationType = file.ReadLine().ToEnumWithDefault(GrooveAnimation.None);
            if (AnimationType != GrooveAnimation.None) { AnimationMode = file.ReadLine().ToEnum<GroovePathMode>(); }

            switch (AnimationType)
            {
                case GrooveAnimation.None:
                    break;

                case GrooveAnimation.rock:
                    AnimationPeriod = file.ReadSingle();
                    AnimationCentre = file.ReadVector3();
                    AnimationAxis = file.ReadLine().ToEnum<GrooveAnimationAxis>();
                    RockMaxAngle = file.ReadSingle();
                    break;

                case GrooveAnimation.shear:
                    ShearPeriod = file.ReadVector3();
                    AnimationCentre = file.ReadVector3();
                    ShearMagnitude = file.ReadVector3();
                    break;

                case GrooveAnimation.spin:
                    AnimationPeriod = file.ReadSingle();
                    AnimationCentre = file.ReadVector3();
                    AnimationAxis = file.ReadLine().ToEnum<GrooveAnimationAxis>();
                    break;

                default:
                    Console.WriteLine(file.ToString());
                    break;
            }
        }
    }
}
