using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using ToxicRagers.CarmageddonReincarnation.Formats;
using ToxicRagers.Stainless.Formats;

namespace ToxicRagers.CarmageddonReincarnation.VirtualTextures
{
    public class VTMapTileTDX
    {
        List<VTMapTile> coords = new List<VTMapTile>();

        string tileName;
        TDX texture = null;

        public string ZADFile
        {
            get;
            set;
        }

        public string ZADEntryLocation
        {
            get;
            set;
        }

        public TDX Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public List<VTMapTile> Coords
        {
            get { return coords; }
            set { coords = value; }
        }

        public string TileName
        {
            get { return tileName; }
            set { tileName = value; }
        }

        public void GetTextureFromZAD()
        {
            if (File.Exists(ZADFile))
            {
                ZAD currentZAD = ZAD.Load(ZADFile);
                var zadEntry = (from entry in currentZAD.Contents where entry.Name == ZADEntryLocation || entry.Name == ZADEntryLocation.Replace("\\", "/") select entry).First();
                
                var buffer = currentZAD.ExtractToBuffer(zadEntry);
                if (buffer != null)
                    using (MemoryStream stream = new MemoryStream(buffer))
                    {
                        texture = TDX.Load(stream, zadEntry.Name);
                    }
            }
        }
    }
}
