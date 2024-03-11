using System.IO.Compression;

using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class WAD
    {
        [Flags]
        public enum WADFlags
        {
            HasCompressedFiles = 1 << 1,
            Unknown = 1 << 7,
            HasDataTimes = 1 << 9
        }

        public string Name { get; set; }

        public string Location { get; set; }

        public Version Version { get; set; }

        public WADFlags Flags { get; set; }

        public List<WADEntry> Contents { get; set; } = new List<WADEntry>();

        public IEnumerable<WADEntry> Files => Contents.Where(c => !c.IsDirectory);

        public static WAD Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            using (Stream stream = fi.OpenRead())
            {
                return Load(stream, Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path));
            }
        }

        public static WAD Load(Stream stream, string name, string location)
        {

            WAD wad = new WAD
            {
                Name = name,
                Location = location
            };

            using (BinaryReader br = new BinaryReader(stream))
            {
                if (br.ReadByte() != 0x34 ||
                    br.ReadByte() != 0x12)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, $"{name} isn't a valid WAD file");
                    return null;
                }

                byte minor = br.ReadByte();
                byte major = br.ReadByte();

                wad.Version = new Version(major, minor);
                wad.Flags = (WADFlags)br.ReadUInt32();

                int xmlLength = (int)br.ReadUInt32();
                if (xmlLength > 0)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "Unexpected data discovered.  Aborting");
                    return null;
                }

                int namesLength = (int)br.ReadUInt32();
                int nameBlockStart = (int)br.BaseStream.Position;
                int nameBlockEnd = (int)br.BaseStream.Position + namesLength;
                Dictionary<int, string> names = new Dictionary<int, string>();

                while (br.BaseStream.Position < nameBlockEnd && br.PeekChar() != 0)
                {
                    names.Add((int)(br.BaseStream.Position - nameBlockStart), br.ReadNullTerminatedString());
                }

                br.BaseStream.Seek(nameBlockEnd, SeekOrigin.Begin);

                if (wad.Flags.HasFlag(WADFlags.HasDataTimes))
                {
                    int count = br.ReadInt32();

                    for (int i = 0; i < count; i++)
                    {
                        br.ReadInt32();     // index (with bit 0x1 set)
                        br.ReadInt32();     // dos date time?  time added to archive?
                    }
                }

                int fileCount = br.ReadInt32();
                int folderCount = br.ReadInt32();

                int offsetCount = br.ReadInt32();
                List<int> offsets = new List<int>();
                for (int i = 0; i < offsetCount; i++)
                {
                    offsets.Add(br.ReadInt32());
                }

                void processFileEntry(WADEntry parent)
                {
                    WADEntry entry = new WADEntry
                    {
                        Name = names[br.ReadInt32()],
                        Size = br.ReadInt32(),
                        ParentEntry = parent,
                        Offset = offsets[(int)(br.ReadUInt32() & 0x00FFFFFF)]
                    };

                    br.ReadInt32(); // Unknown

                    wad.Contents.Add(entry);
                }

                void processDirectoryEntry(WADEntry parent = null)
                {
                    WADEntry entry = new WADEntry
                    {
                        Name = names[br.ReadInt32()],
                        IsDirectory = true,
                        ParentEntry = parent
                    };

                    wad.Contents.Add(entry);

                    int fileEntries = br.ReadInt32();
                    int folderEntries = br.ReadInt32();
                    br.ReadInt32(); // unknown

                    for (int i = 0; i < folderEntries; i++)
                    {
                        processDirectoryEntry(entry);
                    }

                    for (int i = 0; i < fileEntries; i++)
                    {
                        processFileEntry(entry);
                    }
                }

                processDirectoryEntry();
            }

            return wad;
        }

        public void Extract(WADEntry file, string destination, bool createFullPath = true)
        {
            if (createFullPath) { destination = Path.Combine(destination, Path.GetDirectoryName(file.FullPath)); }
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path.Combine(destination, file.Name), FileMode.Create)))
            using (FileStream fs = new FileStream(Path.Combine(Location, $"{Name}.wad"), FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(file.Offset, SeekOrigin.Begin);

                int length = br.ReadInt32();

                if (length == -1)
                {
                    bw.Write(br.ReadBytes(file.Size - 4));
                }
                else
                {
                    br.BaseStream.Seek(2, SeekOrigin.Current);

                    using (MemoryStream ms = new MemoryStream(br.ReadBytes(file.Size - 2)))
                    using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        byte[] data = new byte[length];
                        ds.Read(data, 0, length);
                        bw.Write(data, 0, data.Length);
                    }
                }
            }
        }
    }

    public class WADEntry
    {
        public WADEntry ParentEntry { get; set; }

        public bool IsDirectory { get; set; }

        public int Offset { get; set; }

        public int Size { get; set; }

        public string Name { get; set; }

        public string FullPath => Path.Combine(ParentEntry?.FullPath ?? "", Name);
    }
}