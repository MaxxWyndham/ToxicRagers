using System;
using System.ComponentModel;
using System.Web;
using ToxicRagers.Helpers;
using ToxicRagers.Carmageddon.Formats;
using ToxicRagers.Carmageddon2.Formats;

namespace ToxicRagers.Carmageddon.Helpers
{
    public class c1Car
    {
        // A wrapper class, brings together everything necessary to load (and eventually save) a car.
        #region Properties
        string name;
        Vector3 headposition = Vector3.Zero;
        Vector2 headturnangles = Vector2.Zero;
        Vector3 mirrorcamoffset = Vector3.Zero;
        int mirrorviewingangle = 0;
        string[] pratcamborders = new string[4];
        bool stealworthy = false;
        int modelcount = 0;
        DAT[] models;
        public CrushData[] Crushes;
        int actorcount = 0;
        c2Act[] actors;

        public string RootPath;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DescriptionAttribute("Offset of driver's head in 3D space")]
        public Vector3 DriverHeadPosition
        {
            get { return headposition; }
            set { headposition = value; }
        }

        [DescriptionAttribute("Angles to turn to make head go left and right")]
        public Vector2 DriverHeadTurnAngles
        {
            get { return headturnangles; }
            set { headturnangles = value; }
        }

        [DescriptionAttribute("Offset of 'mirror camera' in 3D space")]
        public Vector3 MirrorCamOffset
        {
            get { return mirrorcamoffset; }
            set { mirrorcamoffset = value; }
        }

        [DescriptionAttribute("Viewing angle of mirror")]
        public int MirrorCamViewingAngle
        {
            get { return mirrorviewingangle; }
            set { mirrorviewingangle = value; }
        }

        [DescriptionAttribute("Pratcam border names")]

        [TypeConverterAttribute(typeof(StringArrayConverter))]
        public string[] PratcamBorders
        {
            get { return pratcamborders; }
            set { pratcamborders = value; }
        }

        [DescriptionAttribute("Can be stolen")]
        public bool StealWorthy { get { return stealworthy; } set { stealworthy = value; } }

        [BrowsableAttribute(false)]
        public int ModelCount
        {
            get { return modelcount; }
            set
            {
                modelcount = value;
                models = new DAT[modelcount];
                Crushes = new CrushData[modelcount];
            }
        }

        [BrowsableAttribute(false)]
        public DAT[] Models
        {
            get { return models; }
        }

        [BrowsableAttribute(false)]
        public int ActorCount
        {
            get { return actorcount; }
            set
            {
                actorcount = value;
                actors = new c2Act[actorcount];
            }
        }

        [BrowsableAttribute(false)]
        public c2Act[] Actors
        {
            get { return actors; }
        }
        #endregion

        #region Constructors
        public c1Car(string CarName)
        {
            name = CarName;
        }
        #endregion

        public void Load(string rootPath, bool bRootPath = true)
        {
            if (!bRootPath)
            {
                rootPath = rootPath.Substring(0, rootPath.LastIndexOf("\\")).ToLower().Replace("\\cars", "\\");
            }

            RootPath = rootPath;

            c1CarTXT.Load(rootPath + "\\CARS\\" + name + ".txt", this);

            // save crush data
            bool bSwapped = false;
            for (int i = 0; i < Crushes.Length; i++)
            {
                if (Crushes[i] != null && Crushes[i].ChunkCount > 0)
                {
                    // this is a cobble-mess because the crush data is in order of loaded actor, not loaded model
                    int modelindex = i;
                    if (Crushes[i].MaxRootVertex > models[i].DatMeshes[0].Mesh.Verts.Count) { modelindex++; bSwapped = true; } else if (bSwapped) { modelindex--; }


                    c2Act crushpoints = new c2Act();
                    crushpoints.AddRootNode("crushpoints");

                    //Console.WriteLine("Processing " + models[modelindex].DatMeshes[0].Name + " (V: " + models[modelindex].DatMeshes[0].Mesh.Verts.Count + ", MV: " + Crushes[i].MaxRootVertex + ")");
                    for (int j = 0; j < Crushes[i].ChunkCount; j++)
                    {
                        Matrix3D m = Matrix3D.Identity;
                        m.Scale = 0.05f;
                        m.Position = models[modelindex].DatMeshes[0].Mesh.Verts[Crushes[i].Chunks[j].RootVertex];
                        crushpoints.AddActor("p" + j, "Sphere_48", m, false);
                    }

                    //crushpoints.Save("D:\\CrushDebug\\" + name + "_" + i + "_Points.act");
                }
            }
        }

        public void LoadModel(int modelnum, string modelname)
        {
            if (models[modelnum] == null) { models[modelnum] = DAT.Load(RootPath + "\\MODELS\\" + modelname); }
        }

        public void LoadActor(int actornum, string actorname)
        {
            if (actors[actornum] == null) { actors[actornum] = new c2Act(); }
            actors[actornum].Load(RootPath + "\\ACTORS\\" + actorname);
        }
    }

    public class CrushData
    {
        public Single UnknownA;
        public Vector2 UnknownB;
        public Single UnknownC;
        public Single UnknownD;
        public Single UnknownE;
        public Single UnknownF;

        public int ChunkCount;
        public CrushChunk[] Chunks;

        public void SetChunkCount(int i)
        {
            ChunkCount = i;
            Chunks = new CrushChunk[i];
        }

        public int MaxRootVertex
        {
            get
            {
                int r = 0;

                for (int i = 0; i < ChunkCount; i++)
                {
                    if (Chunks[i].RootVertex > r) { r = Chunks[i].RootVertex; }
                }

                return r;
            }
        }
    }

    public class CrushChunk
    {
        public int RootVertex;
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;
        public Vector3 D;

        public int ChunkEntryCount;
        public int RootEntryVertex;
        public CrushChunkEntry[] CrushVerts;

        public void SetChunkEntryCount(int i)
        {
            ChunkEntryCount = i;
            CrushVerts = new CrushChunkEntry[i];
        }
    }

    public class CrushChunkEntry
    {
        public int VertIndex;
        public int Weight;
    }
}
