using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.Vigilante82ndOffense.Formats
{
    public enum EXPEntryType
    {
        Binary,
        Animation
    }

    public class EXP
    {
        string name;
        string location;
        int entryCount;
        List<EXPEntry> contents;

        public string Name => name;
        public List<EXPEntry> Contents => contents;

        public EXP()
        {
            contents = new List<EXPEntry>();
        }

        public static EXP Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            EXP exp = new EXP()
            {
                name = Path.GetFileNameWithoutExtension(path),
                location = Path.GetDirectoryName(path) + "\\"
            };

            using (BEBinaryReader br = new BEBinaryReader(fi.OpenRead()))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    string section = br.ReadString(4);
                    int length = 0;

                    switch (section)
                    {
                        case "FORM":
                            length = (int)br.ReadUInt32();
                            break;

                        case "TERR":
                        case "XOBF":
                            break;

                        case "TITL":
                            length = (int)br.ReadUInt32();
                            if (length > 0) { throw new NotImplementedException(string.Format("TITL section has a length ({0}), don't know what to do!", length)); }
                            break;

                        case "TEXT":
                            length = (int)br.ReadUInt32();
                            if (length > 0) { throw new NotImplementedException(string.Format("TEXT section has a length ({0}), don't know what to do!", length)); }
                            break;

                        case "HEAD":
                            length = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "U: {0}", br.ReadUInt16());
                            Logger.LogToFile(Logger.LogLevel.Debug, "U: {0}", br.ReadUInt32());
                            exp.entryCount = br.ReadUInt16();
                            Logger.LogToFile(Logger.LogLevel.Debug, "U: {0}", br.ReadUInt32());
                            Logger.LogToFile(Logger.LogLevel.Debug, "U: {0}", br.ReadUInt16());
                            Logger.LogToFile(Logger.LogLevel.Debug, "U: {0}", br.ReadUInt16());
                            Logger.LogToFile(Logger.LogLevel.Debug, "U: {0}", br.ReadUInt16());
                            Logger.LogToFile(Logger.LogLevel.Debug, "U: {0}", br.ReadUInt32());
                            Logger.LogToFile(Logger.LogLevel.Debug, "U: {0}", br.ReadUInt32());
                            break;

                        case "BIN ":
                            length = (int)br.ReadUInt32();
                            exp.contents.Add(new EXPEntry { Size = length, Offset = (int)br.BaseStream.Position, EntryType = EXPEntryType.Binary });
                            br.ReadBytes(length);
                            break;

                        case "ANM ":
                            length = (int)br.ReadUInt32();
                            exp.contents.Add(new EXPEntry { Size = length, Offset = (int)br.BaseStream.Position, EntryType = EXPEntryType.Animation });
                            br.ReadBytes(length);
                            break;

                        case "PLTX":
                            length = (int)br.ReadUInt32();
                            if (length > 0) { throw new NotImplementedException(string.Format("PLTX section has a length ({0}), don't know what to do!", length)); }
                            break;

                        default:
                            throw new NotImplementedException(string.Format("Unexpected section \"{0}\" at {1,0:X0}", section, br.BaseStream.Position - 4));
                    }
                }
            }

            return exp;
        }

        public void Extract(EXPEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(destination + "\\" + file.Filename, FileMode.Create)))
            using (FileStream fs = new FileStream(location + name + ".exp", FileMode.Open))
            {
                fs.Seek(file.Offset, SeekOrigin.Begin);

                byte[] buff = new byte[file.Size];
                fs.Read(buff, 0, file.Size);
                bw.Write(buff);
                buff = null;
            }
        }
    }

    public class EXPEntry
    {
        int offset;
        int size;
        EXPEntryType type;

        public int Offset
        {
            get => offset;
            set => offset = value;
        }

        public int Size
        {
            get => size;
            set => size = value;
        }

        public EXPEntryType EntryType
        {
            get => type;
            set => type = value;
        }

        public string Filename => string.Format("{0}.{1}", offset, type.ToString().Substring(0, 3).ToLower());
    }
}