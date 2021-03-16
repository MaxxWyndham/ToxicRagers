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

        public Vector3 PathCentre { get; set; } = Vector3.Zero;

        public float PathPeriod { get; set; }

        public Vector3 PathDelta { get; set; }

        public GrooveAnimation AnimationType { get; set; }

        public GroovePathMode AnimationMode { get; set; }

        public float AnimationPeriod { get; set; }

        public Vector3 AnimationCentre { get; set; } = Vector3.Zero;

        public GrooveAnimationAxis AnimationAxis { get; set; }

        public float RockMaxAngle { get; set; }

        public Vector3 ShearPeriod { get; set; }

        public Vector3 ShearMagnitude { get; set; }

        public static Groove Load(DocumentParser file)
        {
            Groove groove = new Groove
            {
                Part = file.ReadLine(),
                LollipopMode = file.ReadLine().ToEnumWithDefault(LollipopMode.NotALollipop),
                Mode = file.ReadLine().ToEnum<GrooveMode>(),
                PathType = file.ReadLine().ToEnumWithDefault(GroovePathNames.None)
            };

            if (groove.PathType != GroovePathNames.None) { groove.PathMode = file.ReadLine().ToEnum<GroovePathMode>(); }

            switch (groove.PathType)
            {
                case GroovePathNames.None:
                    break;

                case GroovePathNames.straight:
                    groove.PathCentre = file.ReadVector3();
                    groove.PathPeriod = file.ReadSingle();
                    groove.PathDelta = file.ReadVector3();
                    break;

                default:
                    Console.WriteLine();
                    break;
            }

            groove.AnimationType = file.ReadLine().ToEnumWithDefault(GrooveAnimation.None);
            if (groove.AnimationType != GrooveAnimation.None) { groove.AnimationMode = file.ReadLine().ToEnum<GroovePathMode>(); }

            switch (groove.AnimationType)
            {
                case GrooveAnimation.None:
                    break;

                case GrooveAnimation.rock:
                    groove.AnimationPeriod = file.ReadSingle();
                    groove.AnimationCentre = file.ReadVector3();
                    groove.AnimationAxis = file.ReadLine().ToEnum<GrooveAnimationAxis>();
                    groove.RockMaxAngle = file.ReadSingle();
                    break;

                case GrooveAnimation.shear:
                    groove.ShearPeriod = file.ReadVector3();
                    groove.AnimationCentre = file.ReadVector3();
                    groove.ShearMagnitude = file.ReadVector3();
                    break;

                case GrooveAnimation.spin:
                    groove.AnimationPeriod = file.ReadSingle();
                    groove.AnimationCentre = file.ReadVector3();
                    groove.AnimationAxis = file.ReadLine().ToEnum<GrooveAnimationAxis>();
                    break;

                default:
                    Console.WriteLine(file.ToString());
                    break;
            }

            return groove;
        }
    }
}
