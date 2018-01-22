using System.IO;

using ToxicRagers.CarmageddonReincarnation.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public enum SetupContext
    {
        Vehicle
    }

    public class Setup
    {
        SetupContext context;
        LUACodeBlock settings;

        public LUACodeBlock Settings
        {
            get => settings;
            set => settings = value;
        }

        public Setup() { }

        public Setup(SetupContext context)
        {
            switch (context)
            {
                case SetupContext.Vehicle:
                    this.context = SetupContext.Vehicle;
                    settings = new VehicleSetupCode();
                    break;
            }
        }

        public static Setup Load(string path)
        {
            Setup setup = new Setup();
            LOL lol = LOL.Load(path);

            using (MemoryStream ms = new MemoryStream(lol.ReadAllBytes()))
            using (StreamReader sr = new StreamReader(ms))
            {
                string testLine = sr.ReadLine();
                string setupFile = testLine + "\r\n" + sr.ReadToEnd();

                switch (testLine.Split(':')[0])
                {
                    case "car":
                        setup.context = SetupContext.Vehicle;
                        setup.settings = VehicleSetupCode.Parse(setupFile);
                        break;

                    default:
                        return null;
                }
            }

            return setup;
        }

        public void Save(string path)
        {
            switch (context)
            {
                case SetupContext.Vehicle:
                    using (StreamWriter sw = new StreamWriter(path + "\\setup.lol"))
                    {
                        sw.WriteLine(settings.ToString());
                    }
                    break;
            }
        }
    }

    public class VehicleSetupCode : LUACodeBlock
    {
        public VehicleSetupCode()
        {
            blockPrefix = "car";
            underScored = false;

            AddMethod(
                LUACodeBlockMethodType.Set,
                "PowerMultiplier",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "TractionFactor",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor", Value = 1 }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "FinalDrive",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "RearGrip",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1.4f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontGrip",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CMPosY",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.52f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CMPosZ",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontDownforce",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value", Value = 20 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RearDownforce",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value", Value = 20 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontRoll",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RearRoll",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontCriticalAngle",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 6 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RearCriticalAngle",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 6 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontSuspGive",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.0667f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RearSuspGive",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.0667f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SuspDamping",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SuspensionRollFactor",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SuspensionPitchFactor",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "MomentOfInertiaMultiplier",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerSpeed1",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerSpeed2",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerSpeedVel",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 150 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerCentreMultiplier",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "MaxSteeringAngle",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 40 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "BrakeBalance",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 60 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "BrakeForce",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 60 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "HandBrakeStrength",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "TorqueSplit",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 65 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LSDThresholdF",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LSDThresholdR",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LSDThresholdM",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "ReversePowerMulitplier",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "WheelMass",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "DragCoefficient",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerLimit1",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerLimit2",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerLimitSpeed",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CastorSpeed1",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CastorSpeed2",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CastorSpeedVel",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 100 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerGyroscope",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "BrakeAttack",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "HandBrakeAttack",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.1f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SlideSpinRecovery",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RollingResistance",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.018f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "DriveMI",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 2.2f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "EngineMI",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.06f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RedLine",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value", Value = 6500 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "MaxRevs",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value", Value = 8000 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LimitRevs",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value", Value = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "ConstantEngineFriction",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 833 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LinearEngineFriction",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "QuadraticEngineFriction",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.0023f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "ConstantDriveFriction",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 9.1f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LinearDriveFriction",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "QuadraticDriveFriction",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.0008f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "EngineBrakeDelay",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "EngineBrakeAttack",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.28f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "ClutchDelay",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "Mass",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 2000 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "NumGears",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 5 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "GearRatios",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "0", Value = 10 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "1", Value = 13.86f },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "2", Value = 8.2193f },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "3", Value = 5.7005f },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "4", Value = 4.2327f },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "5", Value = 3.2277f },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "6", Value = 2.6775f },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "7", Value = 2.1616f },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "8", Value = 1.7157f },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "9", Value = 1.3752f },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "10", Value = 1.1346f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "TorqueCurve",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "0", Value = 20 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "1", Value = 80 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "2", Value = 208 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "3", Value = 264 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "4", Value = 280 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "5", Value = 264 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "6", Value = 240 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "7", Value = 208 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "8", Value = 160 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "9" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "10" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "11" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "12" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "13" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "14" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "15" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "16" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "17" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "18" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "19" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "20" }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "StabilityGripChange",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.4f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "StabilityThreshold",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CollisionEffect",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Effect" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );
        }

        public static VehicleSetupCode Parse(string cdata)
        {
            return Parse<VehicleSetupCode>(cdata);
        }
    }
}