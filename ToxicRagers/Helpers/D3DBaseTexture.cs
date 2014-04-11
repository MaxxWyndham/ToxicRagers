using System;
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

        public string BaseFlush { get { return baseFlush.ToString("X"); } }
        public string MipFlush { get { return mipFlush.ToString("X"); } }
        public bool Tiled { get { return tiled; } }
        public int Endian { get { return endian; } }
        public D3DFormat DataFormat { get { return dataFormat; } }

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

            var type = BitVector32.CreateSection(3);
            var signx = BitVector32.CreateSection(3, type);
            var signy = BitVector32.CreateSection(3, signx);
            var signz = BitVector32.CreateSection(3, signy);
            var signw = BitVector32.CreateSection(3, signz);
            var clampx = BitVector32.CreateSection(7, signw);
            var clampy = BitVector32.CreateSection(7, clampx);
            var clampz = BitVector32.CreateSection(7, clampy);
            var spacer = BitVector32.CreateSection(7, clampz);
            var pitch = BitVector32.CreateSection(511, spacer);
            var tiled = BitVector32.CreateSection(1, pitch);

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

            var dataformat = BitVector32.CreateSection(63);
            var endian = BitVector32.CreateSection(3, dataformat);
            var requestsize = BitVector32.CreateSection(3, endian);
            var stacked = BitVector32.CreateSection(1, requestsize);
            var clamppolicy = BitVector32.CreateSection(1, stacked);
            var baseaddressa = BitVector32.CreateSection(1023, clamppolicy);
            var baseaddressb = BitVector32.CreateSection(1023, baseaddressa);

            baseAddress = ((int)bv[baseaddressb] << 10) | bv[baseaddressa];
            clampPolicy = bv[clamppolicy];
            this.stacked = (bv[stacked] == 1);
            requestSize = bv[requestsize];
            this.endian = bv[endian];
            dataFormat = (D3DFormat)bv[dataformat];

            br.ReadUInt32();    // GPUTEXTURESIZE

            bv = new BitVector32((int)br.ReadUInt32());

            var bordersize = BitVector32.CreateSection(1);
            spacer = BitVector32.CreateSection(7, bordersize);
            var anisofilter = BitVector32.CreateSection(7, spacer);
            var mipfilter = BitVector32.CreateSection(3, anisofilter);
            var minfilter = BitVector32.CreateSection(3, mipfilter);
            var magfilter = BitVector32.CreateSection(3, minfilter);
            var expadjust = BitVector32.CreateSection(63, magfilter);
            var swizzlew = BitVector32.CreateSection(7, expadjust);
            var swizzlez = BitVector32.CreateSection(7, swizzlew);
            var swizzley = BitVector32.CreateSection(7, swizzlez);
            var swizzlex = BitVector32.CreateSection(7, swizzley);
            var numformat = BitVector32.CreateSection(1, swizzlex);

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
