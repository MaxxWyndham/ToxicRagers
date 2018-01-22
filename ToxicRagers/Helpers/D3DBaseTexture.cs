using System.Collections.Specialized;
using System.IO;

namespace ToxicRagers.Helpers
{
    // http://dageron.com/?page_id=5238&lang=en

    public class D3DBaseTexture
    {
        int commonType;
        int referenceCount;
        int fence;
        int readFence;
        int identifier;
        int baseFlush;
        int mipFlush;

        int type;
        int signX;
        int signY;
        int signZ;
        int signW;
        int clampX;
        int clampY;
        int clampZ;
        int pitch;
        bool tiled;

        int baseAddress;
        int clampPolicy;
        bool stacked;
        int requestSize;
        int endian;
        D3DFormat dataFormat;

        int borderSize;
        int anisoFilter;
        int mipFilter;
        int minFilter;
        int magFilter;
        int expAdjust;
        int swizzleX;
        int swizzleY;
        int swizzleZ;
        int swizzleW;
        int numFormat;

        public string BaseFlush => baseFlush.ToString("X");
        public string MipFlush => mipFlush.ToString("X");
        public bool Tiled => tiled;
        public int Endian => endian;
        public D3DFormat DataFormat => dataFormat;

        public D3DBaseTexture(BinaryReader br)
        {
            BitVector32 bv = new BitVector32();

            commonType = br.ReadUInt16();
            referenceCount = (int)br.ReadUInt32();
            fence = (int)br.ReadUInt32();
            readFence = (int)br.ReadUInt32();
            identifier = (int)br.ReadUInt32();
            baseFlush = (int)br.ReadUInt32();
            mipFlush = (int)br.ReadUInt32();

            bv = new BitVector32((int)br.ReadUInt32());

            BitVector32.Section type = BitVector32.CreateSection(3);
            BitVector32.Section signx = BitVector32.CreateSection(3, type);
            BitVector32.Section signy = BitVector32.CreateSection(3, signx);
            BitVector32.Section signz = BitVector32.CreateSection(3, signy);
            BitVector32.Section signw = BitVector32.CreateSection(3, signz);
            BitVector32.Section clampx = BitVector32.CreateSection(7, signw);
            BitVector32.Section clampy = BitVector32.CreateSection(7, clampx);
            BitVector32.Section clampz = BitVector32.CreateSection(7, clampy);
            BitVector32.Section spacer = BitVector32.CreateSection(7, clampz);
            BitVector32.Section pitch = BitVector32.CreateSection(511, spacer);
            BitVector32.Section tiled = BitVector32.CreateSection(1, pitch);

            this.tiled = (bv[tiled] == 1);
            this.pitch = bv[pitch];
            clampX = bv[clampx];
            clampY = bv[clampy];
            clampZ = bv[clampz];
            signX = bv[signx];
            signY = bv[signy];
            signZ = bv[signz];
            signW = bv[signw];
            this.type = bv[type];

            bv = new BitVector32((int)br.ReadUInt32());

            BitVector32.Section dataformat = BitVector32.CreateSection(63);
            BitVector32.Section endian = BitVector32.CreateSection(3, dataformat);
            BitVector32.Section requestsize = BitVector32.CreateSection(3, endian);
            BitVector32.Section stacked = BitVector32.CreateSection(1, requestsize);
            BitVector32.Section clamppolicy = BitVector32.CreateSection(1, stacked);
            BitVector32.Section baseaddressa = BitVector32.CreateSection(1023, clamppolicy);
            BitVector32.Section baseaddressb = BitVector32.CreateSection(1023, baseaddressa);

            baseAddress = ((int)bv[baseaddressb] << 10) | bv[baseaddressa];
            clampPolicy = bv[clamppolicy];
            this.stacked = (bv[stacked] == 1);
            requestSize = bv[requestsize];
            this.endian = bv[endian];
            dataFormat = (D3DFormat)bv[dataformat];

            br.ReadUInt32();    // GPUTEXTURESIZE

            bv = new BitVector32((int)br.ReadUInt32());

            BitVector32.Section bordersize = BitVector32.CreateSection(1);
            spacer = BitVector32.CreateSection(7, bordersize);
            BitVector32.Section anisofilter = BitVector32.CreateSection(7, spacer);
            BitVector32.Section mipfilter = BitVector32.CreateSection(3, anisofilter);
            BitVector32.Section minfilter = BitVector32.CreateSection(3, mipfilter);
            BitVector32.Section magfilter = BitVector32.CreateSection(3, minfilter);
            BitVector32.Section expadjust = BitVector32.CreateSection(63, magfilter);
            BitVector32.Section swizzlew = BitVector32.CreateSection(7, expadjust);
            BitVector32.Section swizzlez = BitVector32.CreateSection(7, swizzlew);
            BitVector32.Section swizzley = BitVector32.CreateSection(7, swizzlez);
            BitVector32.Section swizzlex = BitVector32.CreateSection(7, swizzley);
            BitVector32.Section numformat = BitVector32.CreateSection(1, swizzlex);

            borderSize = bv[bordersize];
            anisoFilter = bv[anisofilter];
            mipFilter = bv[mipfilter];
            minFilter = bv[minfilter];
            magFilter = bv[magfilter];
            expAdjust = bv[expadjust];
            swizzleX = bv[swizzlex];
            swizzleY = bv[swizzley];
            swizzleZ = bv[swizzlez];
            swizzleW = bv[swizzlew];
            numFormat = bv[numformat];

            br.ReadBytes(8);
        }
    }
}