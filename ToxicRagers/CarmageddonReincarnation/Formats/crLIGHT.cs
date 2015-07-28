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
        Single r;
        Single g;
        Single b;
        Single intensity;
        Single range;
        Single inner;
        Single outer;
        Single nearClip = 0.2f;
        LightFlags flags;
        int shadResX = 128;
        int shadResY = 128;
        Single shadCoverX = 1;
        Single shadCoverY = 1;
        Single shadowBias = 0;
        Single shadIntensity = 1;
        int splitCount = 0;
        Single splitDistribution = 1;
        Single shadDistMin;
        Single shadDistMax = 2000;
        bool bUseEdgeCol;
        byte edgeColR;
        byte edgeColG;
        byte edgeColB;
        byte edgeColA;
        Single goboScaleX = 1;
        Single goboScaleY = 1;
        Single goboOffsetX = 0;
        Single goboOffsetY = 0;
        string goboTexture;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public LightType Type
        {
            get { return type; }
            set { type = value; }
        }

        public Single Range
        {
            get { return range; }
            set { range = value; }
        }

        public Single Inner
        {
            get { return inner; }
            set { inner = value; }
        }

        public Single Outer
        {
            get { return outer; }
            set { outer = value; }
        }

        public Single R
        {
            get { return r; }
            set { r = value; }
        }

        public Single G
        {
            get { return g; }
            set { g = value; }
        }

        public Single B
        {
            get { return b; }
            set { b = value; }
        }

        public Single Intensity
        {
            get { return intensity; }
            set { intensity = value; }
        }

        public LightFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public int SplitCount
        {
            get { return splitCount; }
            set { splitCount = value; }
        }

        public Single SplitDistribution
        {
            get { return splitDistribution; }
            set { splitDistribution = value; }
        }

        public Single ShadowCoverX
        {
            get { return shadCoverX; }
            set { shadCoverX = value; }
        }

        public Single ShadowCoverY
        {
            get { return shadCoverY; }
            set { shadCoverY = value; }
        }

        public int ShadowResolutionX
        {
            get { return shadResX; }
            set { shadResX = value; }
        }

        public int ShadowResolutionY
        {
            get { return shadResY; }
            set { shadResY = value; }
        }

        public Single ShadowIntensity
        {
            get { return shadIntensity; }
            set { shadIntensity = value; }
        }

        public Single GoboScaleX
        {
            get { return goboScaleX; }
            set { goboScaleX = value; }
        }

        public Single GoboScaleY
        {
            get { return goboScaleY; }
            set { goboScaleY = value; }
        }

        public Single GoboOffsetX
        {
            get { return goboOffsetX; }
            set { goboOffsetX = value; }
        }

        public Single GoboOffsetY
        {
            get { return goboOffsetY; }
            set { goboOffsetY = value; }
        }

        public Single ShadowBias
        {
            get { return shadowBias; }
            set { shadowBias = value; }
        }

        public Single LightNearClip
        {
            get { return nearClip; }
            set { nearClip = value; }
        }

        public Single ShadowDistance
        {
            get { return shadDistMax; }
            set { shadDistMax = value; }
        }

        public string GOBO
        {
            get { return goboTexture; }
            set { goboTexture = value; }
        }

        public bool UseEdgeColour
        {
            get { return bUseEdgeCol; }
            set { bUseEdgeCol = value; }
        }

        public byte EdgeColourR
        {
            get { return edgeColR; }
            set { edgeColR = value; }
        }

        public byte EdgeColourG
        {
            get { return edgeColG; }
            set { edgeColG = value; }
        }

        public byte EdgeColourB
        {
            get { return edgeColB; }
            set { edgeColB = value; }
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
            LIGHT light = new LIGHT();

            light.type = (LightType)br.ReadUInt32();

            light.r = br.ReadSingle();
            light.g = br.ReadSingle();
            light.b = br.ReadSingle();
            light.intensity = br.ReadSingle();
            light.range = br.ReadSingle();
            light.inner = br.ReadSingle();
            light.outer = br.ReadSingle();
            light.nearClip = br.ReadSingle();
            light.flags = (LightFlags)br.ReadUInt32();
            light.shadResX = (int)br.ReadUInt32();
            light.shadResY = (int)br.ReadUInt32();
            light.shadCoverX = br.ReadSingle();
            light.shadCoverY = br.ReadSingle();
            light.shadowBias = br.ReadSingle();
            light.shadIntensity = br.ReadSingle();
            light.splitCount = (int)br.ReadUInt32();
            light.splitDistribution = br.ReadSingle();
            light.shadDistMin = br.ReadSingle();
            light.shadDistMax = br.ReadSingle();
            light.bUseEdgeCol = (br.ReadUInt32() == 1);
            light.edgeColR = br.ReadByte();
            light.edgeColG = br.ReadByte();
            light.edgeColB = br.ReadByte();
            light.edgeColA = br.ReadByte();
            light.goboScaleX = br.ReadSingle();
            light.goboScaleY = br.ReadSingle();
            light.goboOffsetX = br.ReadSingle();
            light.goboOffsetY = br.ReadSingle();

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
