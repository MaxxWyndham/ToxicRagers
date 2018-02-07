using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Core.Formats
{
    public class FBX
    {
        private static bool bDebug = false;

        int version;
        bool b64bit = false;
        List<FBXElem> elements = new List<FBXElem>();

        public int Version
        {
            get => version;
            set
            {
                version = value;
                b64bit = (version > 7400);
            }
        }

        public bool Is64bit
        {
            get => b64bit;
            set => b64bit = value;
        }

        public int BlockSentinelLength => (b64bit ? 25 : 13);
        public List<FBXElem> Elements => elements;
        public FBXElem FBXHeaderExtension => elements.Find(e => e.ID == "FBXHeaderExtension");

        public static FBX Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            FBX fbx = new FBX();

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            using (BinaryReader br = new BinaryReader(ms))
            {
                if (br.ReadString(20) != "Kaydara FBX Binary  " || br.ReadByte() != 0 || br.ReadByte() != 26 || br.ReadByte() != 0)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "Invalid Binary FBX detected, error!");
                    return null;
                }

                fbx.Version = (int)br.ReadUInt32();

                while (true)
                {
                    FBXElem elem = readElem(br, fbx);
                    if (elem == null) { break; }
                    fbx.elements.Add(elem);
                }
            }

            if (bDebug)
            {
                int depth = 0;
                Logger.LogLevel oldLevel = Logger.Level;

                Logger.Level = Logger.LogLevel.All;

                foreach (FBXElem elem in fbx.elements)
                {
                    debug(elem, ref depth);
                }

                Logger.Level = oldLevel;
            }

            return fbx;
        }

        private static void debug(FBXElem elem, ref int depth)
        {
            Logger.LogToFile(Logger.LogLevel.Debug, "{0}{1}", new string('\t', depth), elem.ID);

            depth++;
            string padding = new string('\t', depth);

            foreach (FBXProperty prop in elem.Properties)
            {
                Logger.LogToFile(Logger.LogLevel.Debug, "{0}{1}", padding, prop.Type);

                switch (prop.Type)
                {
                    case 82:
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}*{1} {2}", padding, ((byte[])prop.Value).Length, ((byte[])prop.Value).ToFormattedString());
                        break;

                    case 83:
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}\"{1}\"", padding, prop.Value);
                        break;

                    case 98:
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}*{1} {2}", padding, ((bool[])prop.Value).Length, ((bool[])prop.Value).ToFormattedString());
                        break;

                    case 100:
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}*{1} {2}", padding, ((double[])prop.Value).Length, ((double[])prop.Value).ToFormattedString());
                        break;

                    case 102:
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}*{1} {2}", padding, ((float[])prop.Value).Length, ((float[])prop.Value).ToFormattedString());
                        break;

                    case 105:
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}*{1} {2}", padding, ((int[])prop.Value).Length, ((int[])prop.Value).ToFormattedString());
                        break;

                    case 108:
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}*{1} {2}", padding, ((long[])prop.Value).Length, ((long[])prop.Value).ToFormattedString());
                        break;

                    default:
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}{1}", padding, prop.Value);
                        break;
                }
            }

            if (elem.Properties.Count > 0) { Logger.LogToFile(Logger.LogLevel.Debug, ""); }

            foreach (FBXElem child in elem.Children)
            {
                debug(child, ref depth);
            }

            if (elem.Children.Count > 0) { Logger.LogToFile(Logger.LogLevel.Debug, ""); }

            depth--;
        }

        private static FBXElem readElem(BinaryReader br, FBX fbx)
        {
            int endOffset = (fbx.Is64bit ? (int)br.ReadUInt64() : (int)br.ReadUInt32());
            if (endOffset == 0) { return null; }

            FBXElem elem = new FBXElem();

            int propCount = (fbx.Is64bit ? (int)br.ReadUInt64() : (int)br.ReadUInt32());
            int propLength = (fbx.Is64bit ? (int)br.ReadUInt64() : (int)br.ReadUInt32());

            elem.ID = br.ReadString();

            for (int i = 0; i < propCount; i++)
            {
                elem.Properties.Add(readDataDictionary(br, br.ReadByte(), fbx));
            }

            if (br.BaseStream.Position < endOffset)
            {
                while (br.BaseStream.Position < (endOffset - fbx.BlockSentinelLength))
                {
                    elem.Children.Add(readElem(br, fbx));
                }

                for (int i = 0; i < fbx.BlockSentinelLength; i++)
                {
                    if (br.ReadByte() != 0)
                    {
                        throw new InvalidDataException();
                    }
                }
            }

            return elem;
        }

        private static FBXProperty readDataDictionary(BinaryReader br, byte dataType, FBX fbx)
        {
            FBXProperty property = new FBXProperty() { Type = dataType };
            int encoding;
            int comLength;

            switch (dataType)
            {
                case 67:  // Bool
                    property.Value = (br.ReadByte() == 1);
                    break;

                case 68:  // Double
                    property.Value = br.ReadDouble();
                    break;

                case 70:  // Single
                    property.Value = br.ReadSingle();
                    break;

                case 73: // 32bit int
                    property.Value = (int)br.ReadUInt32();
                    break;

                case 76: // 64bit int
                    property.Value = (long)br.ReadUInt64();
                    break;

                case 82: // Byte array
                    property.Value = br.ReadBytes((int)br.ReadUInt32());
                    break;

                case 83: // String
                    property.Value = br.ReadPropertyString((int)br.ReadUInt32());
                    break;

                case 98: // bool array
                    {
                        bool[] array = new bool[(int)br.ReadUInt32()];
                        encoding = (int)br.ReadUInt32();
                        comLength = (int)br.ReadUInt32();

                        if (encoding == 0)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                array[i] = br.ReadBoolean();
                            }
                        }
                        else
                        {
                            br.BaseStream.Seek(2, SeekOrigin.Current);

                            using (MemoryStream ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                byte[] data = new byte[1 * array.Length];
                                ds.Read(data, 0, 1 * array.Length);
                                Buffer.BlockCopy(data, 0, array, 0, array.Length * 1);
                            }
                        }

                        property.Value = array;
                    }
                    break;

                case 100: // Double array
                    {
                        double[] array = new double[(int)br.ReadUInt32()];
                        encoding = (int)br.ReadUInt32();
                        comLength = (int)br.ReadUInt32();

                        if (encoding == 0)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                array[i] = br.ReadDouble();
                            }
                        }
                        else
                        {
                            br.BaseStream.Seek(2, SeekOrigin.Current);

                            using (MemoryStream ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                byte[] data = new byte[8 * array.Length];
                                ds.Read(data, 0, 8 * array.Length);
                                Buffer.BlockCopy(data, 0, array, 0, array.Length * 8);
                            }
                        }

                        property.Value = array;
                    }
                    break;

                case 102: // float array
                    {
                        float[] array = new float[(int)br.ReadUInt32()];
                        encoding = (int)br.ReadUInt32();
                        comLength = (int)br.ReadUInt32();

                        if (encoding == 0)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                array[i] = br.ReadSingle();
                            }
                        }
                        else
                        {
                            br.BaseStream.Seek(2, SeekOrigin.Current);

                            using (MemoryStream ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                byte[] data = new byte[4 * array.Length];
                                ds.Read(data, 0, 4 * array.Length);
                                Buffer.BlockCopy(data, 0, array, 0, array.Length * 4);
                            }
                        }

                        property.Value = array;
                    }
                    break;

                case 105: // int array
                    {
                        int[] array = new int[(int)br.ReadUInt32()];
                        encoding = (int)br.ReadUInt32();
                        comLength = (int)br.ReadUInt32();

                        if (encoding == 0)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                array[i] = br.ReadInt32();
                            }
                        }
                        else
                        {
                            br.BaseStream.Seek(2, SeekOrigin.Current);

                            using (MemoryStream ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                byte[] data = new byte[4 * array.Length];
                                ds.Read(data, 0, 4 * array.Length);
                                Buffer.BlockCopy(data, 0, array, 0, array.Length * 4);
                            }
                        }

                        property.Value = array;
                    }
                    break;

                case 108: // long array
                    {
                        long[] array = new long[(int)br.ReadUInt32()];
                        encoding = (int)br.ReadUInt32();
                        comLength = (int)br.ReadUInt32();

                        if (encoding == 0)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                array[i] = br.ReadInt64();
                            }
                        }
                        else
                        {
                            br.BaseStream.Seek(2, SeekOrigin.Current);

                            using (MemoryStream ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                byte[] data = new byte[8 * array.Length];
                                ds.Read(data, 0, 8 * array.Length);
                                Buffer.BlockCopy(data, 0, array, 0, array.Length * 8);
                            }
                        }

                        property.Value = array;
                    }
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unknown Data Type: {0}", dataType));
            }

            return property;
        }

        protected int CalcOffsetsChildrenRoot(int offset, bool isLast)
        {
            if (elements.Count > 0)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    offset = elements[i].CalcOffsets(offset, (i + 1 == elements.Count), this);
                }

                offset += BlockSentinelLength;
            }

            return offset;
        }

        protected void WriteChildrenRoot(BinaryWriter bw, bool isLast)
        {
            if (elements.Count > 0)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].Write(bw, (i + 1 == elements.Count), this);
                }

                bw.Write(new byte[BlockSentinelLength]);
            }
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                b64bit = false;
                version = 7400;

                bw.WriteString("Kaydara FBX Binary");
                bw.Write(new byte[] { 0x20, 0x20, 0x0, 0x1A, 0x0 });
                bw.Write(version);

                CalcOffsetsChildrenRoot((int)bw.BaseStream.Position, false);
                WriteChildrenRoot(bw, false);

                bw.Write(new byte[] { 0xfa, 0xbc, 0xab, 0x09, 0xd0, 0xc8, 0xd4, 0x66, 0xb1, 0x76, 0xfb, 0x83, 0x1c, 0xf7, 0x26, 0x7e });
                bw.Write(new byte[4]);

                int ofs = (int)bw.BaseStream.Position;
                int pad = ((ofs + 15) & ~15) - ofs;
                if (pad == 0) { pad = 16; }
                bw.Write(new byte[pad]);

                bw.Write(version);
                bw.Write(new byte[120]);
                bw.Write(new byte[] { 0xf8, 0x5a, 0x8c, 0x6a, 0xde, 0xf5, 0xd9, 0x7e, 0xec, 0xe9, 0x0c, 0xe3, 0x75, 0x8f, 0x29, 0x0b });
            }
        }
    }

    public class FBXElem
    {
        string id;
        int endOffset = -1;
        int propsLength = -1;
        List<FBXProperty> elemProps = new List<FBXProperty>();
        List<FBXElem> elemSubtree = new List<FBXElem>();

        public string ID
        {
            get => id;
            set => id = value;
        }

        public List<FBXProperty> Properties
        {
            get => elemProps;
            set => elemProps = value;
        }

        public List<FBXElem> Children
        {
            get => elemSubtree;
            set => elemSubtree = value;
        }

        public void Write(BinaryWriter bw, bool isLast, FBX fbx)
        {
            bw.Write(endOffset);
            bw.Write(elemProps.Count);
            bw.Write(propsLength);

            bw.Write(id);

            for (int i = 0; i < elemProps.Count; i++) { elemProps[i].Write(bw); }

            WriteChildren(bw, isLast, fbx);

            if (bw.BaseStream.Position != endOffset) { throw new DataMisalignedException(); }
        }

        protected void WriteChildren(BinaryWriter bw, bool isLast, FBX fbx)
        {
            if (elemSubtree.Count > 0)
            {
                for (int i = 0; i < elemSubtree.Count; i++)
                {
                    elemSubtree[i].Write(bw, (i + 1 == elemSubtree.Count), fbx);
                }

                bw.Write(new byte[fbx.BlockSentinelLength]);
            }
            else if (elemProps.Count == 0 && !isLast)
            {
                bw.Write(new byte[fbx.BlockSentinelLength]);
            }
        }

        public int CalcOffsets(int offset, bool isLast, FBX fbx)
        {
            offset += 12;
            offset += 1 + id.Length;

            propsLength = 0;
            for (int i = 0; i < elemProps.Count; i++)
            {
                propsLength += 1 + elemProps[i].Size;
            }
            offset += propsLength;

            offset = CalcOffsetsChildren(offset, isLast, fbx);

            endOffset = offset;
            return offset;
        }

        protected int CalcOffsetsChildren(int offset, bool isLast, FBX fbx)
        {
            if (elemSubtree.Count > 0)
            {
                for (int i = 0; i < elemSubtree.Count; i++)
                {
                    offset = elemSubtree[i].CalcOffsets(offset, (i + 1 == elemSubtree.Count), fbx);
                }

                offset += fbx.BlockSentinelLength;
            }
            else if (elemProps.Count == 0 && !isLast)
            {
                offset += fbx.BlockSentinelLength;
            }

            return offset;
        }
    }

    public class FBXProperty
    {
        byte propertyType;
        object propertyValue;

        bool compressed;
        byte[] rawData;

        public byte Type
        {
            get => propertyType;
            set => propertyType = value;
        }

        public object Value
        {
            get => propertyValue;
            set => propertyValue = value;
        }

        public bool Compressed
        {
            get => compressed;
            set => compressed = value;
        }

        public int Size
        {
            get
            {
                switch (propertyType)
                {
                    case 67:  // Bool
                        return 1;

                    case 68:  // Double
                        return 8;

                    case 70: // Single
                        return 4;

                    case 73: // 32bit int
                        return 4;

                    case 76: // 64bit int
                        return 8;

                    case 82: // Byte array
                        return 4 + ((byte[])propertyValue).Length;

                    case 83: // String
                        if (propertyValue == null) { propertyValue = ""; }
                        return 4 + Encoding.UTF8.GetBytes((string)propertyValue).Length;

                    case 98: // bool array
                        if (rawData == null)
                        {
                            rawData = new byte[((bool[])propertyValue).Length * sizeof(bool)];
                            Buffer.BlockCopy((bool[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = compress(rawData); }
                        }

                        return 12 + rawData.Length;

                    case 100: // Double array
                        if (rawData == null)
                        {
                            rawData = new byte[((double[])propertyValue).Length * sizeof(double)];
                            Buffer.BlockCopy((double[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = compress(rawData); }
                        }

                        return 12 + rawData.Length;

                    case 102: // float array
                        if (rawData == null)
                        {
                            rawData = new byte[((float[])propertyValue).Length * sizeof(float)];
                            Buffer.BlockCopy((float[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = compress(rawData); }
                        }

                        return 12 + rawData.Length;

                    case 105: // int array
                        if (rawData == null)
                        {
                            rawData = new byte[((int[])propertyValue).Length * sizeof(int)];
                            Buffer.BlockCopy((int[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = compress(rawData); }
                        }

                        return 12 + rawData.Length;

                    case 108: // long array
                        if (rawData == null)
                        {
                            rawData = new byte[((long[])propertyValue).Length * sizeof(long)];
                            Buffer.BlockCopy((long[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = compress(rawData); }
                        }

                        return 12 + rawData.Length;

                    default:
                        throw new NotImplementedException(string.Format("Unable to calculate the size of property type {0}", propertyType));
                }
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(propertyType);

            switch (propertyType)
            {
                case 67:  // Bool
                    bw.Write((bool)propertyValue);
                    break;

                case 68:  // Double
                    bw.Write((double)propertyValue);
                    break;

                case 70: // Single
                    bw.Write((float)propertyValue);
                    break;

                case 73: // 32bit int
                    bw.Write((int)propertyValue);
                    break;

                case 76: // 64bit int
                    bw.Write((long)propertyValue);
                    break;

                case 82: // Byte array
                    byte[] b = (byte[])propertyValue;
                    bw.Write(b.Length);
                    bw.Write(b);
                    break;

                case 83: // String
                    string s = (string)propertyValue;
                    bw.Write(Encoding.UTF8.GetBytes(s).Length);
                    bw.WritePropertyString(s);
                    break;

                case 98: // bool array
                    bool[] bo = (bool[])propertyValue;
                    bw.Write(bo.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                case 100: // Double array
                    double[] d = (double[])propertyValue;
                    bw.Write(d.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                case 102: // float array
                    float[] f = (float[])propertyValue;
                    bw.Write(f.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                case 105: // int array
                    int[] i = (int[])propertyValue;
                    bw.Write(i.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                case 108: // long array
                    long[] l = (long[])propertyValue;
                    bw.Write(l.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                default:
                    throw new NotImplementedException(string.Format("Unable to write property type {0}", propertyType));
            }
        }

        private static byte[] compress(byte[] input)
        {
            using (MemoryStream compressStream = new MemoryStream())
            using (DeflateStream compressor = new DeflateStream(compressStream, CompressionMode.Compress))
            {
                compressor.Write(input, 0, input.Length);
                compressor.Flush();
                compressor.Close();

                byte[] data = compressStream.ToArray();
                byte[] compressed = new byte[2 + data.Length + 4];
                compressed[0] = 0x58;
                compressed[1] = 0x85;
                data.CopyTo(compressed, 2);
                BitConverter.GetBytes(ReverseBytes(AdlerChecksum.Generate(ref input, 0, input.Length))).CopyTo(compressed, compressed.Length - 4);

                return compressed;
            }
        }

        public static uint ReverseBytes(uint value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
    }

    public class AdlerChecksum
    {
        public const uint AdlerStart = 0x0001;
        public const uint AdlerBase = 0xFFF1;

        public static uint Generate(ref byte[] buffer, int index, int length)
        {
            if (buffer == null || length - index <= 0) { return 0; }

            uint unSum1 = AdlerStart & 0xFFFF;
            uint unSum2 = (AdlerStart >> 16) & 0xFFFF;

            for (int i = index; i < length; i++)
            {
                unSum1 = (unSum1 + buffer[i]) % AdlerBase;
                unSum2 = (unSum1 + unSum2) % AdlerBase;
            }

            return (unSum2 << 16) + unSum1;
        }
    }
}