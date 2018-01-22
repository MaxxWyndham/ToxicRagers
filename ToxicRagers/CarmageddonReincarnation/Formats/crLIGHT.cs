using System;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class LIGHT
    {
        public enum LightType
        {
            Point = 0,
            Directional = 1,
            Spot = 2
        }

        [Flags]
        public enum LightFlags
        {
            CastShadow = 0x1,
            UsesGobo = 0x4,
            Unknown8 = 0x8,
            UsePool = 0x100
        }

        string name;

        LightType type;
        float r;
        float g;
        float b;
        float intensity;
        float range;
        float inner;
        float outer;
        float nearClip = 0.2f;
        LightFlags flags;
        int shadResX = 128;
        int shadResY = 128;
        float shadCoverX = 1;
        float shadCoverY = 1;
        float shadowBias = 0;
        float shadIntensity = 1;
        int splitCount = 0;
        float splitDistribution = 1;
        float shadDistMin;
        float shadDistMax = 2000;
        bool bUseEdgeCol;
        byte edgeColR;
        byte edgeColG;
        byte edgeColB;
        byte edgeColA;
        float goboScaleX = 1;
        float goboScaleY = 1;
        float goboOffsetX = 0;
        float goboOffsetY = 0;
        string goboTexture;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public LightType Type
        {
            get => type;
            set => type = value;
        }

        public float Range
        {
            get => range;
            set => range = value;
        }

        public float Inner
        {
            get => inner;
            set => inner = value;
        }

        public float Outer
        {
            get => outer;
            set => outer = value;
        }

        public float R
        {
            get => r;
            set => r = value;
        }

        public float G
        {
            get => g;
            set => g = value;
        }

        public float B
        {
            get => b;
            set => b = value;
        }

        public float Intensity
        {
            get => intensity;
            set => intensity = value;
        }

        public LightFlags Flags
        {
            get => flags;
            set => flags = value;
        }

        public int SplitCount
        {
            get => splitCount;
            set => splitCount = value;
        }

        public float SplitDistribution
        {
            get => splitDistribution;
            set => splitDistribution = value;
        }

        public float ShadowCoverX
        {
            get => shadCoverX;
            set => shadCoverX = value;
        }

        public float ShadowCoverY
        {
            get => shadCoverY;
            set => shadCoverY = value;
        }

        public int ShadowResolutionX
        {
            get => shadResX;
            set => shadResX = value;
        }

        public int ShadowResolutionY
        {
            get => shadResY;
            set => shadResY = value;
        }

        public float ShadowIntensity
        {
            get => shadIntensity;
            set => shadIntensity = value;
        }

        public float GoboScaleX
        {
            get => goboScaleX;
            set => goboScaleX = value;
        }

        public float GoboScaleY
        {
            get => goboScaleY;
            set => goboScaleY = value;
        }

        public float GoboOffsetX
        {
            get => goboOffsetX;
            set => goboOffsetX = value;
        }

        public float GoboOffsetY
        {
            get => goboOffsetY;
            set => goboOffsetY = value;
        }

        public float ShadowBias
        {
            get => shadowBias;
            set => shadowBias = value;
        }

        public float LightNearClip
        {
            get => nearClip;
            set => nearClip = value;
        }

        public float ShadowDistance
        {
            get => shadDistMax;
            set => shadDistMax = value;
        }

        public string GOBO
        {
            get => goboTexture;
            set => goboTexture = value;
        }

        public bool UseEdgeColour
        {
            get => bUseEdgeCol;
            set => bUseEdgeCol = value;
        }

        public byte EdgeColourR
        {
            get => edgeColR;
            set => edgeColR = value;
        }

        public byte EdgeColourG
        {
            get => edgeColG;
            set => edgeColG = value;
        }

        public byte EdgeColourB
        {
            get => edgeColB;
            set => edgeColB = value;
        }

        public static LIGHT Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            LIGHT light;

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadUInt32() != 3)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid LIGHT file", path);
                    return null;
                }

                light = Load(br);
                light.name = Path.GetFileNameWithoutExtension(path);

                if (br.BaseStream.Position != br.BaseStream.Length) { Logger.LogToFile(Logger.LogLevel.Warning, "Incomplete"); }
            }

            return light;
        }

        public static LIGHT Load(BinaryReader br)
        {
            LIGHT light = new LIGHT()
            {
                type = (LightType)br.ReadUInt32(),

                r = br.ReadSingle(),
                g = br.ReadSingle(),
                b = br.ReadSingle(),
                intensity = br.ReadSingle(),
                range = br.ReadSingle(),
                inner = br.ReadSingle(),
                outer = br.ReadSingle(),
                nearClip = br.ReadSingle(),
                flags = (LightFlags)br.ReadUInt32(),
                shadResX = (int)br.ReadUInt32(),
                shadResY = (int)br.ReadUInt32(),
                shadCoverX = br.ReadSingle(),
                shadCoverY = br.ReadSingle(),
                shadowBias = br.ReadSingle(),
                shadIntensity = br.ReadSingle(),
                splitCount = (int)br.ReadUInt32(),
                splitDistribution = br.ReadSingle(),
                shadDistMin = br.ReadSingle(),
                shadDistMax = br.ReadSingle(),
                bUseEdgeCol = (br.ReadUInt32() == 1),
                edgeColR = br.ReadByte(),
                edgeColG = br.ReadByte(),
                edgeColB = br.ReadByte(),
                edgeColA = br.ReadByte(),
                goboScaleX = br.ReadSingle(),
                goboScaleY = br.ReadSingle(),
                goboOffsetX = br.ReadSingle(),
                goboOffsetY = br.ReadSingle()
            };

            int nameLength = (int)br.ReadUInt32();
            int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

            light.goboTexture = br.ReadString(nameLength);
            br.ReadBytes(padding);

            return light;
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Create(path)))
            {
                bw.Write(3);
                Save(bw, this);
            }
        }

        public static void Save(BinaryWriter bw, LIGHT light)
        {
            bw.Write((int)light.type);
            bw.Write(light.r);
            bw.Write(light.g);
            bw.Write(light.b);
            bw.Write(light.intensity);
            bw.Write(light.range);
            bw.Write(light.inner);
            bw.Write(light.outer);
            bw.Write(light.nearClip);
            bw.Write((int)light.flags);
            bw.Write(light.shadResX);
            bw.Write(light.shadResY);
            bw.Write(light.shadCoverX);
            bw.Write(light.shadCoverY);
            bw.Write(light.shadowBias);
            bw.Write(light.shadIntensity);
            bw.Write(light.splitCount);
            bw.Write(light.splitDistribution);
            bw.Write(light.shadDistMin);
            bw.Write(light.shadDistMax);
            bw.Write((light.bUseEdgeCol ? 1 : 0));
            bw.Write(light.edgeColR);
            bw.Write(light.edgeColG);
            bw.Write(light.edgeColB);
            bw.Write(light.edgeColA);
            bw.Write(light.goboScaleX);
            bw.Write(light.goboScaleY);
            bw.Write(light.goboOffsetX);
            bw.Write(light.goboOffsetY);

            if (light.goboTexture != null)
            {
                int nameLength = light.goboTexture.Length;
                int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                bw.Write(nameLength);
                bw.WriteString(light.goboTexture);
                bw.Write(new byte[padding]);
            }
            else
            {
                bw.Write(0);
            }
        }
    }
}