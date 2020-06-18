using System;
using System.Collections.Generic;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Carmageddon2.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class Map
    {
        public int Version { get; set; }

        public Vector3 LightColour { get; set; }

        public Vector2 DiffuseLight0 { get; set; }

        public Vector2 DiffuseLight1 { get; set; }

        public Vector2 DiffuseLightOther { get; set; }

        public Vector3 GridPosition { get; set; }

        public int GridRotation { get; set; }

        public List<Checkpoint> Checkpoints { get; set; } = new List<Checkpoint>();

        public List<SmashData> Smashables { get; set; } = new List<SmashData>();

        public List<PedSpec> PedSpecs { get; set; } = new List<PedSpec>();

        public string AdditionalActor { get; set; }

        public string SkyPixelmap { get; set; }

        public int HorizontalRepetitions { get; set; }

        public int VerticalSize { get; set; }

        public int PositionOfHorizon { get; set; }

        public string DepthCueMode { get; set; }

        public Vector2 FogDarkness { get; set; }

        public Vector3 DepthCueColour { get; set; }

        public int DefaultEngineNoise { get; set; }

        public List<SpecialEffectVolume> SpecialVolumes { get; set; } = new List<SpecialEffectVolume>();

        public List<object> SoundGenerators { get; set; } = new List<object>();

        public string MaterialDefault { get; set; }

        public string MaterialDarkness { get; set; }

        public string MaterialFog { get; set; }

        public string MapPixelmap { get; set; }

        public Matrix3D WorldMapTransform { get; set; }

        public List<Funk> Funks { get; set; } = new List<Funk>();

        public List<Groove> Grooves { get; set; } = new List<Groove>();

        public List<Vector3> Nodes { get; set; } = new List<Vector3>();

        public List<OpponentPathSection> Paths { get; set; } = new List<OpponentPathSection>();

        public List<object> CopStartPoints { get; set; } = new List<object>();

        public int DronePathVersion { get; set; }

        public List<DronePathNode> DronePaths { get; set; } = new List<DronePathNode>();

        public List<MaterialModifier> MaterialModifiers { get; set; } = new List<MaterialModifier>();

        public List<string> Noncars { get; set; } = new List<string>();

        public List<ShadeTable> ShadeTables { get; set; } = new List<ShadeTable>();

        public List<NetworkStart> NetworkStarts { get; set; } = new List<NetworkStart>();

        public List<string> SplashFiles { get; set; } = new List<string>();

        public List<string> TxtFiles { get; set; } = new List<string>();

        public static Map Load(string path)
        {
            DocumentParser file = new DocumentParser(path);
            Map map = new Map();

            string version = file.ReadLine();

            if (!version.StartsWith("VERSION"))
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            map.Version = version.Replace("VERSION ", "").ToInt();

            if (map.Version == 1)
            {
                // V1 has global lighting data
                map.LightColour = file.ReadVector3();       // RGB for main directional light-source
                map.DiffuseLight0 = file.ReadVector2();     // Ambient/Diffuse light to be used when plaything ambient says 0
                map.DiffuseLight1 = file.ReadVector2();     // Ambient/Diffuse light to be used when plaything ambient says 1
                map.DiffuseLightOther = file.ReadVector2(); // Ambient/Diffuse light to be used when plaything ambient says anything else
            }

            map.GridPosition = file.ReadVector3();          // Position of centre of start of grid
            map.GridRotation = file.ReadInt();              // Direction that grid faces in

            //string t = getNextLine(sr);

            //if (version == 8)
            //{
            //    // alpha and demo v8 files had an extra block here, we'll test if it
            //    if (t.Contains(","))
            //    {
            //        getNextLine(sr); // # laps
            //        getNextLine(sr); // Race completed bonus (all laps raced) for each skill level
            //        getNextLine(sr); // Race completed bonus (all peds killed) for each skill level
            //        getNextLine(sr); // Race completed bonus (all oppos wasted) for each skill level

            //        t = getNextLine(sr);
            //    }
            //}

            int checkpointCount = file.ReadInt();
            for (int i = 0; i < checkpointCount; i++)
            {
                Checkpoint cp = new Checkpoint() { TimerIncrements = file.ReadVector3() };
                int quadCount = file.ReadInt();

                for (int j = 0; j < quadCount; j++)
                {
                    cp.Points.Add(file.ReadVector3());
                    cp.Points.Add(file.ReadVector3());
                    cp.Points.Add(file.ReadVector3());
                    cp.Points.Add(file.ReadVector3());
                }

                map.Checkpoints.Add(cp);
            }

            int smashSpecs = file.ReadInt();
            for (int i = 0; i < smashSpecs; i++)
            {
                SmashData smashable = new SmashData
                {
                    Flags = file.ReadInt(),
                    Trigger = file.ReadLine()
                };

                if (smashable.Trigger.Length == 3 && smashable.Trigger.StartsWith("&"))
                {
                    smashable.TriggerFlags = file.ReadInt();
                }

                smashable.TriggerMode = file.ReadEnum<SmashData.SmashTriggerMode>();

                switch (smashable.TriggerMode)
                {
                    case SmashData.SmashTriggerMode.nochange:
                    case SmashData.SmashTriggerMode.remove:
                        smashable.RemovalThreshold = file.ReadSingle();
                        smashable.Connotations.Load(file);
                        break;

                    case SmashData.SmashTriggerMode.replacemodel:
                        smashable.RemovalThreshold = file.ReadSingle();
                        smashable.Connotations.Load(file);
                        smashable.NewModel = file.ReadLine();
                        smashable.ChanceOfFire = file.ReadInt();

                        if (smashable.ChanceOfFire > 0)
                        {
                            smashable.NumFires = file.ReadInt();
                            smashable.SmokeLevel = file.ReadInts();
                        }
                        break;

                    case SmashData.SmashTriggerMode.texturechange:
                        smashable.IntactMaterial = file.ReadLine();
                        int textureLevels = file.ReadInt();

                        for (int j = 0; j < textureLevels; j++)
                        {
                            SmashDataTextureLevel textureLevel = new SmashDataTextureLevel()
                            {
                                TriggerThreshold = file.ReadSingle(),
                                Flags = file.ReadInt(),
                                CollisionType = file.ReadEnum<SmashDataTextureLevel.TextureLevelCollisionType>()
                            };

                            textureLevel.Connotations.Load(file);

                            int pixelmaps = file.ReadInt();
                            for (int k = 0; k < pixelmaps; k++)
                            {
                                textureLevel.Pixelmaps.Add(file.ReadLine());
                            }

                            smashable.Levels.Add(textureLevel);
                        }
                        break;

                    default:
                        throw new NotImplementedException($"Unknown TriggerMode '{smashable.TriggerMode}'");
                }

                smashable.Reserved1 = file.ReadInt();
                smashable.Reserved2 = file.ReadInt();
                smashable.Reserved3 = file.ReadInt();
                smashable.Reserved4 = file.ReadInt();

                map.Smashables.Add(smashable);
            }

            int pedSpecs = file.ReadInt();

            for (int i = 0; i < pedSpecs; i++)
            {
                PedSpec ps = new PedSpec()
                {
                    MaterialName = file.ReadLine(),
                    MovementIndex = file.ReadInt(),
                    GroupIndex = file.ReadInt(),
                    PedsPer100SquareMetres = file.ReadSingle()
                };

                int exclusionCount = file.ReadInt();
                for (int j = 0; j < exclusionCount; j++)
                {
                    ps.ExclusionMaterials.Add(new PedExclusionMaterial
                    {
                        Flags = file.ReadInt(),
                        Name = file.ReadLine()
                    });
                }

                int exceptionCount = file.ReadInt();
                for (int j = 0; j < exceptionCount; j++)
                {
                    ps.ExceptionMaterials.Add(file.ReadLine());
                }

                map.PedSpecs.Add(ps);
            }

            map.AdditionalActor = file.ReadLine();

            map.SkyPixelmap = file.ReadLine();
            map.HorizontalRepetitions = file.ReadInt();
            map.VerticalSize = file.ReadInt();
            map.PositionOfHorizon = file.ReadInt();
            map.DepthCueMode = file.ReadLine();
            map.FogDarkness = file.ReadVector2();
            map.DepthCueColour = file.ReadVector3();

            map.DefaultEngineNoise = file.ReadInt();

            int specialEffectVolumeCount = file.ReadInt();

            for (int i = 0; i < specialEffectVolumeCount; i++)
            {
                SpecialEffectVolume sev = new SpecialEffectVolume { Type = file.ReadLine() };

                if (sev.Type.ToUpper() == "BOX")
                {
                    sev.Corners.Add(file.ReadVector3());
                    sev.Corners.Add(file.ReadVector3());
                    sev.Corners.Add(file.ReadVector3());
                    sev.Corners.Add(file.ReadVector3());
                }

                sev.GravityMultiplier = file.ReadSingle();
                sev.ViscosityMultiplier = file.ReadSingle();
                sev.CarDamagePerMillisecond = file.ReadSingle();
                sev.PedDamagePerMillisecond = file.ReadSingle();
                sev.CameraEffectIndex = file.ReadInt();

                if (file.PeekLine().Contains(","))
                {
                    sev.SkyColourRGB = file.ReadVector3();
                }
                else
                {
                    sev.SkyColour = file.ReadInt();
                }
                
                sev.WindscreenMaterial = file.ReadLine();
                sev.EntrySoundID = file.ReadInt();
                sev.ExitSoundID = file.ReadInt();
                sev.EngineNoiseIndex = file.ReadInt();
                sev.MaterialIndex = file.ReadInt();

                if (sev.Type.ToUpper() == "BOX")
                {
                    sev.SoundType = file.ReadEnum<SpecialEffectVolume.SpecVolSoundType>();

                    if (sev.SoundType != SpecialEffectVolume.SpecVolSoundType.NONE)
                    {
                        sev.SoundSpec = SoundSpec.Load(file);
                    }
                }

                map.SpecialVolumes.Add(sev);
            }

            int soundGeneratorCount = file.ReadInt();

            if (soundGeneratorCount > 0)
            {
                throw new NotImplementedException("Can't handle sound generators yet!");
            }

            map.MaterialDefault = file.ReadLine();
            map.MaterialDarkness = file.ReadLine();
            map.MaterialFog = file.ReadLine();

            file.ReadLine();    // (ignore) # areas with different screens

            map.MapPixelmap = file.ReadLine();
            map.WorldMapTransform = new Matrix3D(
                file.ReadVector3(),
                file.ReadVector3(),
                file.ReadVector3(),
                file.ReadVector3()
            );

            if (file.ReadLine() != "START OF FUNK")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Map.TXT file?", "START OF FUNK");
                return null;
            }

            while (file.PeekLine() != "END OF FUNK")
            {
                map.Funks.Add(new Funk(file));
                if (file.PeekLine() == "NEXT FUNK") { file.ReadLine(); }
            }
            file.ReadLine();

            if (file.ReadLine() != "START OF GROOVE")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Map.TXT file?", "START OF GROOVE");
                return null;
            }

            while (file.PeekLine() != "END OF GROOVE")
            {
                map.Grooves.Add(new Groove(file));
                if (file.PeekLine() == "NEXT GROOVE") { file.ReadLine(); }
            }
            file.ReadLine();

            if (file.ReadLine() != "START OF OPPONENT PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            int nodeCount = file.ReadInt();
            for (int i = 0; i < nodeCount; i++)
            {
                map.Nodes.Add(file.ReadVector3());
            }

            int pathCount = file.ReadInt();
            for (int i = 0; i < pathCount; i++)
            {
                string[] parts = file.ReadLine().Split(',');

                map.Paths.Add(new OpponentPathSection
                {
                    StartNode = parts[0].ToInt(),
                    EndNode = parts[1].ToInt(),
                    Unknown1 = parts[2].ToInt(),
                    Unknown2 = parts[3].ToInt(),
                    Unknown3 = parts[4].ToInt(),
                    Unknown4 = parts[5].ToInt(),
                    Width = parts[6].ToSingle(),
                    SectionType = parts[7].ToInt()
                });
            }

            int copStartPoints = file.ReadInt();
            if (copStartPoints > 0) { throw new NotImplementedException("Cop start points?  Really?"); }

            if (file.ReadLine() != "END OF OPPONENT PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            if (file.ReadLine() != "START OF DRONE PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            map.DronePathVersion = file.ReadInt();

            int dronepathNodeCount = file.ReadInt();
            for (int i = 0; i < dronepathNodeCount; i++)
            {
                DronePathNode node = new DronePathNode
                {
                    Position = file.ReadVector3(),
                    DroneName = file.ReadLine()
                };

                if (map.DronePathVersion == 0)
                {
                    node.UnknownVector = file.ReadVector3();
                }
                else
                {
                    node.UnknownInt = file.ReadInt();
                }

                int nextNodeCount = file.ReadInt();
                for (int j = 0; j < nextNodeCount; j++)
                {
                    node.Destinations.Add(file.ReadLine());
                }

                map.DronePaths.Add(node);
            }

            if (file.ReadLine() != "END OF DRONE PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            int materialModifierCount = file.ReadInt();
            for (int i = 0; i < materialModifierCount; i++)
            {
                map.MaterialModifiers.Add(new MaterialModifier
                {
                    CarWallFriction = file.ReadSingle(),
                    TyreRoadFriction = file.ReadSingle(),
                    DownForce = file.ReadSingle(),
                    Bumpiness = file.ReadSingle(),
                    TyreSoundIndex = file.ReadInt(),
                    CrashSoundIndex = file.ReadInt(),
                    ScrapeNoiseIndex = file.ReadInt(),
                    Sparkiness = file.ReadSingle(),
                    RoomForExpansion = file.ReadInt(),
                    SkidmarkMaterial = file.ReadLine()
                });
            }

            int noncarCount = file.ReadInt();
            for (int i = 0; i < noncarCount; i++)
            {
                map.Noncars.Add(file.ReadLine());
            }

            int shadetableCount = file.ReadInt();
            for (int i = 0; i < shadetableCount; i++)
            {
                map.ShadeTables.Add(new ShadeTable
                {
                    RGB = file.ReadVector3(),
                    Strengths = file.ReadVector3()
                });
            }

            int networkStartCount = file.ReadInt();
            for (int i = 0; i < networkStartCount; i++)
            {
                map.NetworkStarts.Add(new NetworkStart
                {
                    Position = file.ReadVector3(),
                    Rotation = file.ReadInt()
                });
            }

            int splashfileCount = file.ReadInt();
            for (int i = 0; i < splashfileCount; i++)
            {
                map.SplashFiles.Add(file.ReadLine());
            }

            int txtfileCount = file.ReadInt();
            for (int i = 0; i < txtfileCount; i++)
            {
                map.TxtFiles.Add(file.ReadLine());
            }

            return map;
        }

        public void Save(string path)
        {
            using (DocumentWriter dw = new DocumentWriter(path))
            {
                dw.WriteLine($"VERSION {Version}", Version > 1 ? "V1 expects map checkpoint rectangles" : null);
                if (Version >= 2) { dw.WriteLine(null, "V2 and above expect map checkpoint points"); }
                if (Version >= 3) { dw.WriteLine(null, "V3 and above expect splash file stuff at the end"); }
                if (Version >= 4) { dw.WriteLine(null, "V4 and above expect two sets of PIX and MAT and ped substitution entry"); }
                dw.WriteLine();

                if (Version == 1)
                {
                    dw.WriteLine();
                    dw.WriteLine("//////////// GLOBAL LIGHTING DATA ///////////");
                    dw.WriteLine();
                    dw.WriteLine($"{LightColour.X},{LightColour.Y},{LightColour.Z}", "RGB for main directional light-source");
                    dw.WriteLine($"{DiffuseLight0.X:F1},{DiffuseLight0.Y:F1}", "Ambient/Diffuse light to be used when plaything ambient says 0				}--- Neither of these have smooth lighting effecs applied");
                    dw.WriteLine($"{DiffuseLight1.X:F1},{DiffuseLight1.Y:F1}", "Ambient/Diffuse light to be used when plaything ambient says 1				}");
                    dw.WriteLine($"{DiffuseLightOther.X:F1},{DiffuseLightOther.Y:F1}", "Ambient/Diffuse light to be used when plaything ambient says anything else");
                    dw.WriteLine();
                    dw.WriteLine("/////////////////////////////////////////////");
                    dw.WriteLine();
                }

                dw.WriteLine();

                dw.WriteLine($"{GridPosition.X},{GridPosition.Y},{GridPosition.Z}", "Position of centre of start of grid");
                dw.WriteLine($"{GridRotation}", "Direction that grid faces in");

                dw.WriteLine();
                dw.WriteLine("// Laps, checkpoints etc");
                dw.WriteLine();

                dw.WriteLine($"{Checkpoints.Count}", "# checkpoints");
                for (int i = 0; i < Checkpoints.Count; i++)
                {
                    Checkpoint checkpoint = Checkpoints[i];

                    dw.WriteLine($"// Checkpoint #{i + 1}");
                    dw.WriteLine($"{checkpoint.TimerIncrements.X},{checkpoint.TimerIncrements.Y},{checkpoint.TimerIncrements.Z}", "Timer increment for each skill level (ped mode)");
                    dw.WriteLine($"{checkpoint.Points.Count / 4}", "# quads for this checkpoint");

                    for (int j = 0; j < checkpoint.Points.Count; j++)
                    {
                        Vector3 point = checkpoint.Points[j];

                        dw.WriteLine($"{point.X},{point.Y},{point.Z}", $"Point #{j}");
                    }
                }

                dw.WriteLine();
                dw.WriteLine("// Smashable environment specs");
                dw.WriteLine();

                dw.WriteLine($"{Smashables.Count}", "Number of smash specs");
                dw.WriteLine();

                foreach (SmashData smash in Smashables)
                {
                    dw.WriteLine("// Start of smashable item");
                    dw.WriteLine($"{smash.Flags}", "Flags");
                    dw.WriteLine($"{smash.Trigger}", "Name of trigger material");
                    dw.WriteLine($"{smash.TriggerMode}", "Mode");

                    switch (smash.TriggerMode)
                    {
                        case SmashData.SmashTriggerMode.nochange:
                        case SmashData.SmashTriggerMode.remove:
                            dw.WriteLine($"{smash.RemovalThreshold}", "Removal threshold");
                            dw.IncreaseIndent();
                            smash.Connotations.Write(dw);
                            dw.DecreaseIndent();
                            break;

                        case SmashData.SmashTriggerMode.replacemodel:
                            dw.WriteLine($"{smash.RemovalThreshold}", "Removal threshold");
                            dw.WriteLine();
                            dw.IncreaseIndent();
                            smash.Connotations.Write(dw);
                            dw.DecreaseIndent();
                            dw.WriteLine($"{smash.NewModel}", "new model");
                            dw.WriteLine($"{smash.ChanceOfFire}", "%chance fire");

                            if (smash.ChanceOfFire > 0)
                            {
                                dw.WriteLine($"{smash.NumFires}");
                                dw.WriteLine($"{string.Join(",", smash.SmokeLevel)}");
                            }
                            break;

                        case SmashData.SmashTriggerMode.texturechange:
                            dw.WriteLine($"{smash.IntactMaterial}", "Intact pixelmap");
                            dw.WriteLine($"{smash.Levels.Count}", "Number of levels");

                            int j = 1;

                            foreach (SmashDataTextureLevel textureLevel in smash.Levels)
                            {
                                dw.WriteLine();
                                dw.WriteLine($"{textureLevel.TriggerThreshold}", "Trigger threshold");
                                dw.WriteLine($"{textureLevel.Flags}", "Flags");
                                dw.WriteLine($"{textureLevel.CollisionType}", "Collision type");
                                dw.IncreaseIndent();
                                textureLevel.Connotations.Write(dw);
                                dw.DecreaseIndent();
                                dw.WriteLine();
                                dw.WriteLine($"{textureLevel.Pixelmaps.Count}", "Number of pixelmap");
                                foreach (string pixelmap in textureLevel.Pixelmaps) { dw.WriteLine($"{pixelmap}", $"Pixelmap"); }

                                j++;
                            }
                            break;
                    }

                    dw.WriteLine($"{smash.Reserved1}", "reserved 1");
                    dw.WriteLine($"{smash.Reserved2}", "reserved 2");
                    dw.WriteLine($"{smash.Reserved3}", "reserved 3");
                    dw.WriteLine($"{smash.Reserved4}", "reserved 4");
                }

                dw.WriteLine();
                dw.WriteLine("// Ped specs");
                dw.WriteLine();

                dw.WriteLine($"{PedSpecs.Count}");

                foreach (PedSpec pedspec in PedSpecs)
                {
                    dw.WriteLine();
                    dw.WriteLine($"{pedspec.MaterialName}", "Material name");
                    dw.WriteLine($"{pedspec.MovementIndex}", "Movement index");
                    dw.WriteLine($"{pedspec.GroupIndex}", "Group index");
                    dw.WriteLine($"{pedspec.PedsPer100SquareMetres}", "Peds per 100 square metres");
                    dw.WriteLine($"{pedspec.ExclusionMaterials.Count}", "Number of exclusion materials");

                    foreach (PedExclusionMaterial pedExclusionMaterial in pedspec.ExclusionMaterials)
                    {
                        dw.WriteLine($"{pedExclusionMaterial.Flags}", "Exclusion flags (1 = OK when scared)");
                        dw.WriteLine($"{pedExclusionMaterial.Name}", "Exclusion material #1 name");
                    }

                    dw.WriteLine($"{pedspec.ExceptionMaterials.Count}", "Number of exception materials");
                }

                dw.WriteLine();

                dw.WriteLine($"{AdditionalActor}", "Additional actor");

                dw.WriteLine();
                dw.WriteLine("// HORIZON STUFF");
                dw.WriteLine();

                dw.WriteLine($"{SkyPixelmap}", @"Name of sky texture pixelmap (or ""none"")");
                dw.WriteLine($"{HorizontalRepetitions}", "Horizontal repetitions of sky texture");
                dw.WriteLine($"{VerticalSize}", "Vertical size of sky texture (degrees)");
                dw.WriteLine($"{PositionOfHorizon}", "Position of horizon (pixels below top)");
                dw.WriteLine($"{DepthCueMode}", @"Depth cue mode (""none"", ""dark"" or ""fog"")");
                dw.WriteLine($"{FogDarkness.X},{FogDarkness.Y}", "Degree of fog/darkness");
                dw.WriteLine($"{DepthCueColour.X},{DepthCueColour.Y},{DepthCueColour.Z}", "Depth cue colour (red, green, blue )");

                dw.WriteLine();
                dw.WriteLine("// DEFAULT ENGINE NOISE");
                dw.WriteLine();

                dw.WriteLine($"{DefaultEngineNoise}");

                dw.WriteLine();
                dw.WriteLine("// SPECIAL EFFECTS VOLUMES");
                dw.WriteLine();

                dw.WriteLine($"{SpecialVolumes.Count}", "# special effects volumes");

                foreach (SpecialEffectVolume volume in SpecialVolumes)
                {
                    dw.WriteLine();
                    dw.WriteLine($"{volume.Type}");

                    foreach (Vector3 corner in volume.Corners)
                    {
                        dw.WriteLine($"{corner.X:F3}, {corner.Y:F3}, {corner.Z:F3}");
                    }

                    dw.WriteLine($"{volume.GravityMultiplier:F2}", "gravity multiplier");
                    dw.WriteLine($"{volume.ViscosityMultiplier:F2}", "viscosity multiplier");
                    dw.WriteLine($"{volume.CarDamagePerMillisecond:F2}", "Car damage per millisecond");
                    dw.WriteLine($"{volume.PedDamagePerMillisecond:F2}", "Pedestrian damage per millisecond");
                    dw.WriteLine($"{volume.CameraEffectIndex}", "camera effect index");
                    dw.WriteLine($"{volume.SkyColour}", "sky colour");
                    dw.WriteLine($"{volume.WindscreenMaterial}", "Windscreen texture to use");
                    dw.WriteLine($"{volume.EntrySoundID}", "Sound ID of entry noise");
                    dw.WriteLine($"{volume.ExitSoundID}", "Sound ID of exit noise");
                    dw.WriteLine($"{volume.EngineNoiseIndex}", "Engine noise index");
                    dw.WriteLine($"{volume.MaterialIndex}", "material index");

                    if (volume.Type.ToUpper() == "BOX")
                    {
                        dw.WriteLine($"{volume.SoundType}", "Sound type");

                        if (volume.SoundType != SpecialEffectVolume.SpecVolSoundType.NONE)
                        {
                            volume.SoundSpec.Write(dw);
                        }
                    }
                }

                dw.WriteLine();
                dw.WriteLine("// SOUND GENERATORS");
                dw.WriteLine();

                dw.WriteLine($"{SoundGenerators.Count}", "Number of generators");

                dw.WriteLine();
                dw.WriteLine("// REFLECTIVE WINDSCREEN SPECIFICATIONS");
                dw.WriteLine();

                dw.WriteLine($"{MaterialDefault}", "Material to use for default screens");
                dw.WriteLine($"{MaterialDarkness}", "Material to use for default screens during darkness");
                dw.WriteLine($"{MaterialFog}", "Material to use for default screens during fog");

                dw.WriteLine();

                dw.WriteLine("0", "(ignore) # areas with different screens");

                dw.WriteLine();
                dw.WriteLine("// MAP DETAILS");
                dw.WriteLine();

                dw.WriteLine($"{MapPixelmap}", "Map pixelmap name");
                dw.WriteLine();
                dw.WriteLine($"{WorldMapTransform.M11,7:0.###},{WorldMapTransform.M12,11:0.###},{WorldMapTransform.M13,11:0.###}", "World->map transformation matrix");
                dw.WriteLine($"{WorldMapTransform.M21,7:0.###},{WorldMapTransform.M22,11:0.###},{WorldMapTransform.M23,11:0.###}");
                dw.WriteLine($"{WorldMapTransform.M31,7:0.###},{WorldMapTransform.M32,11:0.###},{WorldMapTransform.M33,11:0.###}");
                dw.WriteLine($"{WorldMapTransform.M41,7:0.###},{WorldMapTransform.M42,11:0.###},{WorldMapTransform.M43,11:0.###}");

                dw.WriteLine();
                dw.WriteLine("// ****** START OF FUNK AND GROOVE STUFF ******");
                dw.WriteLine();

                dw.WriteLine();
                dw.WriteLine("START OF FUNK");
                dw.WriteLine();

                for (int i = 0; i < Funks.Count; i++)
                {
                    Funk funk = Funks[i];

                    dw.WriteLine($"{funk.Material}");
                    dw.WriteLine($"{funk.Mode}");
                    dw.WriteLine($"{funk.MatrixModType}");
                    if (funk.MatrixModType != FunkMatrixMode.None) { dw.WriteLine($"{funk.MatrixModMode}"); }

                    switch (funk.MatrixModType)
                    {
                        case FunkMatrixMode.roll:
                            dw.WriteLine($"{funk.RollPeriods.X},{funk.RollPeriods.Y}");
                            break;

                        case FunkMatrixMode.slither:
                            dw.WriteLine($"{funk.SlitherSpeed.X},{funk.SlitherSpeed.Y}");
                            dw.WriteLine($"{funk.SlitherAmount.X},{funk.SlitherAmount.Y}");
                            break;

                        case FunkMatrixMode.spin:
                            dw.WriteLine($"{funk.SpinPeriod}");
                            break;
                    }

                    dw.WriteLine($"{funk.LightingMode}");
                    dw.WriteLine($"{funk.AnimationType}");

                    switch (funk.AnimationType)
                    {
                        case FunkAnimationType.frames:
                            dw.WriteLine($"{funk.Framerate}");
                            dw.WriteLine($"{funk.FrameMode}");

                            switch (funk.FrameMode)
                            {
                                case FrameType.texturebits:
                                    dw.WriteLine($"{funk.TextureBitMode}");
                                    break;

                                case FrameType.continuous:
                                    dw.WriteLine($"{funk.FrameSpeed}");
                                    break;
                            }

                            dw.WriteLine($"{funk.Frames.Count}");
                            foreach (string frame in funk.Frames)
                            {
                                dw.WriteLine($"{frame}");
                            }
                            break;
                    }

                    if (i + 1 != Funks.Count)
                    {
                        dw.WriteLine();
                        dw.WriteLine("NEXT FUNK");
                        dw.WriteLine();
                    }
                }

                dw.WriteLine();
                dw.WriteLine("END OF FUNK");
                dw.WriteLine();

                dw.WriteLine();
                dw.WriteLine("START OF GROOVE");
                dw.WriteLine();

                for (int i = 0; i < Grooves.Count; i++)
                {
                    Groove groove = Grooves[i];

                    dw.WriteLine($"{groove.Part}", "Actor name of moving part");
                    dw.WriteLine($"{groove.LollipopMode}");
                    dw.WriteLine($"{groove.Mode}");
                    dw.WriteLine($"{groove.PathType}");
                    if (groove.PathType != GroovePathNames.None) { dw.WriteLine($"{groove.PathMode}"); }

                    switch (groove.PathType)
                    {
                        case GroovePathNames.straight:
                            dw.WriteLine($"{groove.PathCentre.X},{groove.PathCentre.Y},{groove.PathCentre.Z}");
                            dw.WriteLine($"{groove.PathPeriod}");
                            dw.WriteLine($"{groove.PathDelta.X},{groove.PathDelta.Y},{groove.PathDelta.Z}");
                            break;
                    }

                    dw.WriteLine($"{groove.AnimationType}");
                    if (groove.AnimationType != GrooveAnimation.None) { dw.WriteLine($"{groove.AnimationMode}"); }

                    switch (groove.AnimationType)
                    {
                        case GrooveAnimation.rock:
                            dw.WriteLine($"{groove.AnimationPeriod}");
                            dw.WriteLine($"{groove.AnimationCentre.X},{groove.AnimationCentre.Y},{groove.AnimationCentre.Z}");
                            dw.WriteLine($"{groove.AnimationAxis}");
                            dw.WriteLine($"{groove.RockMaxAngle}");
                            break;

                        case GrooveAnimation.shear:
                            dw.WriteLine($"{groove.ShearPeriod.X},{groove.ShearPeriod.Y},{groove.ShearPeriod.Z}");
                            dw.WriteLine($"{groove.AnimationCentre.X},{groove.AnimationCentre.Y},{groove.AnimationCentre.Z}");
                            dw.WriteLine($"{groove.ShearMagnitude.X},{groove.ShearMagnitude.Y},{groove.ShearMagnitude.Z}");
                            break;

                        case GrooveAnimation.spin:
                            dw.WriteLine($"{groove.AnimationPeriod}");
                            dw.WriteLine($"{groove.AnimationCentre.X},{groove.AnimationCentre.Y},{groove.AnimationCentre.Z}");
                            dw.WriteLine($"{groove.AnimationAxis}");
                            break;
                    }

                    if (i + 1 != Grooves.Count)
                    {
                        dw.WriteLine();
                        dw.WriteLine("NEXT GROOVE");
                        dw.WriteLine();
                    }
                }

                dw.WriteLine();
                dw.WriteLine("END OF GROOVE");
                dw.WriteLine();
                dw.WriteLine("START OF OPPONENT PATHS");
                dw.WriteLine();

                dw.WriteLine($"{Nodes.Count}", "Number of path nodes");

                int n = 0;
                foreach (Vector3 node in Nodes)
                {
                    dw.WriteLine($"{node.X,9:F3},{node.Y,9:F3},{node.Z,9:F3}", $"Node #{n}");
                    n++;
                }

                dw.WriteLine();

                dw.WriteLine($"{Paths.Count}", "Number of path sections");

                n = 0;
                foreach (OpponentPathSection pathSection in Paths)
                {
                    dw.WriteLine($"{pathSection.StartNode,4},{pathSection.EndNode,4},{pathSection.Unknown1,4},{pathSection.Unknown2,4},{pathSection.Unknown3,4},{pathSection.Unknown4,4},{pathSection.Width,7:F1},{pathSection.SectionType,5}", $"Section #{n}");
                    n++;
                }

                dw.WriteLine();

                dw.WriteLine($"{CopStartPoints.Count}", "Number of cop start points");

                dw.WriteLine();
                dw.WriteLine("END OF OPPONENT PATHS");
                dw.WriteLine();

                dw.WriteLine();
                dw.WriteLine("START OF DRONE PATHS");
                dw.WriteLine();

                dw.WriteLine($"{DronePathVersion}", "version");
                dw.WriteLine($"{DronePaths.Count}", "n_nodes");

                for (int i = 0; i < DronePaths.Count; i++)
                {
                    DronePathNode node = DronePaths[i];

                    dw.WriteLine();
                    dw.WriteLine($"// {i}:");
                    dw.IncreaseIndent();
                    dw.WriteLine($"{node.Position.X:F3}, {node.Position.Y:F3}, {node.Position.Z:F3}");
                    dw.WriteLine($"{node.DroneName}");
                    dw.WriteLine($"{node.UnknownInt}");
                    dw.WriteLine($"{node.Destinations.Count}");
                    dw.IncreaseIndent();
                    foreach (string destination in node.Destinations)
                    {
                        dw.WriteLine($"{destination}");
                    }
                    dw.DecreaseIndent();
                    dw.DecreaseIndent();
                }

                dw.WriteLine();
                dw.WriteLine("END OF DRONE PATHS");
                dw.WriteLine();

                dw.WriteLine($"{MaterialModifiers.Count}", "number of material modifiers");

                for (int i = 0; i < MaterialModifiers.Count; i++)
                {
                    dw.WriteLine($"// {(i == 0 ? "default material" : $"material '{i - 1}'")}");

                    MaterialModifier materialModifier = MaterialModifiers[i];
                    dw.WriteLine($"{materialModifier.CarWallFriction:0.0##}", "car wall friction");
                    dw.WriteLine($"{materialModifier.TyreRoadFriction:0.0##}", "tyre road friction");
                    dw.WriteLine($"{materialModifier.DownForce:0.0##}", "down force");
                    dw.WriteLine($"{materialModifier.Bumpiness:0.0##}", "bumpiness");
                    dw.WriteLine($"{materialModifier.TyreSoundIndex}", "tyre sound index");
                    dw.WriteLine($"{materialModifier.CrashSoundIndex}", "crash sound index");
                    dw.WriteLine($"{materialModifier.ScrapeNoiseIndex}", "scrape noise index");
                    dw.WriteLine($"{materialModifier.Sparkiness:0.0##}", "sparkiness");
                    dw.WriteLine($"{materialModifier.RoomForExpansion}", "room for expansion");
                    dw.WriteLine($"{materialModifier.SkidmarkMaterial}", "skid mark material");
                }

                dw.WriteLine();
                dw.WriteLine("// Non CarObjects");
                dw.WriteLine($"{Noncars.Count}");
                foreach (string noncar in Noncars)
                {
                    dw.WriteLine($"{noncar}");
                }

                dw.WriteLine();

                dw.WriteLine($"{ShadeTables.Count}", "number of dust shade tables");
                foreach (ShadeTable shade in ShadeTables)
                {
                    dw.WriteLine($"{shade.RGB.X}, {shade.RGB.Y}, {shade.RGB.Z}", "r g b values");
                    dw.WriteLine($"{shade.Strengths.X}, {shade.Strengths.Y}, {shade.Strengths.Z}", @"quarter, half and three quarter ""strength""");
                }

                dw.WriteLine();

                dw.WriteLine($"{NetworkStarts.Count}", "Number of network start points");
                n = 0;
                foreach (NetworkStart start in NetworkStarts)
                {
                    dw.WriteLine();
                    dw.WriteLine($"{start.Position.X:F3}, {start.Position.Y:F3}, {start.Position.Z:F3}", $"{n}");
                    dw.WriteLine($"{start.Rotation}");
                    n++;
                }

                dw.WriteLine();

                dw.WriteLine($"{SplashFiles.Count}", "number of splash files");
                foreach (string splash in SplashFiles)
                {
                    dw.WriteLine($"{splash}", "name of pixelmapfile for splashes");
                }

                dw.WriteLine();

                dw.WriteLine($"{TxtFiles.Count}");
                foreach (string txt in TxtFiles)
                {
                    dw.WriteLine($"{txt}");
                }
            }
        }
    }

    public class Checkpoint
    {
        public Vector3 TimerIncrements { get; set; }

        public List<Vector3> Points { get; set; } = new List<Vector3>();
    }

    public class PedSpec
    {
        public string MaterialName { get; set; }

        public int MovementIndex { get; set; }

        public int GroupIndex { get; set; }

        public float PedsPer100SquareMetres { get; set; }

        public List<PedExclusionMaterial> ExclusionMaterials { get; set; } = new List<PedExclusionMaterial>();

        public List<string> ExceptionMaterials { get; set; } = new List<string>();
    }

    public class PedExclusionMaterial
    {
        public int Flags { get; set; }

        public string Name { get; set; }
    }

    public class SpecialEffectVolume
    {
        public enum SpecVolSoundType
        {
            NONE,
            SCATTERED,
            SATURATED
        }

        public string Type { get; set; }

        public float GravityMultiplier { get; set; }

        public float ViscosityMultiplier { get; set; }

        public float CarDamagePerMillisecond { get; set; }

        public float PedDamagePerMillisecond { get; set; }

        public int CameraEffectIndex { get; set; }

        public int SkyColour { get; set; }

        public Vector3 SkyColourRGB { get; set; }

        public string WindscreenMaterial { get; set; }

        public int EntrySoundID { get; set; }

        public int ExitSoundID { get; set; }

        public int EngineNoiseIndex { get; set; }

        public int MaterialIndex { get; set; }

        public List<Vector3> Corners { get; set; } = new List<Vector3>();

        public SpecVolSoundType SoundType { get; set; }

        public SoundSpec SoundSpec { get; set; }
    }

    public class SoundSpec
    {
        public enum SoundSpecType
        {
            RANDOM,
            CONTINUOUS
        }

        public SoundSpecType SoundType { get; set; }

        public Vector2 TimeBetween { get; set; }

        public int MaxPercentPitchDeviation { get; set; }

        public List<int> SoundIDs { get; set; } = new List<int>();

        public static SoundSpec Load(DocumentParser file)
        {
            SoundSpec spec = new SoundSpec
            {
                SoundType = file.ReadEnum<SoundSpecType>()
            };

            switch (spec.SoundType)
            {
                case SoundSpecType.RANDOM:
                    spec.TimeBetween = file.ReadVector2();
                    break;
            }

            spec.MaxPercentPitchDeviation = file.ReadInt();
            int numSounds = file.ReadInt();

            for (int i = 0; i < numSounds; i++)
            {
                spec.SoundIDs.Add(file.ReadInt());
            }

            return spec;
        }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{SoundType}");
            if (SoundType == SoundSpecType.RANDOM) { dw.WriteLine($"{TimeBetween.X},{TimeBetween.Y}"); }
            dw.WriteLine($"{MaxPercentPitchDeviation}");
            dw.WriteLine($"{SoundIDs.Count}");

            foreach (int soundID in SoundIDs)
            {
                dw.WriteLine($"{soundID}");
            }
        }
    }

    public class OpponentPathSection
    {
        public int StartNode { get; set; }

        public int EndNode { get; set; }

        public int Unknown1 { get; set; }

        public int Unknown2 { get; set; }

        public int Unknown3 { get; set; }

        public int Unknown4 { get; set; }

        public float Width { get; set; }

        public int SectionType { get; set; }
    }

    public class DronePathNode
    {
        public Vector3 Position { get; set; }

        public string DroneName { get; set; }

        public int UnknownInt { get; set; }

        public Vector3 UnknownVector { get; set; }

        public List<string> Destinations { get; set; } = new List<string>();
    }

    public class MaterialModifier
    {
        public float CarWallFriction { get; set; }

        public float TyreRoadFriction { get; set; }

        public float DownForce { get; set; }

        public float Bumpiness { get; set; }

        public int TyreSoundIndex { get; set; }

        public int CrashSoundIndex { get; set; }

        public int ScrapeNoiseIndex { get; set; }

        public float Sparkiness { get; set; }

        public int RoomForExpansion { get; set; }

        public string SkidmarkMaterial { get; set; }
    }

    public class ShadeTable
    {
        public Vector3 RGB { get; set; }

        public Vector3 Strengths { get; set; }
    }

    public class NetworkStart
    {
        public Vector3 Position { get; set; }

        public int Rotation { get; set; }
    }
}