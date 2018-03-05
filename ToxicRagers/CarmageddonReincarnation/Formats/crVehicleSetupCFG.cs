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
        List<string> drivers;
        string aiScript;
        bool bEjectDriver = true;
        Vector3 incarCamOffset = Vector3.Zero;
        Vector3 garageCamOffset = Vector3.Zero;
        List<VehicleAttachment> attachments;
        List<VehicleWheelModule> wheelModules;
        List<VehicleMaterialMap> materialMaps;
        List<VehicleWheelMap> wheelMaps;
        List<Vector3> decalPoints;
        VehicleSuspensionFactors suspensionFactors;
        VehicleStats stats;
        bool bSmallDriver = false;
        string driverSuffix;
        List<string> humanTrailers;
        List<string> aiTrailers;
        List<string> mpTrailers;

        public List<string> Drivers
        {
            get => drivers;
            set => drivers = value;
        }

        public string DriverSuffix
        {
            get => driverSuffix;
            set => driverSuffix = value;
        }

        public string AIScript
        {
            get => aiScript;
            set => aiScript = value;
        }

        public bool EjectDriver
        {
            get => bEjectDriver;
            set => bEjectDriver = value;
        }

        public Vector3 InCarCamOffset
        {
            get => incarCamOffset;
            set => incarCamOffset = value;
        }

        public Vector3 GarageCameraOffset
        {
            get => garageCamOffset;
            set => garageCamOffset = value;
        }

        public bool SmallDriver
        {
            get => bSmallDriver;
            set => bSmallDriver = value;
        }

        public List<VehicleAttachment> Attachments
        {
            get => attachments;
            set => attachments = value;
        }

        public List<VehicleWheelModule> WheelModules
        {
            get => wheelModules;
            set => wheelModules = value;
        }

        public List<VehicleMaterialMap> MaterialMaps
        {
            get => materialMaps;
            set => materialMaps = value;
        }

        public List<VehicleWheelMap> WheelMaps
        {
            get => wheelMaps;
            set => wheelMaps = value;
        }

        public List<Vector3> DecalPoints
        {
            get => decalPoints;
            set => decalPoints = value;
        }

        public VehicleSuspensionFactors SuspensionFactors
        {
            get => suspensionFactors;
            set => suspensionFactors = value;
        }

        public VehicleStats Stats
        {
            get => stats;
            set => stats = value;
        }

        public List<string> HumanTrailer
        {
            get => humanTrailers;
            set => humanTrailers = value;
        }

        public List<string> AITrailer
        {
            get => aiTrailers;
            set => aiTrailers = value;
        }

        public List<string> MPTrailer
        {
            get => mpTrailers;
            set => mpTrailers = value;
        }

        public VehicleSetupConfig()
        {
            drivers = new List<string>();
            attachments = new List<VehicleAttachment>();
            wheelModules = new List<VehicleWheelModule>();
            materialMaps = new List<VehicleMaterialMap>();
            wheelMaps = new List<VehicleWheelMap>();
            decalPoints = new List<Vector3>();
            stats = new VehicleStats();
            humanTrailers = new List<string>();
            aiTrailers = new List<string>();
            mpTrailers = new List<string>();
        }

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
                            Console.WriteLine(pathToFile);
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
                if (drivers.Count > 0)
                {
                    sw.WriteLine("[default_driver]");
                    foreach (string driver in drivers)
                    {
                        sw.WriteLine($"{driver}");
                    }
                    sw.WriteLine();
                }

                if (driverSuffix != null)
                {
                    sw.WriteLine("[driver_suffix]");
                    sw.WriteLine(driverSuffix);
                    sw.WriteLine();
                }

                if (!bSmallDriver)
                {
                    sw.WriteLine("[small_driver]");
                    sw.WriteLine();
                }

                if (!bEjectDriver)
                {
                    sw.WriteLine("[disable_ejection]");
                    sw.WriteLine();
                }

                if (garageCamOffset != Vector3.Zero)
                {
                    sw.WriteLine("[garage_camera_offset]");
                    sw.WriteLine($"{garageCamOffset.X},{garageCamOffset.Y},{garageCamOffset.Z}");
                    sw.WriteLine();
                }

                foreach (VehicleAttachment attachment in attachments)
                {
                    sw.WriteLine(attachment.Write());
                }

                foreach (VehicleWheelModule wheelModule in wheelModules)
                {
                    sw.WriteLine(wheelModule.Write());
                }

                if (suspensionFactors != null) { sw.WriteLine(suspensionFactors.Write()); }

                if (aiScript != null)
                {
                    sw.WriteLine("[ai_script]");
                    sw.WriteLine(aiScript);
                    sw.WriteLine();
                }

                foreach (VehicleMaterialMap materialMap in materialMaps)
                {
                    sw.WriteLine(materialMap.Write());
                }

                sw.WriteLine(stats.Write());

                foreach (VehicleWheelMap wheelMap in wheelMaps)
                {
                    sw.WriteLine(wheelMap.Write());
                }

                if (humanTrailers.Count > 0)
                {
                    sw.WriteLine("[Human_Trailer]");
                    foreach (string trailer in humanTrailers)
                    {
                        sw.WriteLine($"{trailer}");
                    }
                    sw.WriteLine();
                }

                if (aiTrailers.Count > 0)
                {
                    sw.WriteLine("[AI_Trailer]");
                    foreach (string trailer in aiTrailers)
                    {
                        sw.WriteLine($"{trailer}");
                    }
                    sw.WriteLine();
                }

                if (mpTrailers.Count > 0)
                {
                    sw.WriteLine("[MP_Trailer]");
                    foreach (string trailer in mpTrailers)
                    {
                        sw.WriteLine($"{trailer}");
                    }
                    sw.WriteLine();
                }

                if (incarCamOffset != Vector3.Zero)
                {
                    sw.WriteLine("[in_car_cam_offset]");
                    sw.WriteLine($"{incarCamOffset.X},{incarCamOffset.Y},{incarCamOffset.Z}");
                    sw.WriteLine();
                }

                if (decalPoints.Count > 0)
                {
                    sw.WriteLine("[decal_points]");

                    foreach (Vector3 point in decalPoints)
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

        AttachmentType attachmentType;
        VehicleAttachmentComplicatedWheels wheels;
        VehicleAttachmentFModEngine engine;
        VehicleAttachmentExhaust exhaust;
        string horn;
        string reverseLightSound;
        string continuousSound;
        string continuousSoundLump;

        public AttachmentType Type
        {
            get => attachmentType;
            set => attachmentType = value;
        }

        public VehicleAttachmentFModEngine FModEngine
        {
            get => engine;
            set => engine = value;
        }

        public VehicleAttachmentComplicatedWheels Wheels
        {
            get => wheels;
            set => wheels = value;
        }

        public VehicleAttachmentExhaust Exhaust
        {
            get => exhaust;
            set => exhaust = value;
        }

        public string Horn
        {
            get => horn;
            set => horn = value;
        }

        public string ReverseLightSound
        {
            get => reverseLightSound;
            set => reverseLightSound = value;
        }

        public VehicleAttachment()
        {
        }

        public VehicleAttachment(DocumentParser doc)
        {
            string s = doc.ReadNextLine();

            switch (s)
            {
                case "DynamicsWheels":
                    attachmentType = AttachmentType.DynamicsWheels;
                    break;

                case "ComplicatedWheels":
                    attachmentType = AttachmentType.ComplicatedWheels;
                    wheels = new VehicleAttachmentComplicatedWheels();

                    while (!doc.NextLineIsASection())
                    {
                        string[] cw = doc.ReadStringArray(2);

                        switch (cw[0])
                        {
                            case "fl_wheel_folder_name":
                                wheels.FLWheel = cw[1];
                                break;

                            case "fr_wheel_folder_name":
                                wheels.FRWheel = cw[1];
                                break;

                            case "rl_wheel_folder_name":
                                wheels.RLWheel = cw[1];
                                break;

                            case "rr_wheel_folder_name":
                                wheels.RRWheel = cw[1];
                                break;

                            case "wheel_folder_name":
                                wheels.FLWheel = cw[1];
                                wheels.FRWheel = cw[1];
                                wheels.RLWheel = cw[1];
                                wheels.RRWheel = cw[1];
                                break;

                            default:
                                throw new NotImplementedException("Unknown ComplicatedWheels parameter: " + cw[0]);
                        }
                    }
                    break;

                case "DynamicsFmodEngine":
                    attachmentType = AttachmentType.DynamicsFmodEngine;
                    engine = new VehicleAttachmentFModEngine();

                    while (!doc.NextLineIsASection())
                    {
                        string[] dfe = doc.ReadStringArray(2);

                        switch (dfe[0])
                        {
                            case "engine":
                                engine.Engine = dfe[1];
                                break;

                            case "rpmsmooth":
                                engine.RPMSmooth = float.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            case "onloadsmooth":
                                engine.OnLoadSmooth = float.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            case "offloadsmooth":
                                engine.OffLoadSmooth = float.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            case "max_revs":
                                engine.MaxRevs = int.Parse(dfe[1]);
                                break;

                            case "min_revs":
                                engine.MinRevs = int.Parse(dfe[1]);
                                break;

                            case "max_speed":
                                engine.MaxSpeed = int.Parse(dfe[1]);
                                break;

                            case "loadmin":
                                engine.LoadMin = float.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            default:
                                throw new NotImplementedException("Unknown DynamicsFmodEngine parameter: " + dfe[0]);
                        }
                    }
                    break;

                case "Horn":
                    attachmentType = AttachmentType.Horn;

                    string[] h = doc.ReadStringArray(2);
                    horn = h[1];
                    break;

                case "ExhaustParticles":
                    attachmentType = AttachmentType.ExhaustParticles;
                    exhaust = new VehicleAttachmentExhaust();

                    while (!doc.NextLineIsASection())
                    {
                        string[] ep = doc.ReadStringArray(2);

                        switch (ep[0])
                        {
                            case "vfx":
                                exhaust.VFX = ep[1];
                                break;

                            case "underwater_vfx":
                                exhaust.UnderwaterVFX = ep[1];
                                break;

                            case "anchor":
                                exhaust.Anchor = ep[1];
                                break;

                            case "multiplier":
                                exhaust.Multiplier = float.Parse(ep[1], ToxicRagers.Culture);
                                break;

                            case "neutral_multiplier":
                                exhaust.NeutralMultiplier = float.Parse(ep[1], ToxicRagers.Culture);
                                break;

                            default:
                                throw new NotImplementedException("Unknown ExhaustParticle parameter: " + ep[0]);
                        }
                    }
                    break;

                case "ReverseLightSound":
                    attachmentType = AttachmentType.ReverseLightSound;

                    string[] rl = doc.ReadStringArray(2);
                    reverseLightSound = rl[1];
                    break;

                case "ContinuousSound":
                    attachmentType = AttachmentType.ContinuousSound;

                    while (!doc.NextLineIsASection())
                    {
                        string[] cs = doc.ReadStringArray(2);

                        switch (cs[0])
                        {
                            case "sound":
                                continuousSound = cs[1];
                                break;

                            case "lump":
                                continuousSoundLump = cs[1];
                                break;

                            default:
                                throw new NotImplementedException("Unknown ContinuousSound parameter: " + cs[0]);
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException("Unknown AttachmentType: " + s);
            }
        }

        public string Write()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[attachment]");
            sb.AppendLine(attachmentType.ToString());

            switch (attachmentType)
            {
                case AttachmentType.ComplicatedWheels:
                    sb.AppendLine(wheels.ToString());
                    break;

                case AttachmentType.DynamicsFmodEngine:
                    sb.Append(engine.ToString());
                    break;

                case AttachmentType.Horn:
                    sb.AppendLine($"event {horn}");
                    break;

                case AttachmentType.ExhaustParticles:
                    sb.Append(exhaust.ToString());
                    break;

                case AttachmentType.ContinuousSound:
                    sb.AppendLine($"sound {continuousSound}");
                    sb.AppendLine($"lump {continuousSoundLump}");
                    break;

                case AttachmentType.ReverseLightSound:
                    sb.AppendLine($"event {reverseLightSound}");
                    break;
            }

            return sb.ToString();
        }
    }

    public class VehicleAttachmentFModEngine
    {
        string engine;
        float rpmSmooth;
        float onLoadSmooth;
        float offLoadSmooth;
        int maxRevs;
        int minRevs;
        int maxSpeed;
        float loadMin;

        public string Engine
        {
            get => engine;
            set => engine = value;
        }

        public float RPMSmooth
        {
            get => rpmSmooth;
            set => rpmSmooth = value;
        }

        public float OnLoadSmooth
        {
            get => onLoadSmooth;
            set => onLoadSmooth = value;
        }

        public float OffLoadSmooth
        {
            get => offLoadSmooth;
            set => offLoadSmooth = value;
        }

        public int MaxRevs
        {
            get => maxRevs;
            set => maxRevs = value;
        }

        public int MinRevs
        {
            get => minRevs;
            set => minRevs = value;
        }

        public int MaxSpeed
        {
            get => maxSpeed;
            set => maxSpeed = value;
        }

        public float LoadMin
        {
            get => loadMin;
            set => loadMin = value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"engine {engine}");
            if (rpmSmooth != default(float)) { sb.AppendLine($"rpmsmooth {rpmSmooth}"); }
            if (onLoadSmooth != default(float)) { sb.AppendLine($"onloadsmooth {onLoadSmooth}"); }
            if (offLoadSmooth != default(float)) { sb.AppendLine($"offloadsmooth {offLoadSmooth}"); }
            if (loadMin != default(float)) { sb.AppendLine($"loadmin {loadMin}"); }
            if (maxSpeed != default(int)) { sb.AppendLine($"max_speed {maxSpeed}"); }
            if (maxRevs != default(int)) { sb.AppendLine($"max_revs {maxRevs}"); }
            if (minRevs != default(int)) { sb.AppendLine($"min_revs {minRevs}"); }
            return sb.ToString();
        }
    }

    public class VehicleAttachmentExhaust
    {
        string vfx;
        string underwaterVFX;
        string anchor;
        float multiplier;
        float neutralMultiplier;

        public string VFX
        {
            get => vfx;
            set => vfx = value;
        }

        public string UnderwaterVFX
        {
            get => underwaterVFX;
            set => underwaterVFX = value;
        }

        public string Anchor
        {
            get => anchor;
            set => anchor = value;
        }

        public float Multiplier
        {
            get => multiplier;
            set => multiplier = value;
        }

        public float NeutralMultiplier
        {
            get => neutralMultiplier;
            set => neutralMultiplier = value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"vfx {vfx}");
            if (underwaterVFX != null) { sb.AppendLine($"underwater_vfx {underwaterVFX}"); }
            sb.AppendLine($"anchor {anchor}");
            if (multiplier != default(float)) { sb.AppendLine($"multiplier {multiplier}"); }
            if (neutralMultiplier != default(float)) { sb.AppendLine($"neutral_multiplier {neutralMultiplier}"); }
            return sb.ToString();
        }
    }

    public class VehicleAttachmentComplicatedWheels
    {
        Dictionary<string, string> wheels = new Dictionary<string, string>();
        string garageSet;

        public string FLWheel
        {
            get => wheels["fl"];
            set => wheels["fl"] = value;
        }

        public string FRWheel
        {
            get => wheels["fr"];
            set => wheels["fr"] = value;
        }

        public string RLWheel
        {
            get => wheels["rl"];
            set => wheels["rl"] = value;
        }

        public string RRWheel
        {
            get => wheels["rr"];
            set => wheels["rr"] = value;
        }

        public string D4
        {
            get => wheels["D4"];
            set => wheels["D4"] = value;
        }

        public string D5
        {
            get => wheels["D5"];
            set => wheels["D5"] = value;
        }

        public string D6
        {
            get => wheels["D6"];
            set => wheels["D6"] = value;
        }

        public string D7
        {
            get => wheels["D7"];
            set => wheels["D7"] = value;
        }

        public string D8
        {
            get => wheels["D8"];
            set => wheels["D8"] = value;
        }

        public string D9
        {
            get => wheels["D9"];
            set => wheels["D9"] = value;
        }

        public string D10
        {
            get => wheels["D10"];
            set => wheels["D10"] = value;
        }

        public string D11
        {
            get => wheels["D11"];
            set => wheels["D11"] = value;
        }

        public string GarageSet
        {
            get => garageSet;
            set => garageSet = value;
        }

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
                    output.AppendLine($"{kvp.Key}_wheel_folder_name {kvp.Value}");
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

        WheelModuleType wheelModuleType;
        string skidMarkImage;
        string skidNoiseSound;
        string tyreParticleVFX;
        bool bOnlyTrails;
        bool bUseScrapeSounds;
        int scrapeSoundIndex;
        int volume;

        public WheelModuleType Type
        {
            get => wheelModuleType;
            set => wheelModuleType = value;
        }

        public string SkidMarkImage
        {
            get => skidMarkImage;
            set => skidMarkImage = value;
        }

        public string SkidNoiseSound
        {
            get => skidNoiseSound;
            set => skidNoiseSound = value;
        }

        public string TyreParticleVFX
        {
            get => tyreParticleVFX;
            set => tyreParticleVFX = value;
        }

        public bool OnlyTrails
        {
            get => bOnlyTrails;
            set => bOnlyTrails = value;
        }

        public bool UseScrapeSounds
        {
            get => bUseScrapeSounds;
            set => bUseScrapeSounds = value;
        }

        public int ScrapeSoundIndex
        {
            get => scrapeSoundIndex;
            set => scrapeSoundIndex = value;
        }

        public int Volume
        {
            get => volume;
            set => volume = value;
        }

        public VehicleWheelModule()
        {
        }

        public VehicleWheelModule(DocumentParser doc)
        {
            string s = doc.ReadNextLine();

            switch (s)
            {
                case "SkidMarks":
                    wheelModuleType = WheelModuleType.SkidMarks;
                    skidMarkImage = doc.ReadStringArray(2)[1];
                    if (doc.ReadNextLine() == "only_trails") { bOnlyTrails = true; } else { doc.Rewind(); }
                    break;

                case "TyreParticles":
                case "TyreSmokeVFX":
                    wheelModuleType = WheelModuleType.TyreParticles;
                    tyreParticleVFX = doc.ReadStringArray(2)[1];
                    break;

                case "SkidNoise":
                    wheelModuleType = WheelModuleType.SkidNoise;
                    if (doc.ReadNextLine() == "use_scrape_sounds")
                    {
                        bUseScrapeSounds = true;
                        scrapeSoundIndex = int.Parse(doc.ReadStringArray(2)[1]);
                        volume = int.Parse(doc.ReadStringArray(2)[1]);
                    }
                    else
                    {
                        doc.Rewind();
                        skidNoiseSound = doc.ReadStringArray(2)[1];
                    }

                    break;

                default:
                    throw new NotImplementedException("Unknown WheelModuleType: " + s);
            }
        }

        public string Write()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[wheel_module]");
            sb.AppendLine(wheelModuleType.ToString());

            switch (wheelModuleType)
            {
                case WheelModuleType.TyreParticles:
                    sb.AppendLine($"vfx {tyreParticleVFX}");
                    break;

                case WheelModuleType.SkidNoise:
                    if (bUseScrapeSounds)
                    {
                        sb.AppendLine("use_scrape_sounds");
                        sb.AppendLine($"scrape_index {scrapeSoundIndex}");
                        sb.AppendLine($"volume {volume}");
                    }
                    else
                    {
                        sb.AppendLine($"sounds {skidNoiseSound}");
                    }
                    break;

                case WheelModuleType.SkidMarks:
                    sb.AppendLine($"image {skidMarkImage}");
                    if (bOnlyTrails) { sb.AppendLine("only_trails"); }
                    break;
            }

            return sb.ToString();
        }
    }

    public class VehicleMaterialMap
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string>();
        string name;
        Vector3 shrapnel;
        string localName;
        int appID;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public Vector3 Shrapnel
        {
            get => shrapnel;
            set => shrapnel = value;
        }

        public string Localisation
        {
            get => localName;
            set => localName = value;
        }

        public Dictionary<string, string> Substitutions
        {
            get => substitutions;
            set => substitutions = value;
        }

        public int MaterialMapProductID
        {
            get => appID;
            set => appID = value;
        }

        public VehicleMaterialMap(DocumentParser doc)
        {
            name = doc.ReadNextLine();

            while (!doc.NextLineIsASection())
            {
                string[] mm = doc.ReadStringArray();

                switch (mm[0].ToLower())
                {
                    case "shrapnel":
                        shrapnel = Vector3.Parse(mm[1]);
                        break;

                    case "localise":
                        localName = mm[1];
                        break;

                    case "material_map_product_id":
                        appID = mm[1].ToInt();
                        break;

                    default:
                        if (mm[1] == ":")
                        {
                            substitutions[mm[0]] = mm[2];
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
            sb.AppendLine(name);
            sb.AppendLine($"localise {localName}");
            foreach (KeyValuePair<string, string> kvp in substitutions)
            {
                sb.AppendLine($"{kvp.Key} : {kvp.Value}");
            }
            sb.AppendLine($"shrapnel {shrapnel.X},{shrapnel.Y},{shrapnel.Z}");
            if (appID > 0) { sb.AppendLine($"material_map_product_id {appID}"); }
            return sb.ToString();
        }
    }

    public class VehicleWheelMap
    {
        string name;
        string localName;
        VehicleAttachmentComplicatedWheels wheels;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Localisation
        {
            get => localName;
            set => localName = value;
        }

        public VehicleAttachmentComplicatedWheels Wheels
        {
            get => wheels;
            set => wheels = value;
        }

        public VehicleWheelMap()
        {
        }

        public VehicleWheelMap(DocumentParser doc)
        {
            name = doc.ReadNextLine();
            wheels = new VehicleAttachmentComplicatedWheels();

            while (!doc.NextLineIsASection() && !doc.EOF())
            {
                string[] wm = doc.ReadStringArray(2);

                switch (wm[0])
                {
                    case "localise":
                        localName = wm[1];
                        break;

                    case "fl_wheel_folder_name":
                        wheels.FLWheel = wm[1];
                        break;

                    case "fr_wheel_folder_name":
                        wheels.FRWheel = wm[1];
                        break;

                    case "rl_wheel_folder_name":
                        wheels.RLWheel = wm[1];
                        break;

                    case "rr_wheel_folder_name":
                        wheels.RRWheel = wm[1];
                        break;

                    case "wheel_folder_name":
                        wheels.FLWheel = wm[1];
                        wheels.FRWheel = wm[1];
                        wheels.RLWheel = wm[1];
                        wheels.RRWheel = wm[1];
                        break;

                    case "D4_wheel_folder_name":
                        wheels.D4 = wm[1];
                        break;

                    case "D5_wheel_folder_name":
                        wheels.D5 = wm[1];
                        break;

                    case "D6_wheel_folder_name":
                        wheels.D6 = wm[1];
                        break;

                    case "D7_wheel_folder_name":
                        wheels.D7 = wm[1];
                        break;

                    case "D8_wheel_folder_name":
                        wheels.D8 = wm[1];
                        break;

                    case "D9_wheel_folder_name":
                        wheels.D9 = wm[1];
                        break;

                    case "D10_wheel_folder_name":
                        wheels.D10 = wm[1];
                        break;

                    case "D11_wheel_folder_name":
                        wheels.D11 = wm[1];
                        break;

                    case "garage_set":
                        wheels.GarageSet = wm[1];
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
            sb.AppendLine(name);
            sb.AppendLine($"localise {localName}");
            sb.Append(wheels.ToString());
            if (wheels.GarageSet != null) { sb.AppendLine($"garage_set {wheels.GarageSet}"); }
            return sb.ToString();
        }
    }

    public class VehicleSuspensionFactors
    {
        float maxCompression;
        float rideHeight;
        int maxSteeringLock;
        float maxExtension;

        public float MaxCompression
        {
            get => maxCompression;
            set => maxCompression = value;
        }

        public float RightHeight
        {
            get => rideHeight;
            set => rideHeight = value;
        }

        public int MaxSteeringLock
        {
            get => maxSteeringLock;
            set => maxSteeringLock = value;
        }

        public float MaxExtension
        {
            get => maxExtension;
            set => maxExtension = value;
        }

        public VehicleSuspensionFactors(DocumentParser doc)
        {
            while (!doc.NextLineIsASection())
            {
                string[] sf = doc.ReadStringArray(2);

                switch (sf[0])
                {
                    case "max_compression":
                        maxCompression = sf[1].ToSingle();
                        break;

                    case "ride_height":
                        rideHeight = sf[1].ToSingle();
                        break;

                    case "max_steering_lock":
                        maxSteeringLock = sf[1].ToInt();
                        break;

                    case "max_extension":
                        maxExtension = sf[1].ToSingle();
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
            if (rideHeight != default(float)) { sb.AppendLine($"ride_height {rideHeight}"); }
            if (maxCompression != default(float)) { sb.AppendLine($"max_compression {maxCompression}"); }
            if (maxExtension != default(float)) { sb.AppendLine($"max_extension {maxExtension}"); }
            if (maxSteeringLock != default(int)) { sb.AppendLine($"max_steering_lock {maxSteeringLock}"); }
            return sb.ToString();
        }
    }

    public class VehicleStats
    {
        int topSpeed;
        float time;
        float weight;
        float toughness;
        float unlockLevel = -1;

        public int TopSpeed
        {
            get => topSpeed;
            set => topSpeed = value;
        }

        public float Time
        {
            get => time;
            set => time = value;
        }

        public float Weight
        {
            get => weight;
            set => weight = value;
        }

        public float Toughness
        {
            get => toughness;
            set => toughness = value;
        }

        public float UnlockLevel
        {
            get => unlockLevel;
            set => unlockLevel = value;
        }

        public string Write()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[stats]");
            sb.AppendLine($"{topSpeed}// top speed; they must be in this order and not have spaces before the comments");
            sb.AppendLine($"{time}// time 0 -60");
            sb.AppendLine($"{weight}// weight");
            sb.AppendLine($"{toughness}// toughness");
            if (unlockLevel != -1) { sb.AppendLine($"{unlockLevel}// unlock level"); }
            return sb.ToString();
        }
    }
}