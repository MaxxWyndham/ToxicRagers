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

            AddMethod(LUACodeBlockMethodType.Set,
                "CollisionEffect",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Effect", Value = "effects.f_carsharpnel06", ForceOutput = true },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value", Value = 10.5f, ForceOutput = true }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "PowerMultiplier",
                "Multiplies up engine power",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "TractionFactor",
                "Gives extra traction with no extra lateral grip. 1 is normal",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor", Value = 1 }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "FinalDrive",
                "Final Drive Ratio",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "RearGrip",
                "Controls the grip of the rear tyres(in g) 1.5 is normal",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1.4f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontGrip",
                "Controls the grip of the front tyres (in g) 1.5 is normal",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CMPosY",
                "Height of centre of mass, in metres.  0 is at ground level, positive is up",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.52f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CMPosZ",
                "Forwards/Backwards position of centre of mass of car.  0.0 half way between wheels, 1.0 is over front axle, -1.0 is over rear axel. Suspension is adjusted to keep car level",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontDownforce",
                "Down force at front axle in kg",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value", Value = 20 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RearDownforce",
                "Down force at rear axle in kg",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value", Value = 20 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontRoll",
                "Front anti roll bar setting.  0.0 = soft, 1.0 = strong",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RearRoll",
                "Rear anti roll bar setting.  0.0 = soft, 1.0 = strong",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontCriticalAngle",
                "Angle at which front tyres loose traction (degrees)",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 6 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RearCriticalAngle",
                "Angle at which rear tyres loose traction (degrees)",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 6 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "FrontSuspGive",
                "Amount of suspension compression due to the weight of the car (front)",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.0667f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RearSuspGive",
                "Amount of suspension compression due to the weight of the car (rear)",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.0667f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SuspDamping",
                "Adjust suspension damping. 1 = critical damping",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SuspensionRollFactor",
                "Controls roll. 1.0 = physically realistic",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SuspensionPitchFactor",
                "Controls pitch. 1.0 = physically realistic",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "MomentOfInertiaMultiplier",
                "Moment of inertia multiplier of car in y axis",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerSpeed1",
                "Controls how fast the steering is. 1.0 is normal",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerSpeed2",
                "Controls how fast the steering is at SteerSpeedVel.  Actual steerspeed is interpolated between this and the steerspeed value, depending on the cars velocity",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerSpeedVel",
                "Controls how fast the steering is. See SteerSpeed2",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 150 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerCentreMultiplier",
                "Controls how fast the steering returns to centre. 0.0 is a special case which means centre immediately key is pressed",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "MaxSteeringAngle",
                "The angle in degrees that the wheels turn from straight ahead to full lock",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 40 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "BrakeBalance",
                "Controls the percentage of brake force applied to the front wheels",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 60 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "BrakeForce",
                "The brake force applied per KG(in m/s^2).  Will be limited by tyre grip",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 60 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "HandBrakeStrength",
                "Controls the decceleration force in Newtons per kg that the handbrake applies to the car",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "TorqueSplit",
                "Fraction of torque passed to rear wheels",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 65 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LSDThresholdF",
                "Front differential Rotation speed difference in rad/s needed to lock differential",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LSDThresholdR",
                "Rear differential Rotation speed difference in rad/s needed to lock differential",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LSDThresholdM",
                "Mid differential Rotation speed difference in rad/s needed to lock differential",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "ReversePowerMulitplier",
                "Reduces power when reversing",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "WheelMass",
                "Average mass of a wheel per 1000kg of car",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 10 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "DragCoefficient",
                "Fraction of air resistance force compared to a flat plate of the same/ncross-sectional area",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerLimit1",
                "Prevents wheels being turned to much for the given speed. 0 is off",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerLimit2",
                "Prevents wheels being turned to much for the given speed. 0 is off",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerLimitSpeed",
                "Speed at which steer limit 2 is applied",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CastorSpeed1",
                "Controls how stable the steering is. 0.0 is no castor 1.0 is max",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CastorSpeed2",
                "Controls how stable the steering is at SteerSpeedVel.  Actual castor speed is interpolated between this and the castor speed value, depending on the cars velocity",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "CastorSpeedVel",
                "Controls how stable the steering is. See CastorSpeed2",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 100 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SteerGyroscope",
                "Tends to make the front wheels preserve there world space alignment. This helps make the car stable. 0.0 is off, 1.0 is maximum",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "BrakeAttack",
                "Time taken to apply the full brake force in seconds",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "HandBrakeAttack",
                "Time taken in seconds for handbrake to reach full strength",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.1f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "SlideSpinRecovery",
                "When recovering from a slide, this prevents car spinning out the other way",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 1 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RollingResistance",
                "Rolling resistance force in Newtons per newton of vertical force per ms^-1 of velocity",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.018f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "DriveMI",
                "Moment of inertia due to drive system (Including gear box, wheels etc)",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 2.2f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "EngineMI",
                "Moment of inertyia of engine",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.06f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "RedLine",
                "Maximum safe revs of engine",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value", Value = 6500 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "MaxRevs",
                "Maximum revs of engine",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Value", Value = 8000 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LimitRevs",
                "Prevents engine over reving.  Can be on or off. (will reduce wheelspin when on)",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value", Value = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "ConstantEngineFriction",
                "Constant retardation torque in engine",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 833 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LinearEngineFriction",
                "Retardation torque in engine proportional to angular velocity",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "QuadraticEngineFriction",
                "Retardation torque in engine proportional to square of angular velocity",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.0023f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "ConstantDriveFriction",
                "Constant retardation torque in drive system",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 9.1f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "LinearDriveFriction",
                "Retardation torque in drive proportional to angular velocity",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "QuadraticDriveFriction",
                "Retardation torque in drive proportional to square of angular velocity",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.0008f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "EngineBrakeDelay",
                "Period of between releasing accelerator and engine braking starting",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "EngineBrakeAttack",
                "Period from engine braking starting to full engine braking (i.e. zero throttle)",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.28f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "ClutchDelay",
                "Period clutch is down when changing gear (s)",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "Mass",
                "Set the mass of the vehicle in Kg",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 2000 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "NumGears",
                "Number of gears (excluding R and N)",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 5 }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "GearRatios",
                "Engine revs / wheel revs",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Count", Value = 10 },
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
                "Engine torque in Nm",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Count", Value = 20 },
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
                "Amount grip can be adjusted to stabilise car",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.4f }
            );

            AddMethod(LUACodeBlockMethodType.Set,
                "StabilityThreshold",
                "Adjust point that stability control kicks in 0.0 = immediately, 1.0 = quit late, -1 = never",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.5f }
            );
        }

        public static VehicleSetupCode Parse(string cdata)
        {
            return Parse<VehicleSetupCode>(cdata);
        }
    }
}