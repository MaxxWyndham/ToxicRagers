using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class CarDescriptor
    {
        private Dictionary<string, object> tokens = new Dictionary<string, object>();
        private string textureDescriptorFile;
        private string hierarchyFile;
        private string headerFile;
        private string dynamicObjectFile;
        private string shadowDcolFile;
        private string deformationFile;

        // SFX
        private string hornSirenSfx;
        private string engineSfx;
        private string brakeLightsTextureName;
        private Vector2 topLeftUV;
        private Vector2 bottomRightUV;
        private float brakeLightSize;

        // Node Definitions
        private int numBrakeLights;
        private List<string> brakeLightNodes = new List<string>();
        private int numDoors;
        private List<string> doorNodes = new List<string>();
        private int numBonnets;
        private List<string> bonnetNodes = new List<string>();
        private string shellNode;
        private string towBarNode;
        private string trailerDescriptor;
        private int numEngineFireSmokeNodes;
        private List<string> engineFireSmokeNodes = new List<string>();

        // Handling
        private int numGears;
        private List<float> gears = new List<float>();
        private float differntialRatio;
        private float clutchTorque;
        private float engineInertia;
        private float gearboxInertia;
        private float engineIdleRevs;
        private string torqueCurveFunc;
        private string tireSlipCurveFunc;
        private string tireLoadCurveFunc;
        private string camberThrustCurveFunc;
        private string camberGripCurveFunc;
        private string frontDownForceCurveFunc;
        private string rearDownForceCurveFunc;
        private string rollResistanceCurveFunc;

        // Wheel data
        private int numWheels;
        private float wheelThickness;
        private List<WheelData> wheels = new List<WheelData>();

        // Cockpit
        private string cockpitHierarchyFile;
        private Vector3 cockpitCameraPos;
        private Vector3 cockpitLookAtPosition;
        private Vector3 cockpitCarmeraToSteeringWheelOffset;
        private string driverNode;

        public class WheelData
        {
            private string masterNode;
            private string instanceNode;
            private int drivingWheel;
            private int steeringWheel;
            private float mass;
            private float xAxisMomentOfInertia;
            private Vector3 suspensionDirection;
            private float maxSuspensionOffset;
            private float minSuspensionOffset;
            private float camberAngle;
            private float radius;
            private float suspensionSpringStiffness;
            private float suspensionShockDamping;
            public string MasterNode
            {
                get => masterNode;
                set => masterNode = value;
            }
            public string InstanceNode
            {
                get => instanceNode;
                set => instanceNode = value;
            }
            public int DrivingWheel
            {
                get => drivingWheel;
                set => drivingWheel = value;
            }
            public int SteeringWheel
            {
                get => steeringWheel;
                set => steeringWheel = value;
            }
            public float Mass
            {
                get => mass;
                set => mass = value;
            }
            public float XAxisMomentOfInertia
            {
                get => xAxisMomentOfInertia;
                set => xAxisMomentOfInertia = value;
            }
            public Vector3 SuspensionDirection
            {
                get => suspensionDirection;
                set => suspensionDirection = value;
            }
            public float MaxSuspensionOffset
            {
                get => maxSuspensionOffset;
                set => maxSuspensionOffset = value;
            }
            public float MinSuspensionOffset
            {
                get => minSuspensionOffset;
                set => minSuspensionOffset = value;
            }
            public float CamberAngle
            {
                get => camberAngle;
                set => camberAngle = value;
            }
            public float Radius
            {
                get => radius;
                set => radius = value;
            }
            public float SuspensionSpringStiffness
            {
                get => suspensionSpringStiffness;
                set => suspensionSpringStiffness = value;
            }
            public float SuspensionShockDamping
            {
                get => suspensionShockDamping;
                set => suspensionShockDamping = value;
            }
        }

        public Dictionary<string, object> Tokens
        {
            get => tokens;
            set => tokens = value;
        }

        public string TextureDescriptorFile
        {
            get => textureDescriptorFile;
            set => textureDescriptorFile = value;
        }
        public string HierarchyFile
        {
            get => hierarchyFile;
            set => hierarchyFile = value;
        }
        public string HeaderFile
        {
            get => headerFile;
            set => headerFile = value;
        }
        public string DynamicObjectFile
        {
            get => dynamicObjectFile;
            set => dynamicObjectFile = value;
        }
        public string ShadowDcolFile
        {
            get => shadowDcolFile;
            set => shadowDcolFile = value;
        }
        public string DeformationFile
        {
            get => deformationFile;
            set => deformationFile = value;
        }

        // SFX
        public string HornSirenSfx
        {
            get => hornSirenSfx;
            set => hornSirenSfx = value;
        }
        public string EngineSfx
        {
            get => engineSfx;
            set => engineSfx = value;
        }
        public string BrakeLightsTextureName
        {
            get => brakeLightsTextureName;
            set => brakeLightsTextureName = value;
        }
        public Vector2 TopLeftUV
        {
            get => topLeftUV;
            set => topLeftUV = value;
        }
        public Vector2 BottomRightUV
        {
            get => bottomRightUV;
            set => bottomRightUV = value;
        }
        public float BrakeLightSize
        {
            get => brakeLightSize;
            set => brakeLightSize = value;
        }

        // Node Definitions
        public int NumBrakeLights
        {
            get => numBrakeLights;
            set => numBrakeLights = value;
        }
        public List<string> BrakeLightNodes
        {
            get => brakeLightNodes;
            set => brakeLightNodes = value;
        }
        public int NumDoors
        {
            get => numDoors;
            set => numDoors = value;
        }
        public List<string> DoorNodes
        {
            get => doorNodes;
            set => doorNodes = value;
        }
        public int NumBonnets
        {
            get => numBonnets;
            set => numBonnets = value;
        }
        public List<string> BonnetNodes
        {
            get => bonnetNodes;
            set => bonnetNodes = value;
        }
        public string ShellNode
        {
            get => shellNode;
            set => shellNode = value;
        }
        public string TowBarNode
        {
            get => towBarNode;
            set => towBarNode = value;
        }
        public string TrailerDescriptor
        {
            get => trailerDescriptor;
            set => trailerDescriptor = value;
        }
        public int NumEngineFireSmokeNodes
        {
            get => numEngineFireSmokeNodes;
            set => numEngineFireSmokeNodes = value;
        }
        public List<string> EngineFireSmokeNodes
        {
            get => engineFireSmokeNodes;
            set => engineFireSmokeNodes = value;
        }

        // Handling
        public int NumGears
        {
            get => numGears;
            set => numGears = value;
        }
        public List<float> Gears
        {
            get => gears;
            set => gears = value;
        }
        public float DifferntialRatio
        {
            get => differntialRatio;
            set => differntialRatio = value;
        }
        public float ClutchTorque
        {
            get => clutchTorque;
            set => clutchTorque = value;
        }
        public float EngineInertia
        {
            get => engineInertia;
            set => engineInertia = value;
        }
        public float GearboxInertia
        {
            get => gearboxInertia;
            set => gearboxInertia = value;
        }
        public float EngineIdleRevs
        {
            get => engineIdleRevs;
            set => engineIdleRevs = value;
        }
        public string TorqueCurveFunc
        {
            get => torqueCurveFunc;
            set => torqueCurveFunc = value;
        }
        public string TireSlipCurveFunc
        {
            get => tireSlipCurveFunc;
            set => tireSlipCurveFunc = value;
        }
        public string TireLoadCurveFunc
        {
            get => tireLoadCurveFunc;
            set => tireLoadCurveFunc = value;
        }
        public string CamberThrustCurveFunc
        {
            get => camberThrustCurveFunc;
            set => camberThrustCurveFunc = value;
        }
        public string CamberGripCurveFunc
        {
            get => camberGripCurveFunc;
            set => camberGripCurveFunc = value;
        }
        public string FrontDownForceCurveFunc
        {
            get => frontDownForceCurveFunc;
            set => frontDownForceCurveFunc = value;
        }
        public string RearDownForceCurveFunc
        {
            get => rearDownForceCurveFunc;
            set => rearDownForceCurveFunc = value;
        }
        public string RollResistanceCurveFunc
        {
            get => rollResistanceCurveFunc;
            set => rollResistanceCurveFunc = value;
        }

        // Wheel data
        public int NumWheels
        {
            get => numWheels;
            set => numWheels = value;
        }
        public float WheelThickness
        {
            get => wheelThickness;
            set => wheelThickness = value;
        }
        public List<WheelData> Wheels
        {
            get => wheels;
            set => wheels = value;
        }

        // Cockpit
        public string CockpitHierarchyFile
        {
            get => cockpitHierarchyFile;
            set => cockpitHierarchyFile = value;
        }

        public Vector3 CockpitCameraPos
        {
            get => cockpitCameraPos;
            set => cockpitCameraPos = value;
        }
        public Vector3 CockpitLookAtPosition
        {
            get => cockpitLookAtPosition;
            set => cockpitLookAtPosition = value;
        }
        public Vector3 CockpitCarmeraToSteeringWheelOffset
        {
            get => cockpitCarmeraToSteeringWheelOffset;
            set => cockpitCarmeraToSteeringWheelOffset = value;
        }
        public string DriverNode
        {
            get => driverNode;
            set => driverNode = value;
        }

        public static CarDescriptor Load(string path)
        {

            CarDescriptor descriptor = new CarDescriptor();

            DocumentParser parser = new DocumentParser(path);

            if (parser.PeekLine().Trim() == "TOKENS_START")
            {
                parser.ReadLine();
                while (parser.PeekLine().Trim() != "TOKENS_END")
                {
                    var tokenLine = parser.ReadLine().Trim().Split(new[] { ' ', '\t' });
                    if (!descriptor.Tokens.ContainsKey(tokenLine[0]))
                    {
                        descriptor.Tokens.Add(tokenLine[0], tokenLine.Length > 1 ? tokenLine[1] : null);
                    }

                }
                parser.ReadLine();


            }

            descriptor.TextureDescriptorFile = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.HierarchyFile = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.HeaderFile = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.DynamicObjectFile = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.ShadowDcolFile = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.DeformationFile = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.HornSirenSfx = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.EngineSfx = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.BrakeLightsTextureName = parser.ReadLine().Trim().Replace("\"", "");

            var splitLine = parser.ReadLine().Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            descriptor.TopLeftUV = new Vector2(float.Parse(splitLine[0]), float.Parse(splitLine[1]));

            splitLine = parser.ReadLine().Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            descriptor.BottomRightUV = new Vector2(float.Parse(splitLine[0]), float.Parse(splitLine[1]));
            descriptor.BrakeLightSize = parser.ReadSingle();

            descriptor.NumBrakeLights = parser.ReadInt();

            for (int i = 0; i < descriptor.NumBrakeLights; i++)
            {
                descriptor.BrakeLightNodes.Add(parser.ReadLine().Replace("\"", ""));
            }

            descriptor.NumDoors = parser.ReadInt();

            for (int i = 0; i < descriptor.NumDoors; i++)
            {
                descriptor.DoorNodes.Add(parser.ReadLine().Replace("\"", ""));
            }

            descriptor.NumBonnets = parser.ReadInt();

            for (int i = 0; i < descriptor.NumBonnets; i++)
            {
                descriptor.BonnetNodes.Add(parser.ReadLine().Replace("\"", ""));
            }

            descriptor.ShellNode = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.TowBarNode = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.TrailerDescriptor = parser.ReadLine().Trim().Replace("\"", "");

            descriptor.NumEngineFireSmokeNodes = parser.ReadInt();

            for (int i = 0; i < descriptor.NumEngineFireSmokeNodes; i++)
            {
                descriptor.EngineFireSmokeNodes.Add(parser.ReadLine().Trim().Replace("\"", ""));
            }

            descriptor.NumGears = parser.ReadInt();

            for (int i = 0; i < descriptor.NumGears; i++)
            {
                descriptor.Gears.Add(parser.ReadSingle());
            }

            descriptor.DifferntialRatio = parser.ReadSingle();
            descriptor.ClutchTorque = parser.ReadSingle();
            descriptor.EngineInertia = parser.ReadSingle();
            descriptor.GearboxInertia = parser.ReadSingle();
            descriptor.EngineIdleRevs = parser.ReadSingle();

            descriptor.TorqueCurveFunc = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.TireSlipCurveFunc = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.TireLoadCurveFunc = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.CamberThrustCurveFunc = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.CamberGripCurveFunc = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.FrontDownForceCurveFunc = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.RearDownForceCurveFunc = parser.ReadLine().Trim().Replace("\"", "");
            descriptor.RollResistanceCurveFunc = parser.ReadLine().Trim().Replace("\"", "");

            descriptor.NumWheels = parser.ReadInt();
            descriptor.WheelThickness = parser.ReadSingle();

            for (int i = 0; i < descriptor.NumWheels; i++)
            {
                WheelData wheelData = new WheelData();

                wheelData.MasterNode = parser.ReadLine().Trim().Replace("\"", "");
                wheelData.InstanceNode = parser.ReadLine().Trim().Replace("\"", "");
                wheelData.DrivingWheel = parser.ReadInt();
                wheelData.SteeringWheel = parser.ReadInt();
                wheelData.Mass = parser.ReadSingle();
                wheelData.XAxisMomentOfInertia = parser.ReadSingle();
                splitLine = parser.ReadLine().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                wheelData.SuspensionDirection = new Vector3(splitLine[0].ToSingle(), splitLine[1].ToSingle(), splitLine[2].ToSingle());
                wheelData.MaxSuspensionOffset = parser.ReadSingle();
                wheelData.MinSuspensionOffset = parser.ReadSingle();
                wheelData.CamberAngle = parser.ReadSingle();
                wheelData.Radius = parser.ReadSingle();
                wheelData.SuspensionSpringStiffness = parser.ReadSingle();
                wheelData.SuspensionShockDamping = parser.ReadSingle();
                descriptor.Wheels.Add(wheelData);
            }

            string cockpitIdentifier = parser.ReadLine().Trim().Replace("\"", "");
            if (cockpitIdentifier != "COCKPIT")
            {
                throw new ArgumentException($"Expected \"COCKPIT\" but got {cockpitIdentifier}!!!!");
            }
            descriptor.cockpitHierarchyFile = parser.ReadLine().Trim().Replace("\"", "");
            splitLine = parser.ReadLine().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            descriptor.cockpitCameraPos = new Vector3(splitLine[0].ToSingle(), splitLine[1].ToSingle(), splitLine[2].ToSingle());
            splitLine = parser.ReadLine().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            descriptor.CockpitLookAtPosition = new Vector3(splitLine[0].ToSingle(), splitLine[1].ToSingle(), splitLine[2].ToSingle());
            splitLine = parser.ReadLine().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            descriptor.CockpitCarmeraToSteeringWheelOffset = new Vector3(splitLine[0].ToSingle(), splitLine[1].ToSingle(), splitLine[2].ToSingle());
            descriptor.DriverNode = parser.ReadLine().Trim().Replace("\"", "");

            return descriptor;
        }

        public void Save(string path)
        {

            using (DocumentWriter writer = new DocumentWriter(path))
            {
                if (Tokens.Count > 0)
                {
                    writer.WriteLine("TOKENS_START");
                    foreach (var tokenKvp in Tokens)
                    {
                        string tokenValue = "";
                        if (tokenKvp.Value != null)
                        {
                            if (tokenKvp.Value.GetType() == typeof(string))
                            {
                                tokenValue = $"\"{tokenKvp.Value}\"";
                            }
                            else if (tokenKvp.Value.GetType() == typeof(float))
                            {
                                tokenValue = $"{tokenKvp.Value:F}";
                            }
                            else
                            {
                                tokenValue = $"{tokenKvp.Value}";
                            }
                        }

                        writer.WriteLine($"\t{tokenKvp.Key}\t{tokenValue}");
                    }
                    writer.WriteLine("TOKENS_END");
                }

                writer.WriteLine("// Texure Descriptor");
                writer.WriteLine($"\t\"{TextureDescriptorFile}\"");
                writer.WriteLine();

                writer.WriteLine("// Hierarchy filename");
                writer.WriteLine($"\t\"{HierarchyFile}\"");
                writer.WriteLine();

                writer.WriteLine("// Header filename");
                writer.WriteLine($"\t\"{HeaderFile}\"");
                writer.WriteLine();

                writer.WriteLine("// Dynamic object filename");
                writer.WriteLine($"\t\"{DynamicObjectFile}\"");
                writer.WriteLine();

                writer.WriteLine("// Shadow dcol filename");
                writer.WriteLine($"\t\"{ShadowDcolFile}\"");
                writer.WriteLine();

                writer.WriteLine("// deformation filename");
                writer.WriteLine($"\t\"{DeformationFile}\"");
                writer.WriteLine();

                writer.WriteLine("// Horn/Siren SFX");
                writer.WriteLine($"\t\"{HornSirenSfx}\"");
                writer.WriteLine();

                writer.WriteLine("// Engine SFX");
                writer.WriteLine($"\t\"{EngineSfx}\"");
                writer.WriteLine();

                writer.WriteLine("// Brake light texture name, top-left and bottom-right uv coords");
                writer.WriteLine($"\t\"{BrakeLightsTextureName}\"");
                writer.WriteLine($"\t{TopLeftUV.X:F}	{TopLeftUV.Y:F}");
                writer.WriteLine($"\t{BottomRightUV.X:F}	{BottomRightUV.Y:F}");
                writer.WriteLine();

                writer.WriteLine("// Brake light size (n x n metres)");
                writer.WriteLine($"\t{BrakeLightSize:F}");

                writer.WriteLine();

                writer.WriteLine("// Number of Brake lights");
                writer.WriteLine($"\t{NumBrakeLights}");

                for (int i = 0; i < NumBrakeLights; i++)
                {
                    writer.WriteLine($"\t\"{BrakeLightNodes[i]}\"");
                }

                writer.WriteLine();

                writer.WriteLine("// Number of Doors (pivot nodes)");

                writer.WriteLine($"\t{NumDoors}");

                for (int i = 0; i < NumDoors; i++)
                {
                    writer.WriteLine($"\t\"{DoorNodes[i]}\"");
                }
                writer.WriteLine();

                writer.WriteLine("// Number of bonnets (pivot nodes) (this includes teh front bonnet and the hatch at the back");

                writer.WriteLine($"\t{NumBonnets}");

                for (int i = 0; i < NumBonnets; i++)
                {
                    writer.WriteLine($"\t\"{BonnetNodes[i]}\"");
                }
                writer.WriteLine();

                writer.WriteLine("// Car shell node name");
                writer.WriteLine($"\t\"{ShellNode}\"");
                writer.WriteLine();

                writer.WriteLine("// Tow bar node name");
                writer.WriteLine($"\t\"{TowBarNode}\"");
                writer.WriteLine();

                writer.WriteLine("// Trailer Descriptor");
                writer.WriteLine($"\t\"{TrailerDescriptor}\"");

                writer.WriteLine();

                writer.WriteLine("// Engine fire and smoke nulls (0 to 4), the first is the engine, the others are extras for wasted");
                writer.WriteLine($"\t{NumEngineFireSmokeNodes}");

                for (int i = 0; i < NumEngineFireSmokeNodes; i++)
                {
                    writer.WriteLine($"\t\"{EngineFireSmokeNodes[i]}\"");
                }
                writer.WriteLine();

                writer.WriteLine("// Number of Gears");

                writer.WriteLine($"\t{NumGears}");
                writer.WriteLine();

                for (int i = 0; i < NumGears; i++)
                {
                    writer.WriteLine($"\t{Gears[i]:F}", i == 0 ? "Reverse" : null);
                }

                writer.WriteLine();

                writer.WriteLine("// Differential ratio");
                writer.WriteLine($"\t{DifferntialRatio:F}");
                writer.WriteLine();

                writer.WriteLine("// Clutch torque (Nm)");
                writer.WriteLine($"\t{ClutchTorque:F}");
                writer.WriteLine();

                writer.WriteLine("// Engine inertia");
                writer.WriteLine($"\t{EngineInertia:F}");
                writer.WriteLine();

                writer.WriteLine("// Gearbox inertia");
                writer.WriteLine($"\t{GearboxInertia:F}");
                writer.WriteLine();

                writer.WriteLine("// Engine idle revs");
                writer.WriteLine($"\t{EngineIdleRevs:F}");

                writer.WriteLine();

                writer.WriteLine("// Name of torque curve function");
                writer.WriteLine($"\t\"{TorqueCurveFunc}\"");
                writer.WriteLine();

                writer.WriteLine("// Name of tire slip curve function");
                writer.WriteLine($"\t\"{TireSlipCurveFunc}\"");
                writer.WriteLine();

                writer.WriteLine("// Name of tire load curve function");
                writer.WriteLine($"\t\"{TireLoadCurveFunc}\"");
                writer.WriteLine();

                writer.WriteLine("// Name of camber thrust curve");
                writer.WriteLine($"\t\"{CamberThrustCurveFunc}\"");
                writer.WriteLine();

                writer.WriteLine("// Name of camber grip curve");
                writer.WriteLine($"\t\"{CamberGripCurveFunc}\"");
                writer.WriteLine();

                writer.WriteLine("// Name of Front Down force curve");
                writer.WriteLine($"\t\"{FrontDownForceCurveFunc}\"");
                writer.WriteLine();

                writer.WriteLine("// Name of Rear Down force curve");
                writer.WriteLine($"\t\"{RearDownForceCurveFunc}\"");
                writer.WriteLine();

                writer.WriteLine("// Name of Roll resistance curve");
                writer.WriteLine($"\t\"{RollResistanceCurveFunc}\"");

                writer.WriteLine();
                writer.WriteLine();

                writer.WriteLine("//=============================================================================");
                writer.WriteLine("// Wheel specific data ");
                writer.WriteLine("//=============================================================================");
                writer.WriteLine();

                writer.WriteLine("// Number of wheels");
                writer.WriteLine($"\t{NumWheels}");
                writer.WriteLine();

                writer.WriteLine("// Wheel thickness (m)");
                writer.WriteLine($"\t{WheelThickness:F}");

                writer.WriteLine();

                writer.WriteLine("// Wheel Data ");
                for (int i = 0; i < NumWheels; i++)
                {
                    WheelData wheelData = Wheels[i];

                    writer.WriteLine();

                    writer.WriteLine("// Centered hierarchy node ");
                    writer.WriteLine($"\t\"{wheelData.MasterNode}\"");
                    writer.WriteLine();

                    writer.WriteLine("// Offset instance node ");
                    writer.WriteLine($"\t\"{wheelData.InstanceNode}\"");
                    writer.WriteLine();

                    writer.WriteLine("// Driving wheel ");
                    writer.WriteLine($"\t{wheelData.DrivingWheel}");
                    writer.WriteLine();

                    writer.WriteLine("// Steering wheel ");
                    writer.WriteLine($"\t{wheelData.SteeringWheel}");
                    writer.WriteLine();

                    writer.WriteLine("// Mass (Kg) ");
                    writer.WriteLine($"\t{wheelData.Mass:F}");
                    writer.WriteLine();

                    writer.WriteLine("// x-axis moment of inertia ");
                    writer.WriteLine($"\t{wheelData.XAxisMomentOfInertia:F}");

                    writer.WriteLine();

                    writer.WriteLine("// Suspension direction ");
                    writer.WriteLine($"\t{wheelData.SuspensionDirection.X:F}	{wheelData.SuspensionDirection.Y:F}	{wheelData.SuspensionDirection.Z:F}");
                    writer.WriteLine();

                    writer.WriteLine("// Maximum suspension offset ");
                    writer.WriteLine($"\t{wheelData.MaxSuspensionOffset:F}");
                    writer.WriteLine();

                    writer.WriteLine("// Minimum suspension offset ");
                    writer.WriteLine($"\t{wheelData.MinSuspensionOffset:F}");
                    writer.WriteLine();

                    writer.WriteLine("// Camber angle ");
                    writer.WriteLine($"\t{wheelData.CamberAngle:F}");
                    writer.WriteLine();

                    writer.WriteLine("// Radius (m) ");
                    writer.WriteLine($"\t{wheelData.Radius:F}");
                    writer.WriteLine();

                    writer.WriteLine("// Suspension spring stiffness (Nm-1) ");
                    writer.WriteLine($"\t{wheelData.SuspensionSpringStiffness:F}");
                    writer.WriteLine();

                    writer.WriteLine("// Suspension shock absorber damping (Nsm-1) ");
                    writer.WriteLine($"\t{wheelData.SuspensionShockDamping:F}");

                    writer.WriteLine();
                    writer.WriteLine();

                    writer.WriteLine("//---------------------------------------------------------------------------");
                }

                writer.WriteLine();
                writer.WriteLine("COCKPIT");
                writer.WriteLine();

                writer.WriteLine("// cockpit hierarchy to use");
                writer.WriteLine($"\t\"{cockpitHierarchyFile}\"");
                writer.WriteLine();

                writer.WriteLine("// cockpit camera position in carspace x y z ");
                writer.WriteLine($"\t{cockpitCameraPos.X:F}	{cockpitCameraPos.Y:F}	{cockpitCameraPos.Z:F}");
                writer.WriteLine();

                writer.WriteLine("// cockpit camera lookAt position ");
                writer.WriteLine($"\t{CockpitLookAtPosition.X:F}	{CockpitLookAtPosition.Y:F}	{CockpitLookAtPosition.Z:F}");
                writer.WriteLine();

                writer.WriteLine("// camera to steering wheel offset ");
                writer.WriteLine($"\t{CockpitCarmeraToSteeringWheelOffset.X:F}	{CockpitCarmeraToSteeringWheelOffset.Y:F}	{CockpitCarmeraToSteeringWheelOffset.Z:F}");
                writer.WriteLine();

                writer.WriteLine($"\t\"{DriverNode}\"");
            }
        }
    }
}
