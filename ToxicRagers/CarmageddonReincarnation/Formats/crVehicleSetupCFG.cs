using System;
using System.Collections.Generic;
using ToxicRagers.Helpers;
using ToxicRagers.CarmageddonReincarnation.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class VehicleSetupConfig
    {
        string defaultDriver;
        string aiScript;
        bool bEjectDriver = true;
        List<VehicleAttachment> attachments;
        List<VehicleWheelModule> wheelModules;
        List<VehicleMaterialMap> materialMaps;
        VehicleSuspensionFactors suspensionFactors;
        VehicleStats stats;

        public string DefaultDriver
        {
            get { return defaultDriver; }
            set { defaultDriver = value; }
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
            attachments = new List<VehicleAttachment>();
            wheelModules = new List<VehicleWheelModule>();
            materialMaps = new List<VehicleMaterialMap>();
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
                            setup.DefaultDriver = doc.ReadNextLine();
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

                        case "[disable_ejection]":
                            setup.EjectDriver = false;
                            break;

                        case "[stats]":
                            setup.Stats.TopSpeed = doc.ReadInt();
                            setup.Stats.Time = doc.ReadFloat();
                            setup.Stats.Weight = doc.ReadFloat();
                            setup.Stats.Toughness = doc.ReadFloat();
                            if (!doc.EOF()) { setup.Stats.UnlockLevel = doc.ReadInt(); }
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
            ReverseLightSound
        }

        AttachmentType attachmentType;
        VehicleAttachmentComplicatedWheels wheels;
        VehicleAttachmentFModEngine engine;
        VehicleAttachmentExhaust exhaust;
        string horn;
        string reverseLightSound;

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

                default:
                    throw new NotImplementedException("Unknown AttachmentType: " + s);
            }
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
    }

    public class VehicleAttachmentExhaust
    {
        string vfx;
        string anchor;

        public string VFX
        {
            get { return vfx; }
            set { vfx = value; }
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
    }

    public class VehicleMaterialMap
    {
        string name;
        Vector3 shrapnel;

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

        public VehicleMaterialMap(DocumentParser doc)
        {
            this.name = doc.ReadNextLine();

            while (!doc.NextLineIsASection())
            {
                var mm = doc.ReadStringArray(2);

                switch (mm[0])
                {
                    case "shrapnel":
                        this.shrapnel = Vector3.Parse(mm[1]);
                        break;

                    default:
                        throw new NotImplementedException("Unknown MaterialMap parameter: " + mm[0]);
                }
            }
        }
    }

    public class VehicleSuspensionFactors
    {
        Single maxCompression;
        Single rideHeight;
        int maxSteeringLock;

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

        public VehicleSuspensionFactors(DocumentParser doc)
        {
            while (!doc.NextLineIsASection())
            {
                var sf = doc.ReadStringArray(2);

                switch (sf[0])
                {
                    case "max_compression":
                        this.maxCompression = Single.Parse(sf[1], ToxicRagers.Culture);
                        break;

                    case "ride_height":
                        this.rideHeight = Single.Parse(sf[1], ToxicRagers.Culture);
                        break;

                    case "max_steering_lock":
                        this.maxSteeringLock = int.Parse(sf[1]);
                        break;

                    default:
                        throw new NotImplementedException("Unknown SuspensionFactor parameter: " + sf[0]);
                }
            }
        }
    }

    public class VehicleStats
    {
        int topSpeed;
        Single time;
        Single weight;
        Single toughness;
        int unlockLevel;

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

        public int UnlockLevel
        {
            get { return unlockLevel; }
            set { unlockLevel = value; }
        }
    }
}
