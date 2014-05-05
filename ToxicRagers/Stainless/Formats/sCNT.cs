using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class CNT
    {
        CNT parent;
        string name;
        string nodeName;
        string modelName;
        string section;
        Matrix3D transform;
        List<CNT> childNodes = new List<CNT>();
        Version version;
        CNTLight light;

        public string Name { 
            get { return name; }
            set { name = value; }
        }

        public Matrix3D Transform {
            get { return transform; } 
            set { transform = value; } 
        }

        public string Section {
            get { return section; } 
            set { section = value; } 
        }

        public string Model {
            get { return modelName; } 
            set { modelName = value; } 
        }

        public CNTLight Light { get { return light; } }

        public CNT Parent { get { return parent; } }
        public List<CNT> Children { get { return childNodes; } }

        public static CNT Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            CNT cnt;

            using (BinaryReader br = new BinaryReader(fi.OpenRead(), Encoding.Default))
            {
                if (br.ReadByte() != 69 ||
                    br.ReadByte() != 35)
                {
                    Logger.LogToFile("{0} isn't a valid CNT file", Path);
                    return null;
                }

                byte minor = br.ReadByte();
                byte major = br.ReadByte();

                Logger.LogToFile("CNT v{0}.{1}", major, minor);

                cnt = Load(br, new Version(major, minor));

                Logger.LogToFile("{0} :: {1}", br.BaseStream.Position.ToString("X"), br.BaseStream.Length.ToString("X"));
                if (br.BaseStream.Position != br.BaseStream.Length) { Logger.LogToFile("Still has data remaining"); }
            }

            return cnt;
        }

        // The Load(BinaryReader) version skips the header check and is used for recursive loading
        private static CNT Load(BinaryReader br, Version version, CNT parent = null)
        {
            CNT cnt = new CNT();
            int nameLength, padding;
            if (parent != null) { cnt.parent = parent; }

            cnt.version = version;

            Logger.LogToFile("{0}", br.BaseStream.Position.ToString("X"));

            if (version.Major == 3)
            {
                cnt.name = br.ReadBytes(16).ToName();

                Logger.LogToFile("Name: \"{0}\"", cnt.Name);
            }
            else
            {
                nameLength = (int)br.ReadUInt32();
                padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                cnt.name = br.ReadString(nameLength);
                br.ReadBytes(padding);

                Logger.LogToFile("Name: \"{0}\" of length {1}, padding of {2}", cnt.Name, nameLength, padding);
            }

            byte flags = br.ReadByte();
            if (flags != 0)
            {
                Logger.LogToFile("Flags: {0}", flags);
                br.ReadByte();
            }

            Logger.LogToFile("This is usually 0: {0}", br.ReadSingle());

            cnt.transform = new Matrix3D(
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                            );

            cnt.section = br.ReadString(4);
            switch (cnt.section)
            {
                case "LITd":
                    Logger.LogToFile("LITd, skipping 16 bytes");
                    br.ReadBytes(16);
                    break;

                case "LITg":
                    cnt.light = new CNTLight((int)br.ReadUInt32());

                    switch (cnt.light.LightType)
                    {
                        case 2: // Bounding box?
                            Logger.LogToFile("Light type: {0}", cnt.light.LightType);

                            for (int i = 0; i < 26; i++)
                            {
                                Logger.LogToFile("{0}] {1}", i, br.ReadSingle());
                            }

                            break;

                        case 3:
                            Logger.LogToFile("Light type: {0}", cnt.light.LightType);
                            break;

                        default:
                            Logger.LogToFile("Unknown light type!  I've never seen a light like {0} before", cnt.light.LightType);
                            break;
                    }

                    nameLength = (int)br.ReadUInt32();
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                    cnt.light.Name = br.ReadString(nameLength);
                    br.ReadBytes(padding);

                    Logger.LogToFile("LITg: \"{0}\" of length {1}, padding of {2}", cnt.light.Name, nameLength, padding);
                    break;

                case "EMIT":    // <= v4.0
                    int emitVersion = br.ReadByte();
                    int toSkip = (emitVersion == 6 ? 128 : 136);

                    br.ReadBytes(25);
                    Logger.LogToFile("EMIT v{0}, skipping 26 bytes, reading a name (\"{1}\") and then skipping {2} bytes", emitVersion, br.ReadString((int)br.ReadUInt32()), toSkip);
                    br.ReadBytes(toSkip);
                    break;

                case "EMT2":
                    Logger.LogToFile("EMT2, skipping 650 bytes");
                    br.ReadBytes(650);
                    break;

                case "MODL":
                case "SKIN":
                    if (version.Major == 3)
                    {
                        cnt.modelName = br.ReadBytes(16).ToName();

                        Logger.LogToFile("MODL: \"{0}\"", cnt.Name);
                        br.ReadBytes(16);
                    }
                    else
                    {
                        nameLength = (int)br.ReadUInt32();
                        padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                        cnt.modelName = br.ReadString(nameLength);
                        br.ReadBytes(padding);

                        Logger.LogToFile("{0}: \"{1}\" of length {2}, padding of {3}", cnt.section, cnt.modelName, nameLength, padding);
                    }

                    break;

                case "VFXI":
                    nameLength = (int)br.ReadUInt32();

                    string effectName = br.ReadString(nameLength);

                    Logger.LogToFile("VXFI: \"{0}\" of length {1}, padding of {2}", effectName, nameLength, 0);
                    break;

                case "NULL":
                    break;

                default:
                    Logger.LogToFile("Unknown section \"{0}\"; Aborting", cnt.section);
                    return null;
            }

            int childNodes = (int)br.ReadUInt32();

            for (int i = 0; i < childNodes; i++)
            {
                Logger.LogToFile("Loading child {0} of {1}", (i + 1), childNodes);
                cnt.childNodes.Add(Load(br, version, cnt));
            }

            br.ReadUInt32();    // Terminator

            return cnt;
        }

        public void Save(string Path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path, FileMode.Create), Encoding.Default))
            {
                bw.Write(new byte[] { 69, 35, 0, 4 });

                Save(bw, this);
            }
        }

        private static void Save(BinaryWriter bw, CNT cnt)
        {
            int nameLength = cnt.Name.Length;
            int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

            bw.Write(nameLength);
            bw.WriteString(cnt.Name);
            bw.Write(new byte[padding]);

            bw.Write((byte)0);

            bw.Write((int)0);

            bw.Write(cnt.Transform.M11);
            bw.Write(cnt.Transform.M12);
            bw.Write(cnt.Transform.M13);
            bw.Write(cnt.Transform.M21);
            bw.Write(cnt.Transform.M22);
            bw.Write(cnt.Transform.M23);
            bw.Write(cnt.Transform.M31);
            bw.Write(cnt.Transform.M32);
            bw.Write(cnt.Transform.M33);
            bw.Write(cnt.Transform.M41);
            bw.Write(cnt.Transform.M42);
            bw.Write(cnt.Transform.M43);

            bw.WriteString(cnt.Section);

            switch (cnt.Section)
            {
                case "LITg":
                    bw.Write(cnt.Light.LightType);

                    switch (cnt.light.LightType)
                    {
                        case 2:
                            throw new NotImplementedException("Save code for LightType 2 does not exist!");
                    }

                    nameLength = cnt.Light.Name.Length;
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                    bw.Write(nameLength);
                    bw.WriteString(cnt.Light.Name);
                    bw.Write(new byte[padding]);
                    break;

                case "MODL":
                case "SKIN":
                    nameLength = cnt.Model.Length;
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                    bw.Write(nameLength);
                    bw.WriteString(cnt.Model);
                    bw.Write(new byte[padding]);
                    break;

                case "NULL":
                    break;

                default:
                    throw new NotImplementedException(string.Format("Save code for CNT section {0} does not exist!", cnt.Section));
            }

            bw.Write(cnt.Children.Count);
            foreach (CNT c in cnt.Children) { Save(bw, c); }
            bw.Write((int)0);
        }

        // This seems wrong.  I am very tired.
        public CNT FindByName(string name)
        {
            CNT match = null;

            if (this.name == name)
            {
                match = this;
            }
            else
            {
                foreach (CNT c in this.childNodes)
                {
                    match = c.FindByName(name);
                    if (match != null) { break; }
                }
            }

            return match;
        }
    }

    public class CNTLight
    {
        string name;
        int type;

        public int LightType { get { return type; } }
        public string Name { get { return name; } set { name = value; } }

        public CNTLight(int lightType)
        {
            type = lightType;
        }
    }
}
