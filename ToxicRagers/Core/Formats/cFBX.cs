using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using ToxicRagers.Helpers;

namespace ToxicRagers.Core.Formats
{
    public class FBX
    {
        public static int BLOCK_SENTINEL_LENGTH = 13;
        private static bool bDebug = false;

        int version;
        List<FBXElem> elements = new List<FBXElem>();

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        public List<FBXElem> Elements { get { return elements; } }

        public static FBX Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            FBX fbx = new FBX();

            using (var br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadString(20) != "Kaydara FBX Binary  " || br.ReadByte() != 0 || br.ReadByte() != 26 || br.ReadByte() != 0) 
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "Invalid Binary FBX detected, error!");
                    return null;
                }

                fbx.version = (int)br.ReadUInt32();

                while (true)
                {
                    var elem = read_elem(br);
                    if (elem == null) { break; }
                    fbx.elements.Add(elem);
                }
            }

            if (bDebug)
            {
                int depth = 0;
                Logger.LogLevel oldLevel = Logger.Level;

                Logger.Level = Logger.LogLevel.All;

                foreach (var elem in fbx.elements)
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

            foreach (var prop in elem.Properties)
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

            foreach (var child in elem.Children)
            {
                debug(child, ref depth);
            }

            if (elem.Children.Count > 0) { Logger.LogToFile(Logger.LogLevel.Debug, ""); }

            depth--;
        }

        private static FBXElem read_elem(BinaryReader br)
        {
            int endOffset = (int)br.ReadUInt32();
            if (endOffset == 0) { return null; }

            var elem = new FBXElem();

            int propCount = (int)br.ReadUInt32();
            int propLength = (int)br.ReadUInt32();

            elem.ID = br.ReadString();

            for (int i = 0; i < propCount; i++)
            {
                elem.Properties.Add(read_data_dict(br, br.ReadByte()));
            }

            if (br.BaseStream.Position < endOffset)
            {
                while (br.BaseStream.Position < (endOffset - BLOCK_SENTINEL_LENGTH))
                {
                    elem.Children.Add(read_elem(br));
                }

                for (int i = 0; i < BLOCK_SENTINEL_LENGTH; i++)
                {
                    if (br.ReadByte() != 0)
                    {
                        throw new InvalidDataException();
                    }
                }
            }

            return elem;
        }

        private static FBXProperty read_data_dict(BinaryReader br, byte dataType)
        {
            var property = new FBXProperty();
            property.Type = dataType;

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
                        var array = new bool[(int)br.ReadUInt32()];
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

                            using (var ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                var data = new byte[1 * array.Length];
                                ds.Read(data, 0, 1 * array.Length);
                                Buffer.BlockCopy(data, 0, array, 0, array.Length * 1);
                            }
                        }

                        property.Value = array;
                    }
                    break;

                case 100: // Double array
                    {
                        var array = new double[(int)br.ReadUInt32()];
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

                            using (var ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                var data = new byte[8 * array.Length];
                                ds.Read(data, 0, 8 * array.Length);
                                Buffer.BlockCopy(data, 0, array, 0, array.Length * 8);
                            }
                        }

                        property.Value = array;
                    }
                    break;

                case 102: // float array
                    {
                        var array = new float[(int)br.ReadUInt32()];
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

                            using (var ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                var data = new byte[4 * array.Length];
                                ds.Read(data, 0, 4 * array.Length);
                                Buffer.BlockCopy(data, 0, array, 0, array.Length * 4);
                            }
                        }

                        property.Value = array;
                    }
                    break;

                case 105: // int array
                    {
                        var array = new int[(int)br.ReadUInt32()];
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

                            using (var ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                var data = new byte[4 * array.Length];
                                ds.Read(data, 0, 4 * array.Length);
                                Buffer.BlockCopy(data, 0, array, 0, array.Length * 4);
                            }
                        }

                        property.Value = array;
                    }
                    break;

                case 108: // long array
                    {
                        var array = new long[(int)br.ReadUInt32()];
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

                            using (var ms = new MemoryStream(br.ReadBytes(comLength - 2)))
                            using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                var data = new byte[8 * array.Length];
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

        protected int calc_offsets_children_root(int offset, bool isLast)
        {
            if (this.elements.Count > 0)
            {
                for (int i = 0; i < this.elements.Count; i++)
                {
                    offset = this.elements[i].calc_offsets(offset, (i + 1 == this.elements.Count));
                }

                offset += BLOCK_SENTINEL_LENGTH;
            }

            return offset;
        }

        protected void write_children_root(BinaryWriter bw, bool isLast)
        {
            if (this.elements.Count > 0)
            {
                for (int i = 0; i < this.elements.Count; i++)
                {
                    this.elements[i].write(bw, (i + 1 == this.elements.Count));
                }

                bw.Write(new byte[BLOCK_SENTINEL_LENGTH]);
            }
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                bw.WriteString("Kaydara FBX Binary");
                bw.Write(new byte[] { 0x20, 0x20, 0x0, 0x1A, 0x0 });
                bw.Write(this.version);

                this.calc_offsets_children_root((int)bw.BaseStream.Position, false);
                this.write_children_root(bw, false);

                bw.Write(new byte[] { 0xfa, 0xbc, 0xab, 0x09, 0xd0, 0xc8, 0xd4, 0x66, 0xb1, 0x76, 0xfb, 0x83, 0x1c, 0xf7, 0x26, 0x7e });
                bw.Write(new byte[4]);

                int ofs = (int)bw.BaseStream.Position;
                int pad = ((ofs + 15) & ~15) - ofs;
                if (pad == 0) { pad = 16; }
                bw.Write(new byte[pad]);

                bw.Write(this.version);
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
            get { return id; }
            set { id = value; }
        }

        public List<FBXProperty> Properties
        {
            get { return elemProps; }
            set { elemProps = value; }
        }

        public List<FBXElem> Children
        {
            get { return elemSubtree; }
            set { elemSubtree = value; }
        }

        public void write(BinaryWriter bw, bool isLast)
        {
            bw.Write(endOffset);
            bw.Write(elemProps.Count);
            bw.Write(propsLength);

            bw.Write(id);

            for (int i = 0; i < elemProps.Count; i++) { elemProps[i].Write(bw); }

            write_children(bw, isLast);

            if (bw.BaseStream.Position != endOffset) { throw new DataMisalignedException(); }
        }

        protected void write_children(BinaryWriter bw, bool isLast)
        {
            if (this.elemSubtree.Count > 0)
            {
                for (int i = 0; i < this.elemSubtree.Count; i++)
                {
                    this.elemSubtree[i].write(bw, (i + 1 == this.elemSubtree.Count));
                }

                bw.Write(new byte[FBX.BLOCK_SENTINEL_LENGTH]);
            }
            else if (this.elemProps.Count == 0 && !isLast)
            {
                bw.Write(new byte[FBX.BLOCK_SENTINEL_LENGTH]);
            }
        }

        public int calc_offsets(int offset, bool isLast)
        {
            offset += 12;
            offset += 1 + id.Length;

            propsLength = 0;
            for (int i = 0; i < elemProps.Count; i++)
            {
                propsLength += 1 + elemProps[i].Size;
            }
            offset += propsLength;

            offset = calc_offsets_children(offset, isLast);

            endOffset = offset;
            return offset;
        }

        protected int calc_offsets_children(int offset, bool isLast)
        {
            if (this.elemSubtree.Count > 0)
            {
                for (int i = 0; i < this.elemSubtree.Count; i++)
                {
                    offset = this.elemSubtree[i].calc_offsets(offset, (i + 1 == this.elemSubtree.Count));
                }

                offset += FBX.BLOCK_SENTINEL_LENGTH;
            }
            else if (this.elemProps.Count == 0 && !isLast)
            {
                offset += FBX.BLOCK_SENTINEL_LENGTH;
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
            get { return propertyType; }
            set { propertyType = value; }
        }

        public object Value
        {
            get { return propertyValue; }
            set { propertyValue = value; }
        }

        public bool Compressed
        {
            get { return compressed; }
            set { compressed = value; }
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
                        return 4 + ((string)propertyValue).Length;

                    case 98: // bool array
                        if (rawData == null)
                        {
                            rawData = new byte[((bool[])propertyValue).Length * sizeof(bool)];
                            Buffer.BlockCopy((bool[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = Compress(rawData); }
                        }

                        return 12 + rawData.Length;

                    case 100: // Double array
                        if (rawData == null)
                        {
                            rawData = new byte[((double[])propertyValue).Length * sizeof(double)];
                            Buffer.BlockCopy((double[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = Compress(rawData); }
                        }

                        return 12 + rawData.Length;

                    case 102: // float array
                        if (rawData == null)
                        {
                            rawData = new byte[((float[])propertyValue).Length * sizeof(float)];
                            Buffer.BlockCopy((float[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = Compress(rawData); }
                        }

                        return 12 + rawData.Length;

                    case 105: // int array
                        if (rawData == null)
                        {
                            rawData = new byte[((int[])propertyValue).Length * sizeof(int)];
                            Buffer.BlockCopy((int[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = Compress(rawData); }
                        }

                        return 12 + rawData.Length;

                    case 108: // long array
                        if (rawData == null)
                        {
                            rawData = new byte[((long[])propertyValue).Length * sizeof(long)];
                            Buffer.BlockCopy((long[])propertyValue, 0, rawData, 0, rawData.Length);
                            if (compressed) { rawData = Compress(rawData); }
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
                    var b = (byte[])propertyValue;
                    bw.Write(b.Length);
                    bw.Write(b);
                    break;

                case 83: // String
                    var s = (string)propertyValue;
                    bw.Write(s.Length);
                    bw.WritePropertyString(s);
                    break;

                case 98: // bool array
                    var bo = (bool[])propertyValue;
                    bw.Write(bo.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                case 100: // Double array
                    var d = (double[])propertyValue;
                    bw.Write(d.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                case 102: // float array
                    var f = (float[])propertyValue;
                    bw.Write(f.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                case 105: // int array
                    var i = (int[])propertyValue;
                    bw.Write(i.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                case 108: // long array
                    var l = (long[])propertyValue;
                    bw.Write(l.Length);
                    bw.Write((compressed ? 1 : 0));
                    bw.Write(rawData.Length);
                    bw.Write(rawData);
                    break;

                default:
                    throw new NotImplementedException(string.Format("Unable to write property type {0}", propertyType));
            }
        }

        private static byte[] Compress(byte[] input)
        {
            using (var compressStream = new MemoryStream())
            using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress))
            {
                compressor.Write(input, 0, input.Length);
                compressor.Flush();
                compressor.Close();

                var data = compressStream.ToArray();
                byte[] compressed = new byte[2 + data.Length + 4];
                compressed[0] = 0x58;
                compressed[1] = 0x85;
                data.CopyTo(compressed, 2);
                BitConverter.GetBytes(ReverseBytes(AdlerChecksum.Generate(ref input, 0, input.Length))).CopyTo(compressed, compressed.Length - 4);

                return compressed;
            }
        }

        public static UInt32 ReverseBytes(UInt32 value)
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
            if (buffer == null || length - index <= 0)
                return 0;

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
