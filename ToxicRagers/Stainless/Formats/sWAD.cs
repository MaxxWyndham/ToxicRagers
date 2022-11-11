using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

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
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid WAD file", name);
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
	                int nameIndex = br.ReadInt32();
	                int size = br.ReadInt32();
	                int offsetIndex = (int)(br.ReadUInt32() & 0x00FFFFFF);
					WADEntry_Bytes entry = new WADEntry_Bytes
                    {
                        Name = names[nameIndex],
                        Size = size,
                        ParentEntry = parent,
                        Offset = offsets[offsetIndex]
                    };
                    var currentPos = br.BaseStream.Position;
                    entry.Data = wad.Extract(entry, br);
                    br.BaseStream.Seek(currentPos, SeekOrigin.Begin);
                    br.ReadInt32(); // Unknown

                    wad.Contents.Add(entry);
                    parent.ChildEntries.Add(entry);
                }

                void processDirectoryEntry(WADEntry parent = null)
                {
	                int nameIndex = br.ReadInt32();

					WADEntry entry = new WADEntry
                    {
                        Name = names[nameIndex],
                        IsDirectory = true,
                        ParentEntry = parent
                    };

                    wad.Contents.Add(entry);
                    parent?.ChildEntries.Add(entry);

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

        public void Save(string path)
        {
	        string folder = Path.GetDirectoryName(path);
	        if (!Directory.Exists(folder))
	        {
		        Directory.CreateDirectory(folder);
	        }
	        //FileInfo fi = new FileInfo(path);
			//Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
			using (Stream stream = new FileStream(path, FileMode.Create))
			{
				Save(stream, Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path));
			} 
		}

        public void Save(Stream stream, string wadName, string location)
        {
	        using (BinaryWriter writer = new BinaryWriter(stream))
	        {
		        writer.Write((byte)0x34);
		        writer.Write((byte)0x12);
			    writer.Write((byte)Version.Minor);
			    writer.Write((byte)Version.Major);
                writer.Write((int)Flags);
                writer.Write((int)0);

                List<string> names = new List<string>();
                List<int> namePositions = new List<int>();
                List<int> offsets = new List<int>();    // Offset into Data list, needs to be adjusted after size of entries, names, offsets is known!
                List<byte> entries = new List<byte>();
                List<byte> data = new List<byte>();
                int namePos = 0;
                int currentOffset = 0;
                int currentOffsetIndex = 0;
                WADEntry rootEntry = Contents.First(e => e.ParentEntry == null);

                void processFile(WADEntry entry)
                {
	                byte[] entryData = entry.GetData();
                    entries.AddRange(BitConverter.GetBytes(entryData.Length));
	                entries.AddRange(BitConverter.GetBytes(offsets.Count | 0x01000000));
	                entries.AddRange(new byte[] {0,0,0,0});

                    offsets.Add(data.Count);
                    data.AddRange(entryData);
                }
				void processFolder(WADEntry entry)
                {
                    var childFiles = entry.ChildEntries.Where(e => e.IsDirectory == false);
                    var childFolders = entry.ChildEntries.Where(e => e.IsDirectory == true);
                    entries.AddRange(BitConverter.GetBytes(childFiles.Count()));
                    entries.AddRange(BitConverter.GetBytes(childFolders.Count()));
                    entries.AddRange(new byte[] {0,0,0,0});
                    foreach (var child in childFolders)
                    {
                        processEntry(child);
                    }
                    foreach (var child in childFiles)
                    {
                        processEntry(child);
                    }
                }
                void processEntry(WADEntry entry)
                {
	                int currentPos = namePos;
	                if (names.Any(n => n == entry.Name))
	                {
		                currentPos = namePositions[names.FindIndex(n => n == entry.Name)];
	                }
	                else
	                {
		                names.Add(entry.Name);
		                namePositions.Add(currentPos);
		                namePos += entry.Name.Length + 1;
					}

	                entries.AddRange(BitConverter.GetBytes(currentPos));
					if (entry.IsDirectory)
                    {
                        processFolder(entry);
                    }
                    else
                    {
                        processFile(entry);
                    }

                }

                processEntry(rootEntry);
                int nameBlockStart = (int)writer.BaseStream.Position;
                int nameBlockLength = namePos + (16 - (namePos % 16));
                int nameBlockEnd = (int)writer.BaseStream.Position + nameBlockLength;

                writer.Write(nameBlockLength);
                foreach (string name in names)
                {
                    writer.WriteNullTerminatedString(name);
                }

                writer.Write(new byte[+(16 - (namePos % 16) )]);

                if (Flags.HasFlag(WADFlags.HasDataTimes))
                {
	                writer.Write(0);
                }

                writer.Write(Contents.Count(e => e.IsDirectory == false));
                writer.Write(Contents.Count(e => e.IsDirectory == true));
                writer.Write(offsets.Count);
                int offsetStart = offsets.Count * 4 + entries.Count + (int)writer.BaseStream.Position;
                
                foreach (var offset in offsets)
                {
	                currentOffset = offset;
                    writer.Write(offsetStart + currentOffset);
                }

                writer.Write(entries.ToArray());
                writer.Write(data.ToArray());
			}
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

                int length = Flags.HasFlag(WADFlags.HasCompressedFiles) ? br.ReadInt32() : -1;

				if (length == -1)
                {
                    bw.Write(br.ReadBytes(file.Size));
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
        public byte[] Extract(WADEntry file, BinaryReader br)
        {
            
                br.BaseStream.Seek(file.Offset, SeekOrigin.Begin);
				
                int length = Flags.HasFlag(WADFlags.HasCompressedFiles) ? br.ReadInt32() : -1;

                if (length == -1)
                {
                     return br.ReadBytes(file.Size - (Flags.HasFlag(WADFlags.HasCompressedFiles) ? 4 : 0));
                }
                else
                {
                    br.BaseStream.Seek(2, SeekOrigin.Current);

                    using (MemoryStream ms = new MemoryStream(br.ReadBytes(file.Size - 2)))
                    using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        byte[] data = new byte[length];
                        ds.Read(data, 0, length);
                        return data;
                    }
                }
            
        }
    }

    public class WADEntry
    {
        public WADEntry ParentEntry { get; set; }
        public List<WADEntry> ChildEntries { get; set; } = new List<WADEntry>();

        public bool IsDirectory { get; set; }

        public int Offset { get; set; }

        public int Size { get; set; }

        public string Name { get; set; }

        public string FullPath => Path.Combine(ParentEntry?.FullPath ?? "", Name);
        public virtual byte[] GetData()
        {
	        throw new NotImplementedException();
        }
	}

    public class WADEntry_File : WADEntry
	{
	    public string Path { get; set; }

	    public override byte[] GetData()
	    {
		    return File.ReadAllBytes(Path);
	    }
    }

    public class WADEntry_Bytes : WADEntry
    {
	    public byte[] Data { get; set; }

	    public override byte[] GetData()
	    {
		    return Data;
	    }
    }
}