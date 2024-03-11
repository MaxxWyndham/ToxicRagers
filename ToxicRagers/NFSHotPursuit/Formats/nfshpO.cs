using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.NFSHotPursuit.Formats
{
    public class O
    {
        public string Name { get; set; }
        public string Location { get; set; }

        public static O Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            O o = new O()
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Location = Path.GetDirectoryName(path) + "\\"
            };

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 0x7f ||
                    br.ReadByte() != 0x45 || // E
                    br.ReadByte() != 0x4c || // L
                    br.ReadByte() != 0x46)   // F
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid O file", path);
                    return null;
                }

                ELF elf = new ELF();

                elf.Header.Ident.Mode = (ELFIdent.BitMode)br.ReadByte();
                elf.Header.Ident.Endian = (ELFIdent.Endianness)br.ReadByte();
                elf.Header.Ident.Version = br.ReadByte();
                elf.Header.Ident.OSABI = (ELFIdent.OperatingSystemABI)br.ReadByte();
                elf.Header.Ident.ABIVersion = br.ReadByte();
                br.ReadBytes(7); // padding

                elf.Header.FileType = (ELFHeader.ObjectType)br.ReadUInt16();
                elf.Header.Machine = (ELFHeader.InstructionArchitecture)br.ReadUInt16();
                elf.Header.Version = (int)br.ReadUInt32();
                elf.Header.EntryPoint = (int)br.ReadUInt32();
                elf.Header.ProgramHeaderOffset = (int)br.ReadUInt32();
                elf.Header.SectionHeaderOffset = (int)br.ReadUInt32();
                elf.Header.Flags = (int)br.ReadUInt32();
                elf.Header.HeaderSize = (short)br.ReadUInt16();
                elf.Header.ProgramHeaderSize = (short)br.ReadUInt16();
                elf.Header.ProgramHeaderCount = (short)br.ReadUInt16();
                elf.Header.SectionHeaderSize = (short)br.ReadUInt16();
                elf.Header.SectionHeaderCount = (short)br.ReadUInt16();
                elf.Header.SectionHeaderNamesIndex = (short)br.ReadUInt16();

                if (elf.Header.ProgramHeaderSize > 0)
                {
                    // do a thing
                }

                if (elf.Header.SectionHeaderSize > 0)
                {
                    br.BaseStream.Seek(elf.Header.SectionHeaderOffset, SeekOrigin.Begin);

                    for (int i = 0; i < elf.Header.SectionHeaderCount; i++)
                    {
                        ELFSection section = new ELFSection
                        {
                            NameIndex = (int)br.ReadUInt32(),
                            Type = (ELFSection.SectionType)br.ReadUInt32(),
                            Flags = (ELFSection.SectionFlags)br.ReadUInt32(),
                            VirtualAddress = (int)br.ReadUInt32(),
                            Offset = (int)br.ReadUInt32(),
                            Size = (int)br.ReadUInt32(),
                            Link = (int)br.ReadUInt32(),
                            Info = (int)br.ReadUInt32(),
                            AddressAlign = (int)br.ReadUInt32(),
                            EntrySize = (int)br.ReadUInt32()
                        };

                        elf.Sections.Add(section);
                    }

                    foreach (ELFSection section in elf.Sections)
                    {
                        br.BaseStream.Seek(section.Offset, SeekOrigin.Begin);

                        section.Data = br.ReadBytes(section.Size);
                    }
                }

                elf.Process();
            }

            return o;
        }
    }

    public class ELFIdent
    {
        public enum BitMode
        {
            [Description("32bit")]
            ThirtyTwo = 1,
            [Description("64bit")]
            SixtyFour = 2
        }

        public enum Endianness
        {
            Little = 1,
            Big = 2
        }

        public enum OperatingSystemABI
        {
            SystemV = 0x0
        }

        public BitMode Mode { get; set; }
        public Endianness Endian { get; set; }
        public byte Version { get; set; }
        public OperatingSystemABI OSABI { get; set; }
        public byte ABIVersion { get; set; }
    }

    public class ELFHeader
    {
        public enum ObjectType
        {
            None = 0x0,
            Rel = 0x1,
            Exec = 0x2,
            Dyn = 0x3,
            Core = 0x4,
            LoOS = 0xfe00,
            HiOS = 0xfeff,
            LoProc = 0xff00,
            HiProc = 0xffff
        }

        public enum InstructionArchitecture
        {
            NoSpecificInstructionSet = 0x0,
            SPARC = 0x2,
            x86 = 0x3,
            MIPS = 0x8,
            PowerPC = 0x14,
            S390 = 0x16,
            ARM = 0x28,
            SuperH = 0x2a,
            IA64 = 0x32,
            amd64 = 0x3e,
            AArch64 = 0xb7,
            RISCV = 0xf3

        }

        public ELFIdent Ident { get; set; } = new ELFIdent();
        public ObjectType FileType { get; set; }
        public InstructionArchitecture Machine { get; set; }
        public int Version { get; set; }
        public int EntryPoint { get; set; }
        public int ProgramHeaderOffset { get; set; }
        public int SectionHeaderOffset { get; set; }
        public int Flags { get; set; }
        public short HeaderSize { get; set; }
        public short ProgramHeaderSize { get; set; }
        public short ProgramHeaderCount { get; set; }
        public short SectionHeaderSize { get; set; }
        public short SectionHeaderCount { get; set; }
        public short SectionHeaderNamesIndex { get; set; }
    }

    public class ELFSymbol
    {
        public enum InfoType
        {
            NoType = 0,
            Object = 1,
            Func = 2,
            Section = 3
        }

        public enum InfoBind
        {
            Local = 0,
            Global = 1,
            Weak = 2
        }

        public int NameIndex { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public int Size { get; set; }
        public InfoType Type { get; set; }
        public InfoBind Binding { get; set; }
        public byte Other { get; set; }
        public short Shndx { get; set; }
    }

    public class ELFRelocation
    {
        public enum InfoType
        {
            R_MIPS_32 = 2,
            R_MIPS_26 = 4,
            R_MIPS_HI16 = 5,
            R_MIPS_LO16 = 6
        }

        public int Offset { get; set; }
        public InfoType RelocationType { get; set; }
        public int SymbolIndex { get; set; }
    }

    public class ELFSection
    {
        public enum SectionType
        {
            NULL = 0x0,
            ProgBits = 0x1,
            SymTab = 0x2,
            StrTab = 0x3,
            Rela = 0x4,
            Hash = 0x5,
            Dynamic = 0x6,
            Note = 0x7,
            NoBits = 0x8,
            Rel = 0x9,
            ShLib = 0xa,
            DynSym = 0xb,
            InitArray = 0xe,
            FiniArray = 0xf,
            PreIntArray = 0x10,
            Group = 0x11,
            SymTabShndx = 0x12,
            Num = 0x13
        }

        [Flags]
        public enum SectionFlags : uint
        {
            Write = 0x1,
            Alloc = 0x2,
            ExecInstr = 0x4,
            Merge = 0x10,
            Strings = 0x20,
            InfoLink = 0x40,
            LinkOrder = 0x80,
            OSNonConforming = 0x100,
            Group = 0x200,
            TLS = 0x400,
            MaskOS = 0xff00000,
            MaskProc = 0xf0000000,
            Ordered = 0x4000000,
            Exclude = 0x8000000
        }

        public int NameIndex { get; set; }
        public string Name { get; set; }
        public SectionType Type { get; set; }
        public SectionFlags Flags { get; set; }
        public int VirtualAddress { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public int Link { get; set; }
        public int Info { get; set; }
        public int AddressAlign { get; set; }
        public int EntrySize { get; set; }

        public byte[] Data { get; set; }

        public List<ELFSymbol> Symbols { get; set; } = new List<ELFSymbol>();
        public List<ELFRelocation> Relocations { get; set; } = new List<ELFRelocation>();
    }

    public class ELF
    {
        public ELFHeader Header { get; set; } = new ELFHeader();
        public List<ELFSection> Sections { get; set; } = new List<ELFSection>();

        public void Process()
        {
            using (MemoryStream ms = new MemoryStream(Sections[Header.SectionHeaderNamesIndex].Data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                foreach (ELFSection section in Sections)
                {
                    br.BaseStream.Seek(section.NameIndex, SeekOrigin.Begin);

                    section.Name = br.ReadNullTerminatedString();
                }
            }

            foreach (ELFSection section in Sections)
            {
                switch (section.Type)
                {
                    case ELFSection.SectionType.SymTab:
                        using (MemoryStream ms = new MemoryStream(section.Data))
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            while (br.BaseStream.Position < br.BaseStream.Length)
                            {
                                ELFSymbol symbol = new ELFSymbol
                                {
                                    NameIndex = (int)br.ReadUInt32(),
                                    Value = (int)br.ReadUInt32(),
                                    Size = (int)br.ReadUInt32()
                                };

                                byte infoTypeBind = br.ReadByte();

                                symbol.Type = (ELFSymbol.InfoType)(infoTypeBind & 0xf);
                                symbol.Binding = (ELFSymbol.InfoBind)((infoTypeBind & 0xf0) >> 4);

                                symbol.Other = br.ReadByte();
                                symbol.Shndx = (short)br.ReadUInt16();

                                section.Symbols.Add(symbol);
                            }
                        }

                        using (MemoryStream ms = new MemoryStream(Sections[section.Link].Data))
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            foreach (ELFSymbol symbol in section.Symbols)
                            {
                                br.BaseStream.Seek(symbol.NameIndex, SeekOrigin.Begin);

                                symbol.Name = br.ReadNullTerminatedString();
                            }
                        }

                        foreach (ELFSymbol symbol in section.Symbols)
                        {
                            if (symbol.Value == 0) { continue; }

                            using (MemoryStream ms = new MemoryStream(Sections[symbol.Shndx].Data))
                            using (BinaryReader br = new BinaryReader(ms))
                            {
                                br.BaseStream.Seek(symbol.Value, SeekOrigin.Begin);

                                string[] parts = symbol.Name.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                                string[] subparts = parts[0].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

                                string objectType = (subparts.Length == 2 ? subparts[1] : subparts[0].Substring(2)).Split('_')[0];

                                switch (objectType)
                                {
                                    case "RenderMethod":
                                        _ = new RenderMethod
                                        {
                                            CodeBlock = br.ReadUInt32(),
                                            UsedCodeBlock = br.ReadUInt32(),
                                            EAGLMicroCode = br.ReadUInt32(),
                                            Effect = br.ReadUInt32(),
                                            ParentRenderMethod = br.ReadUInt32(),
                                            ParameterNames = br.ReadUInt32(),
                                            Unknown2 = br.ReadUInt32(),
                                            Unknown3 = br.ReadInt32(),
                                            Unknown4 = br.ReadUInt32(),
                                            GeometryDataBuffer = br.ReadUInt32(),
                                            IndexOfCommand = br.ReadInt32(),
                                            EffectName = br.ReadUInt32(),
                                            EAGLModelObject = br.ReadUInt32()
                                        };
                                        break;

                                    case "TAR":
                                        (new TextureSampler()).Read(br);
                                        break;

                                    case "Model":
                                        Model model = new Model
                                        {
                                            VariablesOffset = br.ReadUInt32(),
                                            VariableCount = br.ReadUInt32(),
                                            TotalInstanceCount = br.ReadUInt32(),
                                            Transform = new Matrix4D(
                                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                            ),
                                            BoundingCorner1 = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                            BoundingCorner2 = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                            BoundingMin = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                            BoundingMax = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                            Centre = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                            GroupsCount = br.ReadUInt32(),
                                            GroupNamesOffset = br.ReadUInt32(),
                                            Unknown3 = br.ReadUInt32(),
                                            Unknown4 = br.ReadUInt32(),
                                            Unknown5 = br.ReadUInt32(),
                                            NextModel = br.ReadUInt32(),
                                            ModelNameOffset = br.ReadUInt32(),
                                            InterleavedVertices = br.ReadUInt32(),
                                            TexturesOffset = br.ReadUInt32(),
                                            InstanceIndex = br.ReadUInt32(),
                                            GroupStates = br.ReadUInt32(),
                                            IsRenderable = br.ReadUInt32(),
                                            GroupsOffset = br.ReadUInt32(),
                                            Unknown6 = br.ReadUInt32(),
                                            Unknown7 = br.ReadUInt32(),
                                            SkeletonVersion = br.ReadUInt32(),
                                            LastFrame = br.ReadUInt32()
                                        };

                                        br.BaseStream.Seek(model.ModelNameOffset, SeekOrigin.Begin);
                                        model.ModelName = br.ReadNullTerminatedString();

                                        br.BaseStream.Seek(model.VariablesOffset, SeekOrigin.Begin);

                                        for (int i = 0; i < model.VariableCount; i++)
                                        {
                                            Variable variable = new Variable
                                            {
                                                NameOffset = br.ReadUInt32(),
                                                EntrySize = br.ReadUInt16(),
                                                Unknown1 = br.ReadUInt16(),
                                                EntriesCount = br.ReadUInt32(),
                                                EntriesOffset = br.ReadUInt32()
                                            };

                                            model.Variables.Add(variable);
                                        }

                                        for (int i = 0; i < model.VariableCount; i++)
                                        {
                                            br.BaseStream.Seek(model.Variables[i].NameOffset, SeekOrigin.Begin);
                                            model.Variables[i].Name = br.ReadNullTerminatedString();
                                        }

                                        br.BaseStream.Seek(model.TexturesOffset, SeekOrigin.Begin);
                                        uint textureNames = br.ReadUInt32();

                                        if (textureNames != 0)
                                        {
                                            uint textureCount = br.ReadUInt32();

                                            for (int i = 0; i < textureCount; i++)
                                            {
                                                uint textureOffset = br.ReadUInt32();
                                                long lastPosition = br.BaseStream.Position;

                                                br.BaseStream.Seek(textureOffset, SeekOrigin.Begin);

                                                (new TextureSampler()).Read(br);

                                                br.BaseStream.Seek(lastPosition, SeekOrigin.Begin);
                                            }
                                        }

                                        br.BaseStream.Seek(model.GroupsOffset, SeekOrigin.Begin);

                                        for (int i = 0; i < model.GroupsCount; i++)
                                        {
                                            Group group = new Group { ID = br.ReadUInt32() };

                                            switch (group.ID)
                                            {
                                                case 0xa0000000:
                                                    group.Unknown1 = br.ReadUInt32();
                                                    group.GroupMeshDescriptorSize = br.ReadUInt32();
                                                    break;

                                                case 0xa0000001:
                                                    group.Unknown1 = br.ReadUInt32();
                                                    group.MeshDescriptorSize = br.ReadUInt32();
                                                    break;

                                                case 0xa000ffff:
                                                    group.MeshOffset = br.ReadUInt32();
                                                    break;

                                                default:
                                                    break;
                                            }

                                            model.Groups.Add(group);
                                        }
                                        break;

                                    case "BBOX":
                                        _ = new BoundingBox
                                        {
                                            Min = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                            Max = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                                        };
                                        break;

                                    case "geoprimdatabuffer":
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                        break;

                    case ELFSection.SectionType.Rel:
                        using (MemoryStream ms = new MemoryStream(section.Data))
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            while (br.BaseStream.Position < br.BaseStream.Length)
                            {
                                ELFRelocation rel = new ELFRelocation
                                {
                                    Offset = (int)br.ReadUInt32(),
                                    RelocationType = (ELFRelocation.InfoType)br.ReadByte()
                                };

                                rel.SymbolIndex = (br.ReadByte() << 0 | br.ReadByte() << 8 | br.ReadByte() << 16);

                                section.Relocations.Add(rel);
                            }
                        }
                        break;
                }
            }
        }
    }

    public class Model
    {
        public uint VariablesOffset { get; set; }
        public List<Variable> Variables { get; set; } = new List<Variable>();
        public uint VariableCount { get; set; }
        public uint TotalInstanceCount { get; set; }
        public Matrix4D Transform { get; set; }
        public Vector4 BoundingCorner1 { get; set; }
        public Vector4 BoundingCorner2 { get; set; }
        public Vector4 BoundingMin { get; set; }
        public Vector4 BoundingMax { get; set; }
        public Vector4 Centre { get; set; }
        public uint GroupsCount { get; set; }
        public uint GroupNamesOffset { get; set; }
        public uint Unknown3 { get; set; }
        public uint Unknown4 { get; set; }
        public uint Unknown5 { get; set; }
        public uint NextModel { get; set; }
        public uint ModelNameOffset { get; set; }
        public string ModelName { get; set; }
        public uint InterleavedVertices { get; set; }
        public uint TexturesOffset { get; set; }
        public uint InstanceIndex { get; set; }
        public uint GroupStates { get; set; }
        public uint IsRenderable { get; set; }
        public uint GroupsOffset { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();
        public uint Unknown6 { get; set; }
        public uint Unknown7 { get; set; }
        public uint SkeletonVersion { get; set; }
        public uint LastFrame { get; set; }
    }

    public class Group
    {
        public uint ID { get; set; }
        public string GroupName { get; set; }
        public uint MeshOffset { get; set; }
        public uint Unknown1 { get; set; }
        public uint GroupMeshDescriptorSize { get; set; }
        public uint MeshDescriptorSize { get; set; }
    }

    public class Variable
    {
        public uint NameOffset { get; set; }
        public string Name { get; set; }
        public ushort EntrySize { get; set; }
        public ushort Unknown1 { get; set; }
        public uint EntriesCount { get; set; }
        public uint EntriesOffset { get; set; }
    }
    public class TextureSampler
    {
        public uint Unknown1 { get; set; }
        public string TextureTag { get; set; }
        public uint Unknown2 { get; set; }
        public float Unknown3 { get; set; }
        public uint Unknown4 { get; set; }
        public float Unknown5 { get; set; }
        public uint Unknown6 { get; set; }
        public uint UWrapMode { get; set; }
        public uint VWrapMode { get; set; }
        public uint WWrapMode { get; set; }
        public uint Unknown7 { get; set; }
        public uint Unknown8 { get; set; }

        public void Read(BinaryReader br)
        {
            Unknown1 = br.ReadUInt32();
            TextureTag = br.ReadString(4);
            Unknown2 = br.ReadUInt32();
            Unknown3 = br.ReadSingle();
            Unknown4 = br.ReadUInt32();
            Unknown5 = br.ReadSingle();
            Unknown6 = br.ReadUInt32();
            UWrapMode = br.ReadUInt32();
            VWrapMode = br.ReadUInt32();
            WWrapMode = br.ReadUInt32();
            Unknown7 = br.ReadUInt32();
            Unknown8 = br.ReadUInt32();
        }
    }

    public class RenderMethod
    {
        public uint CodeBlock { get; set; }
        public uint UsedCodeBlock { get; set; }
        public uint EAGLMicroCode { get; set; }
        public uint Effect { get; set; }
        public uint ParentRenderMethod { get; set; }
        public uint ParameterNames { get; set; }
        public uint Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public uint Unknown4 { get; set; }
        public uint GeometryDataBuffer { get; set; }
        public int IndexOfCommand { get; set; }
        public uint EffectName { get; set; }
        public uint EAGLModelObject { get; set; }
    }
}
