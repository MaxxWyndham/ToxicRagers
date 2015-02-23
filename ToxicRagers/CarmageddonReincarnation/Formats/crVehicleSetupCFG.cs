using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;
using ToxicRagers.CarmageddonReincarnation.Helpers;
using System.Text;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class VehicleSetupConfig
    {
        List<string> drivers;
        string aiScript;
        bool bEjectDriver = true;
        List<VehicleAttachment> attachments;
        List<VehicleWheelModule> wheelModules;
        List<VehicleMaterialMap> materialMaps;
        List<VehicleWheelMap> wheelMaps;
        List<Vector3> decalPoints;
        VehicleSuspensionFactors suspensionFactors;
        VehicleStats stats;

        public List<string> Drivers
        {
            get { return drivers; }
            set { drivers = value; }
        }

        public string AIScript
        {
            get { return aiScript; }
            set { aiScript = value; }
        }

        public bool EjectDriver
        {
            get { return bEjectDriver; }
            set { bEjectDriver = value; }
        }

        public List<VehicleAttachment> Attachments
        {
            get { return attachments; }
            set { attachments = value; }
        }

        public List<VehicleWheelModule> WheelModules
        {
            get { return wheelModules; }
            set { wheelModules = value; }
        }

        public List<VehicleMaterialMap> MaterialMaps
        {
            get { return materialMaps; }
            set { materialMaps = value; }
        }

        public List<VehicleWheelMap> WheelMaps
        {
            get { return wheelMaps; }
            set { wheelMaps = value; }
        }

        public List<Vector3> DecalPoints
        {
            get { return decalPoints; }
            set { decalPoints = value; }
        }

        public VehicleSuspensionFactors SuspensionFactors
        {
            get { return suspensionFactors; }
            set { suspensionFactors = value; }
        }

        public VehicleStats Stats
        {
            get { return stats; }
            set { stats = value; }
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
        }

        public static VehicleSetupConfig Load(string pathToFile)
        {
            VehicleSetupConfig setup = new VehicleSetupConfig();

            using (var doc = new DocumentParser(pathToFile))
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
            using (var sw = new StreamWriter(path + "\\vehicle_setup.cfg"))
            {
                if (drivers.Count > 0)
                {
                    sw.WriteLine("[default_driver]");
                    sw.WriteLine(drivers[0]);
                    sw.WriteLine();
                }

                foreach (var attachment in attachments)
                {
                    sw.WriteLine(attachment.Write());
                }

                foreach (var wheelModule in wheelModules)
                {
                    sw.WriteLine(wheelModule.Write());
                }

                if (suspensionFactors != null) { sw.WriteLine(suspensionFactors.Write()); }

                foreach (var materialMap in materialMaps)
                {
                    sw.WriteLine(materialMap.Write());
                }

                sw.WriteLine(stats.Write());
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
            get { return attachmentType; }
            set { attachmentType = value; }
        }

        public VehicleAttachmentFModEngine FModEngine
        {
            get { return engine; }
            set { engine = value; }
        }

        public VehicleAttachmentComplicatedWheels Wheels
        {
            get { return wheels; }
            set { wheels = value; }
        }

        public VehicleAttachmentExhaust Exhaust
        {
            get { return exhaust; }
            set { exhaust = value; }
        }

        public string Horn
        {
            get { return horn; }
            set { horn = value; }
        }

        public string ReverseLightSound
        {
            get { return reverseLightSound; }
            set { reverseLightSound = value; }
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
                    this.attachmentType = AttachmentType.DynamicsWheels;
                    break;

                case "ComplicatedWheels":
                    this.attachmentType = AttachmentType.ComplicatedWheels;
                    this.wheels = new VehicleAttachmentComplicatedWheels();

                    while (!doc.NextLineIsASection())
                    {
                        var cw = doc.ReadStringArray(2);

                        switch (cw[0])
                        {
                            case "fl_wheel_folder_name":
                                this.wheels.FLWheel = cw[1];
                                break;

                            case "fr_wheel_folder_name":
                                this.wheels.FRWheel = cw[1];
                                break;

                            case "rl_wheel_folder_name":
                                this.wheels.RLWheel = cw[1];
                                break;

                            case "rr_wheel_folder_name":
                                this.wheels.RRWheel = cw[1];
                                break;

                            case "wheel_folder_name":
                                this.wheels.FLWheel = cw[1];
                                this.wheels.FRWheel = cw[1];
                                this.wheels.RLWheel = cw[1];
                                this.wheels.RRWheel = cw[1];
                                break;

                            default:
                                throw new NotImplementedException("Unknown ComplicatedWheels parameter: " + cw[0]);
                        }
                    }
                    break;

                case "DynamicsFmodEngine":
                    this.attachmentType = AttachmentType.DynamicsFmodEngine;
                    this.engine = new VehicleAttachmentFModEngine();

                    while (!doc.NextLineIsASection())
                    {
                        var dfe = doc.ReadStringArray(2);

                        switch (dfe[0])
                        {
                            case "engine":
                                this.engine.Engine = dfe[1];
                                break;

                            case "rpmsmooth":
                                this.engine.RPMSmooth = Single.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            case "onloadsmooth":
                                this.engine.OnLoadSmooth = Single.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            case "offloadsmooth":
                                this.engine.OffLoadSmooth = Single.Parse(dfe[1], ToxicRagers.Culture);
                                break;

                            case "max_revs":
                                this.engine.MaxRevs = int.Parse(dfe[1]);
                                break;

                            case "min_revs":
                                this.engine.MinRevs = int.Parse(dfe[1]);
                                break;

                            default:
                                throw new NotImplementedException("Unknown DynamicsFmodEngine parameter: " + dfe[0]);
                        }
                    }
                    break;

                case "Horn":
                    this.attachmentType = AttachmentType.Horn;

                    var h = doc.ReadStringArray(2);
                    this.horn = h[1];
                    break;

                case "ExhaustParticles":
                    this.attachmentType = AttachmentType.ExhaustParticles;
                    this.exhaust = new VehicleAttachmentExhaust();

                    while (!doc.NextLineIsASection())
                    {
                        var ep = doc.ReadStringArray(2);

                        switch (ep[0])
                        {
                            case "vfx":
                                this.exhaust.VFX = ep[1];
                                break;

                            case "underwater_vfx":
                                this.exhaust.UnderwaterVFX = ep[1];
                                break;

                            case "anchor":
                                this.exhaust.Anchor = ep[1];
                                break;

                            default:
                                throw new NotImplementedException("Unknown ExhaustParticle parameter: " + ep[0]);
                        }
                    }
                    break;

                case "ReverseLightSound":
                    this.attachmentType = AttachmentType.ReverseLightSound;

                    var rl = doc.ReadStringArray(2);
                    this.reverseLightSound = rl[1];
                    break;

                case "ContinuousSound":
                    this.attachmentType = AttachmentType.ContinuousSound;

                    while (!doc.NextLineIsASection())
                    {
                        var cs = doc.ReadStringArray(2);

                        switch (cs[0])
                        {
                            case "sound":
                                this.continuousSound = cs[1];
                                break;

                            case "lump":
                                this.continuousSoundLump = cs[1];
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
            var sb = new StringBuilder();
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
                    sb.AppendLine(string.Format("event {0}", horn));
                    break;
            }

            return sb.ToString();
        }
    }

    public class VehicleAttachmentFModEngine
    {
        string engine;
        Single rpmSmooth;
        Single onLoadSmooth;
        Single offLoadSmooth;
        int maxRevs;
        int minRevs;

        public string Engine
        {
            get { return engine; }
            set { engine = value; }
        }

        public Single RPMSmooth
        {
            get { return rpmSmooth; }
            set { rpmSmooth = value; }
        }

        public Single OnLoadSmooth
        {
            get { return onLoadSmooth; }
            set { onLoadSmooth = value; }
        }

        public Single OffLoadSmooth
        {
            get { return offLoadSmooth; }
            set { offLoadSmooth = value; }
        }

        public int MaxRevs
        {
            get { return maxRevs; }
            set { maxRevs = value; }
        }

        public int MinRevs
        {
            get { return minRevs; }
            set { minRevs = value; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("engine {0}\r\n", engine);
            if (rpmSmooth != default(Single)) { sb.AppendFormat("rpmsmooth {0}\r\n", rpmSmooth); }
            if (onLoadSmooth != default(Single)) { sb.AppendFormat("onloadsmooth {0}\r\n", onLoadSmooth); }
            if (offLoadSmooth != default(Single)) { sb.AppendFormat("offloadsmooth {0}\r\n", offLoadSmooth); }
            return sb.ToString();
        }
    }

    public class VehicleAttachmentExhaust
    {
        string vfx;
        string underwaterVFX;
        string anchor;

        public string VFX
        {
            get { return vfx; }
            set { vfx = value; }
        }

        public string UnderwaterVFX
        {
            get { return underwaterVFX; }
            set { underwaterVFX = value; }
        }

        public string Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }
    }

    public class VehicleAttachmentComplicatedWheels
    {
        string flWheel;
        string frWheel;
        string rlWheel;
        string rrWheel;

        public string FLWheel { get { return flWheel; } set { flWheel = value; } }
        public string FRWheel { get { return frWheel; } set { frWheel = value; } }
        public string RLWheel { get { return rlWheel; } set { rlWheel = value; } }
        public string RRWheel { get { return rrWheel; } set { rrWheel = value; } }

        public override string ToString()
        {
            if (flWheel == frWheel && rlWheel == rrWheel && flWheel == rlWheel)
            {
                return "wheel_folder_name " + flWheel;
            }
            else
            {
                return string.Format("fl_wheel_folder_name {0}\r\nfr_wheel_folder_name {1}\r\nrl_wheel_folder_name {2}\r\nrr_wheel_folder_name {3}", flWheel, frWheel, rlWheel, rrWheel);
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

        public WheelModuleType Type
        {
            get { return wheelModuleType; }
            set { wheelModuleType = value; }
        }

        public string SkidMarkImage
        {
            get { return skidMarkImage; }
            set { skidMarkImage = value; }
        }

        public string SkidNoiseSound
        {
            get { return skidNoiseSound; }
            set { skidNoiseSound = value; }
        }

        public string TyreParticleVFX
        {
            get { return tyreParticleVFX; }
            set { tyreParticleVFX = value; }
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
                    this.wheelModuleType = WheelModuleType.SkidMarks;
                    this.skidMarkImage = doc.ReadStringArray(2)[1];
                    break;

                case "TyreParticles":
                case "TyreSmokeVFX":
                    this.wheelModuleType = WheelModuleType.TyreParticles;
                    this.tyreParticleVFX = doc.ReadStringArray(2)[1];
                    break;

                case "SkidNoise":
                    this.wheelModuleType = WheelModuleType.SkidNoise;
                    this.skidNoiseSound = doc.ReadStringArray(2)[1];
                    break;

                default:
                    throw new NotImplementedException("Unknown WheelModuleType: " + s);
            }
        }

        public string Write()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[wheel_module]");
            sb.AppendLine(wheelModuleType.ToString());

            switch (wheelModuleType)
            {
                case WheelModuleType.TyreParticles:
                    sb.AppendLine(string.Format("vfx {0}", tyreParticleVFX));
                    break;

                case WheelModuleType.SkidNoise:
                    sb.AppendLine(string.Format("sounds {0}", skidNoiseSound));
                    break;

                case WheelModuleType.SkidMarks:
                    sb.AppendLine(string.Format("image {0}", skidMarkImage));
                    break;
            }

            return sb.ToString();
        }
    }

    public class VehicleMaterialMap
    {
        string name;
        Vector3 shrapnel;
        string localName;
        string paint;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Vector3 Shrapnel
        {
            get { return shrapnel; }
            set { shrapnel = value; }
        }

        public string Localisation
        {
            get { return localName; }
            set { localName = value; }
        }

        public string Paint
        {
            get { return paint; }
            set { paint = value; }
        }

        public VehicleMaterialMap(DocumentParser doc)
        {
            this.name = doc.ReadNextLine();

            while (!doc.NextLineIsASection())
            {
                var mm = doc.ReadStringArray();

                switch (mm[0].ToLower())
                {
                    case "shrapnel":
                        this.shrapnel = Vector3.Parse(mm[1]);
                        break;

                    case "localise":
                        this.localName = mm[1];
                        break;

                    case "paint":
                        this.paint = mm[2];
                        break;

                    default:
                        throw new NotImplementedException("Unknown MaterialMap parameter: " + mm[0]);
                }
            }
        }

        public string Write()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[material_map]");
            sb.AppendLine();
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
            get { return name; }
            set { name = value; }
        }

        public string Localisation
        {
            get { return localName; }
            set { localName = value; }
        }

        public VehicleAttachmentComplicatedWheels Wheels
        {
            get { return wheels; }
            set { wheels = value; }
        }

        public VehicleWheelMap(DocumentParser doc)
        {
            this.name = doc.ReadNextLine();
            this.wheels = new VehicleAttachmentComplicatedWheels();

            while (!doc.NextLineIsASection() && !doc.EOF())
            {
                var wm = doc.ReadStringArray(2);

                switch (wm[0])
                {
                    case "localise":
                        this.localName = wm[1];
                        break;

                    case "fl_wheel_folder_name":
                        this.wheels.FLWheel = wm[1];
                        break;

                    case "fr_wheel_folder_name":
                        this.wheels.FRWheel = wm[1];
                        break;

                    case "rl_wheel_folder_name":
                        this.wheels.RLWheel = wm[1];
                        break;

                    case "rr_wheel_folder_name":
                        this.wheels.RRWheel = wm[1];
                        break;

                    case "wheel_folder_name":
                        this.wheels.FLWheel = wm[1];
                        this.wheels.FRWheel = wm[1];
                        this.wheels.RLWheel = wm[1];
                        this.wheels.RRWheel = wm[1];
                        break;

                    default:
                        throw new NotImplementedException("Unknown WheelMap parameter: " + wm[0]);
                }
            }
        }

        public string Write()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[wheel_map]");
            sb.AppendLine();
            return sb.ToString();
        }
    }

    public class VehicleSuspensionFactors
    {
        Single maxCompression;
        Single rideHeight;
        int maxSteeringLock;
        Single maxExtension;

        public Single MaxCompression
        {
            get { return maxCompression; }
            set { maxCompression = value; }
        }

        public Single RightHeight
        {
            get { return rideHeight; }
            set { rideHeight = value; }
        }

        public int MaxSteeringLock
        {
            get { return maxSteeringLock; }
            set { maxSteeringLock = value; }
        }

        public Single MaxExtension
        {
            get { return maxExtension; }
            set { maxExtension = value; }
        }

        public VehicleSuspensionFactors(DocumentParser doc)
        {
            while (!doc.NextLineIsASection())
            {
                var sf = doc.ReadStringArray(2);

                switch (sf[0])
                {
                    case "max_compression":
                        this.maxCompression = sf[1].ToSingle();
                        break;

                    case "ride_height":
                        this.rideHeight = sf[1].ToSingle();
                        break;

                    case "max_steering_lock":
                        this.maxSteeringLock = sf[1].ToInt();
                        break;

                    case "max_extension":
                        this.maxExtension = sf[1].ToSingle();
                        break;

                    default:
                        throw new NotImplementedException("Unknown SuspensionFactor parameter: " + sf[0]);
                }
            }
        }

        public string Write()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[suspension_factors]");
            sb.AppendLine();
            return sb.ToString();
        }
    }

    public class VehicleStats
    {
        int topSpeed;
        Single time;
        Single weight;
        Single toughness;
        Single unlockLevel = -1;

        public int TopSpeed
        {
            get { return topSpeed; }
            set { topSpeed = value; }
        }

        public Single Time
        {
            get { return time; }
            set { time = value; }
        }

        public Single Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public Single Toughness
        {
            get { return toughness; }
            set { toughness = value; }
        }

        public Single UnlockLevel
        {
            get { return unlockLevel; }
            set { unlockLevel = value; }
        }

        public string Write()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[stats]");
            sb.AppendLine(string.Format("{0}// top speed; they must be in this order and not have spaces before the comments", topSpeed));
            sb.AppendLine(string.Format("{0}// time 0 -60", time));
            sb.AppendLine(string.Format("{0}// weight", weight));
            sb.AppendLine(string.Format("{0}// toughness", toughness));
            if (unlockLevel != -1) { sb.AppendLine(string.Format("{0}// unlock level", unlockLevel)); }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
