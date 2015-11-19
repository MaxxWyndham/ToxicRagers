using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ToxicRagers.Stainless.Formats;
using ToxicRagers.Core.Formats;
using ToxicRagers.Helpers;


namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class crVTMapTileTDX
    {
        List<crVTMapTile> coords = new List<crVTMapTile>();

        byte[] tileName = new byte[4];
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
        public List<crVTMapTile> Coords
        {
            get { return coords; }
            set { coords = value; }
        }
        public byte[] TileName
        {
            get { return tileName; }
            set { tileName = value; }
        }
        public string TileNameString
        {
            get { return BitConverter.ToString(new byte[] { tileName[3], tileName[2], tileName[1], tileName[0] }).Replace("-", string.Empty); }
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
                        texture = TDX.LoadFromMemoryStream(stream, zadEntry.Name);
                    }
            }
        }
    }
}
