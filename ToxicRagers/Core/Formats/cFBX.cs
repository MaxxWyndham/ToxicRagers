using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using ToxicRagers.Helpers;

namespace ToxicRagers.Core.Formats
{
    public class FBX
    {
        const int BLOCK_SENTINEL_LENGTH = 13;

        int version;
        List<FBXElem> elements = new List<FBXElem>();

        public int Version { get { return version; } }
        public List<FBXElem> Elements { get { return elements; } }

        public static FBX Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            FBX fbx = new FBX();

            using (var br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadString(20) != "Kaydara FBX Binary  " || br.ReadByte() != 0 || br.ReadByte() != 26 || br.ReadByte() != 0) 
                {
                    Console.WriteLine("Invalid Binary FBX detected, error!");
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

            return fbx;
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

                case 73: // 32bit int
                    property.Value = (int)br.ReadUInt32();
                    break;

                case 76: // 64bit int
                    property.Value = (int)br.ReadUInt64();
                    break;

                case 82: // Byte array
                    property.Value = br.ReadBytes((int)br.ReadUInt32());
                    break;

                case 83: // String
                    property.Value = br.ReadString((int)br.ReadUInt32());
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

                default:
                    throw new NotSupportedException(string.Format("Unknown Data Type: {0}", dataType));
            }

            return property;
        }
    }

    public class FBXElem
    {
        string id;
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
    }

    public class FBXProperty
    {
        byte propertyType;
        object propertyValue;

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
    }
}
