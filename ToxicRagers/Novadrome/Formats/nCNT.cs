using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.Novadrome.Formats
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

        public string Name { get { return name; } }
        public string Model { get { return modelName; } }
        public string Section { get { return section; } }
        public Matrix3D Transform { get { return transform; } }

        public CNT Parent { get { return parent; } }
        public List<CNT> Children { get { return childNodes; } }

        public Matrix3D CombinedTransform
        {
            get
            {
                var m = transform;
                var cnt = this;

                while (cnt.parent != null)
                {
                    cnt = cnt.parent;

                    m *= cnt.transform;
                }

                return m;
            }
        }

        public static CNT Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            CNT cnt;

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 69 ||
                    br.ReadByte() != 35 ||
                    br.ReadByte() != 0 ||
                    br.ReadByte() != 4)
                {
                    Logger.LogToFile("{0} isn't a valid CNT file", Path);
                    return null;
                }

                cnt = Load(br);

                Logger.LogToFile("{0} :: {1}", br.BaseStream.Position.ToString("X"), br.BaseStream.Length.ToString("X"));
                if (br.BaseStream.Position != br.BaseStream.Length) { Logger.LogToFile("Still has data remaining"); }
            }

            return cnt;
        }

        private static CNT Load(BinaryReader br, CNT parent = null)
        {
            // The Load(BinaryReader) version skips the header check and is used for recursive loading
            CNT cnt = new CNT();
            if (parent != null) { cnt.parent = parent; }

            Logger.LogToFile("{0}", br.BaseStream.Position.ToString("X"));

            int nameLength = (int)br.ReadUInt32();
            int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

            cnt.name = br.ReadString(nameLength);
            br.ReadBytes(padding);

            Logger.LogToFile("Name: \"{0}\" of length {1}, padding of {2}", cnt.Name, nameLength, padding);

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
                case "EMIT":
                    br.ReadBytes(177);
                    break;

                case "MODL":
                    nameLength = (int)br.ReadUInt32();
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                    cnt.modelName = br.ReadString(nameLength);
                    br.ReadBytes(padding);

                    Logger.LogToFile("{0}: \"{1}\" of length {2}, padding of {3}", cnt.section, cnt.modelName, nameLength, padding);
                    break;

                case "NULL":
                    break;

                default:
                    throw new NotImplementedException(string.Format("Load code for CNT section {0} does not exist! ({1})", cnt.Section, br.BaseStream.Position.ToString("X")));
            }

            int childNodes = (int)br.ReadUInt32();

            for (int i = 0; i < childNodes; i++)
            {
                Logger.LogToFile("Loading child {0} of {1}", (i + 1), childNodes);
                cnt.childNodes.Add(Load(br, cnt));
            }

            br.ReadUInt32();    // Terminator

            return cnt;
        }

        public void Save(string Path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path, FileMode.Create)))
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
                case "MODL":
                    nameLength = cnt.Model.Length;
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                    bw.Write(nameLength);
                    bw.WriteString(cnt.Name);
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
}
