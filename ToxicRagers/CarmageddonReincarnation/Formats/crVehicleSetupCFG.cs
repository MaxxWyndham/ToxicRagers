using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ToxicRagers.CarmageddonReincarnation.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class VehicleSetupConfig
    {
        public List<string> Drivers { get; set; } = new List<string>();

        public string DriverSuffix { get; set; }

        public string AIScript { get; set; }

        public bool EjectDriver { get; set; } = true;

        public Vector3 InCarCamOffset { get; set; } = Vector3.Zero;

        public Vector3 GarageCameraOffset { get; set; } = Vector3.Zero;

        public bool SmallDriver { get; set; } = false;

        public List<VehicleAttachment> Attachments { get; set; } = new List<VehicleAttachment>();

        public List<VehicleWheelModule> WheelModules { get; set; } = new List<VehicleWheelModule>();

        public List<VehicleMaterialMap> MaterialMaps { get; set; } = new List<VehicleMaterialMap>();

        public List<VehicleWheelMap> WheelMaps { get; set; } = new List<VehicleWheelMap>();

        public List<Vector3> DecalPoints { get; set; } = new List<Vector3>();

        public VehicleSuspensionFactors SuspensionFactors { get; set; }

        public VehicleStats Stats { get; set; } = new VehicleStats();

        public List<string> HumanTrailer { get; set; } = new List<string>();

        public List<string> AITrailer { get; set; } = new List<string>();

        public List<string> MPTrailer { get; set; } = new List<string>();

        public static VehicleSetupConfig Load(string pathToFile)
        {
            VehicleSetupConfig setup = new VehicleSetupConfig();

            using (DocumentParser doc = new DocumentParser(pathToFile))
            {
                string line = doc.ReadFirstLine();

                while (line != null)
                {
                    switch (line)
                    {
                        case "[default_driver]":
                            while (!doc.NextLineIsASection() && !doc.EOF())
                            {
                                setup.Drivers.Add(doc.ReadNextLine());
                            }
                            break;

                        case "[driver_suffix]":
                            setup.DriverSuffix = doc.ReadNextLine();
                            break;

                        case "[attachment]":
                            setup.Attachments.Add(new VehicleAttachment(doc));
                            break;

                        case "[wheel_module]":
                            setup.WheelModules.Add(new VehicleWheelModule(doc));
                            break;

                        case "[suspension_factors]":
                            setup.SuspensionFactors = new VehicleSuspensionFactors(doc);
                            break;

                        case "[ai_script]":
                            setup.AIScript = doc.ReadNextLine();
                            break;

                        case "[material_map]":
                            setup.MaterialMaps.Add(new VehicleMaterialMap(doc));
                            break;

                        case "[wheel_map]":
                            setup.WheelMaps.Add(new VehicleWheelMap(doc));
                            break;

                        case "[disable_ejection]":
                            setup.EjectDriver = false;
                            break;

                        case "[stats]":
                            setup.Stats.TopSpeed = doc.ReadInt();
                            setup.Stats.Time = doc.ReadFloat();
                            setup.Stats.Weight = doc.ReadFloat();
                            setup.Stats.Toughness = doc.ReadFloat();
                            if (!doc.EOF() && !doc.NextLineIsASection()) { setup.Stats.UnlockLevel = doc.ReadFloat(); }
                            break;

                        case "[decal_points]":
                            while (!doc.NextLineIsASection() && !doc.EOF())
                            {
                                setup.DecalPoints.Add(doc.ReadVector3());
                            }
                            break;

                        case "[in_car_cam_offset]":
                            setup.InCarCamOffset = doc.ReadVector3();
                            break;

                        case "[garage_camera_offset]":
                            setup.GarageCameraOffset = doc.ReadVector3();
                            break;

                        case "[small_driver]":
                            setup.SmallDriver = true;
                            break;

                        case "[Human_Trailer]":
                            while (!doc.NextLineIsASection() && !doc.EOF())
                            {
                                setup.HumanTrailer.Add(doc.ReadNextLine());
                            }
                            break;

                        case "[AI_Trailer]":
                            while (!doc.NextLineIsASection() && !doc.EOF())
                            {
                                setup.AITrailer.Add(doc.ReadNextLine());
                            }
                            break;

                        case "[MP_Trailer]":
                            while (!doc.NextLineIsASection() && !doc.EOF())
                            {
                                setup.MPTrailer.Add(doc.ReadNextLine());
                            }
                            break;

                        default:
                            throw new NotImplementedException("Unexpected [SECTION]: " + line);
                    }

                    line = doc.ReadNextLine();
                }
            }

            return setup;
        }

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path + "\\vehicle_setup.cfg"))
            {
                if (Drivers.Count > 0)
                {
                    sw.WriteLine("[default_driver]");
                    foreach (string driver in Drivers)
                    {
                        sw.WriteLine($"{driver}");
                    }
                    sw.WriteLine();
                }

                if (!string.IsNullOrEmpty(DriverSuffix))
                {
                    sw.WriteLine("[driver_suffix]");
                    sw.WriteLine(DriverSuffix);
                    sw.WriteLine();
                }

                if (SmallDriver)
                {
                    sw.WriteLine("[small_driver]");
                    sw.WriteLine();
                }

                if (!EjectDriver)
                {
                    sw.WriteLine("[disable_ejection]");
                    sw.WriteLine();
                }

                if (GarageCameraOffset != Vector3.Zero)
                {
                    sw.WriteLine("[garage_camera_offset]");
                    sw.WriteLine($"{GarageCameraOffset.X},{GarageCameraOffset.Y},{GarageCameraOffset.Z}");
                    sw.WriteLine();
                }

                foreach (VehicleAttachment attachment in Attachments)
                {
                    sw.WriteLine(attachment.Write());
                }

                foreach (VehicleWheelModule wheelModule in WheelModules)
                {
                    sw.WriteLine(wheelModule.Write());
                }

                if (SuspensionFactors != null) { sw.WriteLine(SuspensionFactors.Write()); }

                if (!string.IsNullOrEmpty(AIScript))
                {
                    sw.WriteLine("[ai_script]");
                    sw.WriteLine(AIScript);
                    sw.WriteLine();
                }

                foreach (VehicleMaterialMap materialMap in MaterialMaps)
                {
                    sw.WriteLine(materialMap.Write());
                }

                sw.WriteLine(Stats.Write());

                foreach (VehicleWheelMap wheelMap in WheelMaps)
                {
                    sw.WriteLine(wheelMap.Write());
                }

                if (HumanTrailer.Count > 0)
                {
                    sw.WriteLine("[Human_Trailer]");
                    foreach (string trailer in HumanTrailer)
                    {
                        sw.WriteLine($"{trailer}");
                    }
                    sw.WriteLine();
                }

                if (AITrailer.Count > 0)
                {
                    sw.WriteLine("[AI_Trailer]");
                    foreach (string trailer in AITrailer)
                    {
                        sw.WriteLine($"{trailer}");
                    }
                    sw.WriteLine();
                }

                if (MPTrailer.Count > 0)
                {
                    sw.WriteLine("[MP_Trailer]");
                    foreach (string trailer in MPTrailer)
                    {
                        sw.WriteLine($"{trailer}");
                    }
                    sw.WriteLine();
                }

                if (InCarCamOffset != Vector3.Zero)
                {
                    sw.WriteLine("[in_car_cam_offset]");
                    sw.WriteLine($"{InCarCamOffset.X},{InCarCamOffset.Y},{InCarCamOffset.Z}");
                    sw.WriteLine();
                }

                if (DecalPoints.Count > 0)
                {
                    sw.WriteLine("[decal_points]");

                    foreach (Vector3 point in DecalPoints)
                    {
                        sw.WriteLine($"{point.X},{point.Y},{point.Z}");
                    }
                }
            }
        }
    }

    public class VehicleAttachment
    {
        public enum AttachmentType
        {
            ComplicatedWheels,
            DynamicsFmodEngine,
            DynamicsWheels,
            ExhaustParticles,
            Horn,
            ReverseLightSound,
            ContinuousSound
        }

        public AttachmentType Type { get; set; }

        public VehicleAttachmentFModEngine FModEngine { get; set; }

        public VehicleAttachmentComplicatedWheels Wheels { get; set; }

        public VehicleAttachmentExhaust Exhaust { get; set; }

        public string Horn { get; set; }

        public string ReverseLightSound { get; set; }

        public string ContinuousSound { get; set; }

        public string ContinuousSoundLump { get; set; }

        public VehicleAttachment() { }

        public VehicleAttachment(DocumentParser doc)
        {
            string s = doc.ReadNextLine();

            switch (s)
            {
                case "DynamicsWheels":
                    Type = AttachmentType.DynamicsWheels;
                    break;

                case "ComplicatedWheels":
                    Type = AttachmentType.ComplicatedWheels;
                    Wheels = new VehicleAttachmentComplicatedWheels();

                    while (!doc.NextLineIsASection())
                    {
                        string[] cw = doc.ReadStringArray(2);

                        switch (cw[0])
                        {
                            case "fl_wheel_folder_name":
                                Wheels.FLWheel = cw[1];
                                break;

                            case "fr_wheel_folder_name":
                                Wheels.FRWheel = cw[1];
                                break;

                            case "rl_wheel_folder_name":
                                Wheels.RLWheel = cw[1];
                                break;

                            case "rr_wheel_folder_name":
                                Wheels.RRWheel = cw[1];
                                break;

                            case "wheel_folder_name":
                                Wheels.FLWheel = cw[1];
                                Wheels.FRWheel = cw[1];
                                Wheels.RLWheel = cw[1];
                                Wheels.RRWheel = cw[1];
                                break;

                            default:
                                throw new NotImplementedException("Unknown ComplicatedWheels parameter: " + cw[0]);
                        }
                    }
                    break;

                case "DynamicsFmodEngine":
                    Type = AttachmentType.DynamicsFmodEngine;
                    FModEngine = new VehicleAttachmentFModEngine();

                    while (!doc.NextLineIsASection())
                    {
                        string[] dfe = doc.ReadStringArray(2);

                        switch (dfe[0])
                        {
                            case "engine":
                                FModEngine.Engine = dfe[1];
                                break;

                            case "rpmsmooth":
                                FModEngine.RPMSmooth = float.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            case "onloadsmooth":
                                FModEngine.OnLoadSmooth = float.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            case "offloadsmooth":
                                FModEngine.OffLoadSmooth = float.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            case "max_revs":
                                FModEngine.MaxRevs = int.Parse(dfe[1]);
                                break;

                            case "min_revs":
                                FModEngine.MinRevs = int.Parse(dfe[1]);
                                break;

                            case "max_speed":
                                FModEngine.MaxSpeed = int.Parse(dfe[1]);
                                break;

                            case "loadmin":
                                FModEngine.LoadMin = float.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            default:
                                throw new NotImplementedException("Unknown DynamicsFmodEngine parameter: " + dfe[0]);
                        }
                    }
                    break;

                case "Horn":
                    Type = AttachmentType.Horn;

                    string[] h = doc.ReadStringArray(2);
                    Horn = h[1];
                    break;

                case "ExhaustParticles":
                    Type = AttachmentType.ExhaustParticles;
                    Exhaust = new VehicleAttachmentExhaust();

                    while (!doc.NextLineIsASection())
                    {
                        string[] ep = doc.ReadStringArray(2);

                        switch (ep[0])
                        {
                            case "vfx":
                                Exhaust.VFX = ep[1];
                                break;

                            case "underwater_vfx":
                                Exhaust.UnderwaterVFX = ep[1];
                                break;

                            case "anchor":
                                Exhaust.Anchor = ep[1];
                                break;

                            case "multiplier":
                                Exhaust.Multiplier = float.Parse(ep[1], ToxicRagers.Culture);
                                break;

                            case "neutral_multiplier":
                                Exhaust.NeutralMultiplier = float.Parse(ep[1], ToxicRagers.Culture);
                                break;

                            default:
                                throw new NotImplementedException("Unknown ExhaustParticle parameter: " + ep[0]);
                        }
                    }
                    break;

                case "ReverseLightSound":
                    Type = AttachmentType.ReverseLightSound;

                    string[] rl = doc.ReadStringArray(2);
                    ReverseLightSound = rl[1];
                    break;

                case "ContinuousSound":
                    Type = AttachmentType.ContinuousSound;

                    while (!doc.NextLineIsASection())
                    {
                        string[] cs = doc.ReadStringArray(2);

                        switch (cs[0])
                        {
                            case "sound":
                                ContinuousSound = cs[1];
                                break;

                            case "lump":
                                ContinuousSoundLump = cs[1];
                                break;

                            default:
                                throw new NotImplementedException($"Unknown ContinuousSound parameter: {cs[0]}");
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException($"Unknown AttachmentType: {s}");
            }
        }

        public string Write()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[attachment]");
            sb.AppendLine(Type.ToString());

            switch (Type)
            {
                case AttachmentType.ComplicatedWheels:
                    sb.AppendLine(Wheels.ToString());
                    break;

                case AttachmentType.DynamicsFmodEngine:
                    sb.Append(FModEngine.ToString());
                    break;

                case AttachmentType.Horn:
                    sb.AppendLine($"event {Horn}");
                    break;

                case AttachmentType.ExhaustParticles:
                    sb.Append(Exhaust.ToString());
                    break;

                case AttachmentType.ContinuousSound:
                    sb.AppendLine($"sound {ContinuousSound}");
                    sb.AppendLine($"lump {ContinuousSoundLump}");
                    break;

                case AttachmentType.ReverseLightSound:
                    sb.AppendLine($"event {ReverseLightSound}");
                    break;
            }

            return sb.ToString();
        }
    }

    public class VehicleAttachmentFModEngine
    {
        public string Engine { get; set; }

        public float RPMSmooth { get; set; }

        public float OnLoadSmooth { get; set; }

        public float OffLoadSmooth { get; set; }

        public int MaxRevs { get; set; }

        public int MinRevs { get; set; }

        public int MaxSpeed { get; set; }

        public float LoadMin { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"engine {Engine}");
            if (RPMSmooth != default) { sb.AppendLine($"rpmsmooth {RPMSmooth}"); }
            if (OnLoadSmooth != default) { sb.AppendLine($"onloadsmooth {OnLoadSmooth}"); }
            if (OffLoadSmooth != default) { sb.AppendLine($"offloadsmooth {OffLoadSmooth}"); }
            if (LoadMin != default) { sb.AppendLine($"loadmin {LoadMin}"); }
            if (MaxSpeed != default) { sb.AppendLine($"max_speed {MaxSpeed}"); }
            if (MaxRevs != default) { sb.AppendLine($"max_revs {MaxRevs}"); }
            if (MinRevs != default) { sb.AppendLine($"min_revs {MinRevs}"); }
            return sb.ToString();
        }
    }

    public class VehicleAttachmentExhaust
    {
        public string VFX { get; set; }

        public string UnderwaterVFX { get; set; }

        public string Anchor { get; set; }

        public float Multiplier { get; set; }

        public float NeutralMultiplier { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"vfx {VFX}");
            if (!string.IsNullOrEmpty(UnderwaterVFX)) { sb.AppendLine($"underwater_vfx {UnderwaterVFX}"); }
            sb.AppendLine($"anchor {Anchor}");
            if (Multiplier != default) { sb.AppendLine($"multiplier {Multiplier}"); }
            if (NeutralMultiplier != default) { sb.AppendLine($"neutral_multiplier {NeutralMultiplier}"); }
            return sb.ToString();
        }
    }

    public class VehicleAttachmentComplicatedWheels
    {
        readonly Dictionary<string, string> wheels = new Dictionary<string, string>();

        public string FLWheel
        {
            get => wheels.ContainsKey("fl") ?  wheels["fl"] : null;
            set => wheels["fl"] = value;
        }

        public string FRWheel
        {
            get => wheels.ContainsKey("fr") ?  wheels["fr"] : null;
            set => wheels["fr"] = value;
        }

        public string RLWheel
        {
            get => wheels.ContainsKey("rl") ?  wheels["rl"] : null;
            set => wheels["rl"] = value;
        }

        public string RRWheel
        {
            get => wheels.ContainsKey("rr") ?  wheels["rr"] : null;
            set => wheels["rr"] = value;
        }

        public string D4
        {
            get => wheels.ContainsKey("D4") ?  wheels["D4"] : null;
            set => wheels["D4"] = value;
        }

        public string D5
        {
            get => wheels.ContainsKey("D5") ?  wheels["D5"] : null;
            set => wheels["D5"] = value;
        }

        public string D6
        {
            get => wheels.ContainsKey("D6") ?  wheels["D6"] : null;
            set => wheels["D6"] = value;
        }

        public string D7
        {
            get => wheels.ContainsKey("D7") ?  wheels["D7"] : null;
            set => wheels["D7"] = value;
        }

        public string D8
        {
            get => wheels.ContainsKey("D8") ?  wheels["D8"] : null;
            set => wheels["D8"] = value;
        }

        public string D9
        {
            get => wheels.ContainsKey("D9") ?  wheels["D9"] : null;
            set => wheels["D9"] = value;
        }

        public string D10
        {
            get => wheels.ContainsKey("D10") ?  wheels["D10"] : null;
            set => wheels["D10"] = value;
        }

        public string D11
        {
            get => wheels.ContainsKey("D11") ?  wheels["D11"] : null;
            set => wheels["D11"] = value;
        }

        public string GarageSet { get; set; }

        public override string ToString()
        {
            if (wheels.Count == 4 && FLWheel == FRWheel && RLWheel == RRWheel && FLWheel == RLWheel)
            {
                return $"wheel_folder_name {FLWheel}";
            }
            else
            {
                StringBuilder output = new StringBuilder();
                foreach (KeyValuePair<string, string> kvp in wheels)
                {
                    if (!string.IsNullOrEmpty(kvp.Value)) { output.AppendLine($"{kvp.Key}_wheel_folder_name {kvp.Value}"); }
                }
                return output.ToString();
            }
        }
    }

    public class VehicleWheelModule
    {
        public enum WheelModuleType
        {
            SkidMarks,
            SkidNoise,
            TyreParticles
        }

        public WheelModuleType Type { get; set; }

        public string SkidMarkImage { get; set; }

        public string SkidNoiseSound { get; set; }

        public string TyreParticleVFX { get; set; }

        public bool OnlyTrails { get; set; }

        public bool UseScrapeSounds { get; set; }

        public int ScrapeSoundIndex { get; set; }

        public int Volume { get; set; }

        public VehicleWheelModule() { }

        public VehicleWheelModule(DocumentParser doc)
        {
            string s = doc.ReadNextLine();

            switch (s)
            {
                case "SkidMarks":
                    Type = WheelModuleType.SkidMarks;
                    SkidMarkImage = doc.ReadStringArray(2)[1];
                    if (doc.ReadNextLine() == "only_trails") { OnlyTrails = true; } else { doc.Rewind(); }
                    break;

                case "TyreParticles":
                case "TyreSmokeVFX":
                    Type = WheelModuleType.TyreParticles;
                    TyreParticleVFX = doc.ReadStringArray(2)[1];
                    break;

                case "SkidNoise":
                    Type = WheelModuleType.SkidNoise;

                    if (doc.ReadNextLine() == "use_scrape_sounds")
                    {
                        UseScrapeSounds = true;
                        ScrapeSoundIndex = int.Parse(doc.ReadStringArray(2)[1]);
                        Volume = int.Parse(doc.ReadStringArray(2)[1]);
                    }
                    else
                    {
                        doc.Rewind();
                        SkidNoiseSound = doc.ReadStringArray(2)[1];
                    }

                    break;

                default:
                    throw new NotImplementedException($"Unknown WheelModuleType: {s}");
            }
        }

        public string Write()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[wheel_module]");
            sb.AppendLine(Type.ToString());

            switch (Type)
            {
                case WheelModuleType.TyreParticles:
                    sb.AppendLine($"vfx {TyreParticleVFX}");
                    break;

                case WheelModuleType.SkidNoise:
                    if (UseScrapeSounds)
                    {
                        sb.AppendLine("use_scrape_sounds");
                        sb.AppendLine($"scrape_index {ScrapeSoundIndex}");
                        sb.AppendLine($"volume {Volume}");
                    }
                    else
                    {
                        sb.AppendLine($"sounds {SkidNoiseSound}");
                    }
                    break;

                case WheelModuleType.SkidMarks:
                    sb.AppendLine($"image {SkidMarkImage}");
                    if (OnlyTrails) { sb.AppendLine("only_trails"); }
                    break;
            }

            return sb.ToString();
        }
    }

    public class VehicleMaterialMap
    {
        public string Name { get; set; }

        public Vector3 Shrapnel { get; set; } = Vector3.Zero;

        public string Localisation { get; set; }

        public Dictionary<string, string> Substitutions { get; set; } = new Dictionary<string, string>();

        public int MaterialMapProductID { get; set; }

        public VehicleMaterialMap(DocumentParser doc)
        {
            Name = doc.ReadNextLine();

            while (!doc.NextLineIsASection())
            {
                string[] mm = doc.ReadStringArray();

                switch (mm[0].ToLower())
                {
                    case "shrapnel":
                        Shrapnel = Vector3.Parse(mm[1]);
                        break;

                    case "localise":
                        Localisation = mm[1];
                        break;

                    case "material_map_product_id":
                        MaterialMapProductID = mm[1].ToInt();
                        break;

                    default:
                        if (mm[1] == ":")
                        {
                            Substitutions[mm[0]] = mm[2];
                        }
                        else
                        {
                            throw new NotImplementedException($"Unknown MaterialMap parameter: {mm[0]}");
                        }
                        break;
                }
            }
        }

        public string Write()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[material_map]");
            sb.AppendLine(Name);
            sb.AppendLine($"localise {Localisation}");
            foreach (KeyValuePair<string, string> kvp in Substitutions)
            {
                sb.AppendLine($"{kvp.Key} : {kvp.Value}");
            }
            sb.AppendLine($"shrapnel {Shrapnel.X},{Shrapnel.Y},{Shrapnel.Z}");
            if (MaterialMapProductID > 0) { sb.AppendLine($"material_map_product_id {MaterialMapProductID}"); }
            return sb.ToString();
        }
    }

    public class VehicleWheelMap
    {
        public string Name { get; set; }

        public string Localisation { get; set; }

        public VehicleAttachmentComplicatedWheels Wheels { get; set; }

        public VehicleWheelMap() { }

        public VehicleWheelMap(DocumentParser doc)
        {
            Name = doc.ReadNextLine();
            Wheels = new VehicleAttachmentComplicatedWheels();

            while (!doc.NextLineIsASection() && !doc.EOF())
            {
                string[] wm = doc.ReadStringArray(2);

                switch (wm[0])
                {
                    case "localise":
                        Localisation = wm[1];
                        break;

                    case "fl_wheel_folder_name":
                        Wheels.FLWheel = wm[1];
                        break;

                    case "fr_wheel_folder_name":
                        Wheels.FRWheel = wm[1];
                        break;

                    case "rl_wheel_folder_name":
                        Wheels.RLWheel = wm[1];
                        break;

                    case "rr_wheel_folder_name":
                        Wheels.RRWheel = wm[1];
                        break;

                    case "wheel_folder_name":
                        Wheels.FLWheel = wm[1];
                        Wheels.FRWheel = wm[1];
                        Wheels.RLWheel = wm[1];
                        Wheels.RRWheel = wm[1];
                        break;

                    case "D4_wheel_folder_name":
                        Wheels.D4 = wm[1];
                        break;

                    case "D5_wheel_folder_name":
                        Wheels.D5 = wm[1];
                        break;

                    case "D6_wheel_folder_name":
                        Wheels.D6 = wm[1];
                        break;

                    case "D7_wheel_folder_name":
                        Wheels.D7 = wm[1];
                        break;

                    case "D8_wheel_folder_name":
                        Wheels.D8 = wm[1];
                        break;

                    case "D9_wheel_folder_name":
                        Wheels.D9 = wm[1];
                        break;

                    case "D10_wheel_folder_name":
                        Wheels.D10 = wm[1];
                        break;

                    case "D11_wheel_folder_name":
                        Wheels.D11 = wm[1];
                        break;

                    case "garage_set":
                        Wheels.GarageSet = wm[1];
                        break;

                    default:
                        throw new NotImplementedException($"Unknown WheelMap parameter: {wm[0]}");
                }
            }
        }

        public string Write()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[wheel_map]");
            sb.AppendLine(Name);
            sb.AppendLine($"localise {Localisation}");
            sb.Append(Wheels.ToString());
            if (!string.IsNullOrEmpty(Wheels.GarageSet)) { sb.AppendLine($"garage_set {Wheels.GarageSet}"); }
            return sb.ToString();
        }
    }

    public class VehicleSuspensionFactors
    {
        public float MaxCompression { get; set; }

        public float RightHeight { get; set; }

        public int MaxSteeringLock { get; set; }

        public float MaxExtension { get; set; }

        public VehicleSuspensionFactors(DocumentParser doc)
        {
            while (!doc.NextLineIsASection())
            {
                string[] sf = doc.ReadStringArray(2);

                switch (sf[0])
                {
                    case "max_compression":
                        MaxCompression = sf[1].ToSingle();
                        break;

                    case "ride_height":
                        RightHeight = sf[1].ToSingle();
                        break;

                    case "max_steering_lock":
                        MaxSteeringLock = sf[1].ToInt();
                        break;

                    case "max_extension":
                        MaxExtension = sf[1].ToSingle();
                        break;

                    default:
                        throw new NotImplementedException($"Unknown SuspensionFactor parameter: {sf[0]}");
                }
            }
        }

        public string Write()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("[suspension_factors]");
            if (RightHeight != default) { sb.AppendLine($"ride_height {RightHeight}"); }
            if (MaxCompression != default) { sb.AppendLine($"max_compression {MaxCompression}"); }
            if (MaxExtension != default) { sb.AppendLine($"max_extension {MaxExtension}"); }
            if (MaxSteeringLock != default) { sb.AppendLine($"max_steering_lock {MaxSteeringLock}"); }

            return sb.ToString();
        }
    }

    public class VehicleStats
    {
        public int TopSpeed { get; set; }

        public float Time { get; set; }

        public float Weight { get; set; }

        public float Toughness { get; set; }

        public float UnlockLevel { get; set; } = -1;

        public string Write()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("[stats]");
            sb.AppendLine($"{TopSpeed}   // top speed; they must be in this order and not have spaces before the comments");
            sb.AppendLine($"{Time}   // time 0 -60");
            sb.AppendLine($"{Weight}   // weight");
            sb.AppendLine($"{Toughness}   // toughness");
            if (UnlockLevel != -1) { sb.AppendLine($"{UnlockLevel}   // unlock level"); }

            return sb.ToString();
        }
    }
}