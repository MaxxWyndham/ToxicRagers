using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;


using ToxicRagers.CarmageddonReincarnation.Formats;
using ToxicRagers.Stainless.Formats;

namespace ToxicRagers.CarmageddonReincarnation.VirtualTextures
{
    public class VirtualTextureDef
    {
        public VTMap DiffuseMap;
        public VTMap NormalMap;
        public VTMap SpecularMap;
        public List<VTPage> DiffusePages;
        public List<VTPage> NormalPages;
        public List<VTPage> SpecularPages;

        public VTPage GetPage(VTMap map, int page)
        {
            if (map == DiffuseMap)
            {
                return DiffusePages[page];
            }
            else if (map == NormalMap)
            {
                return NormalPages[page];
            }
            else if (map == SpecularMap)
            {
                return SpecularPages[page];
            }

            return null;
        }
    }

    public class VTTextureResult
    {
        public VTMapEntry Texture;
        public VirtualTextureDef vtDef;
    }

    public class vtUtils
    {
        public int Extracted = 0;
        public int ThreadsAlive = 0;
        public string CarmaFolder;

        public vtUtils(string carmaFolder)
        {
            CarmaFolder = carmaFolder;
        }
        public TDX LoadTDXFromZADEntry(ZADEntry entry, ZAD zadFile)
        {
            using (MemoryStream stream = new MemoryStream(zadFile.ExtractToBuffer(entry)))
            {
                TDX output = TDX.Load(stream, entry.Name);
                return output;
            }

        }

        public VirtualTextureDef LoadVT(string inputFolder)
        {
            var zadFiles = Directory.EnumerateFiles(inputFolder, "*.zad");
            //bool firstZad = true;
            ZAD EnvironmentsZAD = ZAD.Load(Path.Combine(inputFolder, "Environments.zad"));
            TDX DiffuseTDX = null;
            TDX SpecularTDX = null;
            TDX NormalTDX = null;
            //Console.WriteLine("Loading dictionary TDX files...");
            foreach (ZADEntry entry in EnvironmentsZAD.Contents)
            {
                //Console.WriteLine(entry.Name);
                switch (Path.GetFileName(entry.Name).ToLower())
                {
                    case "diffuse_d.tdx":
                        DiffuseTDX = LoadTDXFromZADEntry(entry, EnvironmentsZAD);
                        break;
                    case "specular_s.tdx":
                        SpecularTDX = LoadTDXFromZADEntry(entry, EnvironmentsZAD);
                        break;
                    case "normal_n.tdx":
                        NormalTDX = LoadTDXFromZADEntry(entry, EnvironmentsZAD);
                        break;
                }
            }

            VirtualTextureDef vtDef = new VirtualTextureDef
            {
                DiffuseMap = (VTMap)DiffuseTDX.ExtraData,
                SpecularMap = (VTMap)SpecularTDX.ExtraData,
                NormalMap = (VTMap)NormalTDX.ExtraData,
                DiffusePages = new List<VTPage>()
            };

            for (int i = 0; i < vtDef.DiffuseMap.PageCount + 1; i++)
            {
                int pageWidth = vtDef.DiffuseMap.GetWidth(i);
                int pageHeight = vtDef.DiffuseMap.GetHeight(i);
                vtDef.DiffusePages.Add(new VTPage(pageWidth, pageHeight, i, vtDef.DiffuseMap));
                //Console.WriteLine("\tDiffuse Page {0} created", i);
            }

            vtDef.SpecularPages = new List<VTPage>();

            for (int i = 0; i < vtDef.SpecularMap.PageCount + 1; i++)
            {
                int pageWidth = vtDef.SpecularMap.GetWidth(i);
                int pageHeight = vtDef.SpecularMap.GetHeight(i);
                vtDef.SpecularPages.Add(new VTPage(pageWidth, pageHeight, i, vtDef.SpecularMap));
                //Console.WriteLine("\tSpecular Page {0} created", i);
            }

            vtDef.NormalPages = new List<VTPage>();

            for (int i = 0; i < vtDef.NormalMap.PageCount + 1; i++)
            {
                int pageWidth = vtDef.NormalMap.GetWidth(i);
                int pageHeight = vtDef.NormalMap.GetHeight(i);
                vtDef.NormalPages.Add(new VTPage(pageWidth, pageHeight, i, vtDef.NormalMap));
                //Console.WriteLine("\tNormal Page {0} created", i);
            }

            foreach (string zadFile in zadFiles)
            {
                if (Path.GetFileNameWithoutExtension(zadFile).ToLower() == "environments") { continue; }
                //Console.Write("Loading ZAD: " + zadFile);
                /*if(Path.GetFileNameWithoutExtension(zadFile).ToLower() == "pages_5")
                {
                    Console.WriteLine("This is page 5");
                }*/
                ZAD currentZAD = ZAD.Load(zadFile);

                foreach (ZADEntry entry in currentZAD.Contents)
                {
                    if (entry.CompressionMethod != CompressionMethods.LZ4)
                    {
                        //Console.WriteLine("This entry isnt compressed using lz4! wtf? {0}", entry.Name);
                    }

                    string tdxName = Path.GetFileNameWithoutExtension(entry.Name).ToLower();
                    string tileName = tdxName.Split(new Char[] { '_' })[0].ToUpper();
                    /*if (tileName == "E4C7607E")
                    {
                        Console.WriteLine("This is E4C7607E");
                    }*/
                    if (vtDef.DiffuseMap.TilesByName.ContainsKey(tileName))
                    {
                        VTMapTileTDX tileTDX = vtDef.DiffuseMap.TilesByName[tileName];
                        //tileTDX.Texture = LoadTDXFromZADEntry(entry, currentZAD);
                        tileTDX.ZADFile = zadFile;
                        tileTDX.ZADEntryLocation = entry.Name;
                        for (int i = 0; i < tileTDX.Coords.Count; i++)
                        {
                            VTMapTile tile = tileTDX.Coords[i];
                            tile.TDXTile = tileTDX;
                            //if (tile.Row < diffusePages[tile.Page].maxTilesToStitch && tile.Column < diffusePages[tile.Page].maxTilesToStitch)
                            {
                                vtDef.DiffusePages[tile.Page].Tiles[tile.Row][tile.Column] = tileTDX;// LoadTDXFromZADEntry(entry, currentZAD);
                            }
                        }
                    }

                    if (vtDef.SpecularMap.TilesByName.ContainsKey(tileName))
                    {
                        VTMapTileTDX tileTDX = vtDef.SpecularMap.TilesByName[tileName];
                        //tileTDX.Texture = LoadTDXFromZADEntry(entry, currentZAD);
                        tileTDX.ZADFile = zadFile;
                        tileTDX.ZADEntryLocation = entry.Name;

                        for (int i = 0; i < tileTDX.Coords.Count; i++)
                        {
                            VTMapTile tile = tileTDX.Coords[i];
                            tile.TDXTile = tileTDX;

                            if (tile.Row < vtDef.SpecularPages[tile.Page].maxTilesToStitch && tile.Column < vtDef.SpecularPages[tile.Page].maxTilesToStitch)
                            {
                                vtDef.SpecularPages[tile.Page].Tiles[tile.Row][tile.Column] = tileTDX;// LoadTDXFromZADEntry(entry, currentZAD);
                            }
                        }
                    }

                    if (vtDef.NormalMap.TilesByName.ContainsKey(tileName))
                    {
                        //currentZAD.Extract(entry, Path.Combine(outputFolder, "Normal", "TDX")+"/");
                        VTMapTileTDX tileTDX = vtDef.NormalMap.TilesByName[tileName];
                        //tileTDX.Texture = LoadTDXFromZADEntry(entry, currentZAD);
                        tileTDX.ZADFile = zadFile;
                        tileTDX.ZADEntryLocation = entry.Name;
                        for (int i = 0; i < tileTDX.Coords.Count; i++)
                        {
                            VTMapTile tile = tileTDX.Coords[i];
                            tile.TDXTile = tileTDX;
                            if (tile.Row < vtDef.NormalPages[tile.Page].maxTilesToStitch && tile.Column < vtDef.NormalPages[tile.Page].maxTilesToStitch)
                            {
                                vtDef.NormalPages[tile.Page].Tiles[tile.Row][tile.Column] = tileTDX;// LoadTDXFromZADEntry(entry, currentZAD);
                            }
                        }
                    }
                }

                continue;

                foreach (ZADEntry entry in currentZAD.Contents)
                {
                    if (entry.CompressionMethod != CompressionMethods.LZ4)
                    {
                        //Console.WriteLine("This entry isnt compressed using lz4! wtf? {0}", entry.Name);

                    }

                    string tdxName = Path.GetFileNameWithoutExtension(entry.Name).ToLower();
                    string tileName = tdxName.Split(new Char[] { '_' })[0].ToUpper();
                    /*if (tileName == "E4C7607E")
                    {
                        Console.WriteLine("This is E4C7607E");
                    }*/
                    if (vtDef.DiffuseMap.TilesByName.ContainsKey(tileName))
                    {

                        VTMapTileTDX tileTDX = vtDef.DiffuseMap.TilesByName[tileName];
                        //tileTDX.Texture = LoadTDXFromZADEntry(entry, currentZAD);
                        tileTDX.ZADFile = zadFile;
                        tileTDX.ZADEntryLocation = entry.Name;
                        for (int i = 0; i < tileTDX.Coords.Count; i++)
                        {
                            VTMapTile tile = tileTDX.Coords[i];
                            tile.TDXTile = tileTDX;
                            //if (tile.Row < diffusePages[tile.Page].maxTilesToStitch && tile.Column < diffusePages[tile.Page].maxTilesToStitch)
                            {
                                vtDef.DiffusePages[tile.Page].Tiles[tile.Row][tile.Column] =
                                    tileTDX; // LoadTDXFromZADEntry(entry, currentZAD);
                            }
                        }
                    }

                    if (vtDef.SpecularMap.TilesByName.ContainsKey(tileName))
                    {

                        VTMapTileTDX tileTDX = vtDef.SpecularMap.TilesByName[tileName];
                        //tileTDX.Texture = LoadTDXFromZADEntry(entry, currentZAD);
                        tileTDX.ZADFile = zadFile;
                        tileTDX.ZADEntryLocation = entry.Name;
                        for (int i = 0; i < tileTDX.Coords.Count; i++)
                        {
                            VTMapTile tile = tileTDX.Coords[i];
                            tile.TDXTile = tileTDX;
                            if (tile.Row < vtDef.SpecularPages[tile.Page].maxTilesToStitch &&
                                tile.Column < vtDef.SpecularPages[tile.Page].maxTilesToStitch)
                            {
                                vtDef.SpecularPages[tile.Page].Tiles[tile.Row][tile.Column] =
                                    tileTDX; // LoadTDXFromZADEntry(entry, currentZAD);
                            }
                        }
                    }

                    if (vtDef.NormalMap.TilesByName.ContainsKey(tileName))
                    {
                        //currentZAD.Extract(entry, Path.Combine(outputFolder, "Normal", "TDX")+"/");
                        VTMapTileTDX tileTDX = vtDef.NormalMap.TilesByName[tileName];
                        //tileTDX.Texture = LoadTDXFromZADEntry(entry, currentZAD);
                        tileTDX.ZADFile = zadFile;
                        tileTDX.ZADEntryLocation = entry.Name;
                        for (int i = 0; i < tileTDX.Coords.Count; i++)
                        {
                            VTMapTile tile = tileTDX.Coords[i];
                            tile.TDXTile = tileTDX;
                            if (tile.Row < vtDef.NormalPages[tile.Page].maxTilesToStitch &&
                                tile.Column < vtDef.NormalPages[tile.Page].maxTilesToStitch)
                            {
                                vtDef.NormalPages[tile.Page].Tiles[tile.Row][tile.Column] =
                                    tileTDX; // LoadTDXFromZADEntry(entry, currentZAD);
                            }
                        }
                    }
                }
            }

            return vtDef;
        }

        public List<string> GetVTList()
        {
            var folders = Directory.GetDirectories(Path.Combine(CarmaFolder, "ZAD_VT"));

            List<string> VTFolders = new List<string>();
            foreach (var folder in folders)
            {
                if (!File.Exists(Path.Combine(folder, "Environments.zad"))) continue;
                VTFolders.Add(folder);

            }

            return VTFolders;
        }

        public string ExtractTexture(string textureName)
        {
            VTTextureResult findTextureResult = FindTexture(textureName);
            if (findTextureResult.Texture == null)
            {
                return "";
            }
            string fileName = Path.Combine(CarmaFolder, findTextureResult.Texture.FileName);
            fileName = Path.ChangeExtension(fileName, ".tga");
            SaveTexture(findTextureResult.Texture, fileName, findTextureResult.vtDef.GetPage(findTextureResult.Texture.Map, 1));

            return fileName;
        }
        public string ExtractTexture(string textureName, string outputPath)
        {
            VTTextureResult findTextureResult = FindTexture(textureName);
            if (findTextureResult.Texture == null)
            {
                return "";
            }
            string fileName = Path.Combine(outputPath, textureName);
            fileName = Path.ChangeExtension(fileName, ".tga");
            SaveTexture(findTextureResult.Texture, fileName, findTextureResult.vtDef.GetPage(findTextureResult.Texture.Map, 1));
            return fileName;
        }

        public VTTextureResult FindTexture(string textureName)
        {
            List<string> vtList = GetVTList();
            VTTextureResult result = new VTTextureResult();

            foreach (string vtPath in vtList)
            {
                VirtualTextureDef vtDef = LoadVT(vtPath);
                VTMapEntry tex = FindTexture(textureName, vtDef);
                if (tex != null)
                {
                    result.vtDef = vtDef;
                    result.Texture = tex;
                    break;
                }
            }

            return result;
        }

        public VTMapEntry FindTexture(string textureName, VirtualTextureDef vtDef)
        {
            return FindTexture(textureName, vtDef, true, true, true);
        }

        public VTMapEntry FindTexture(string textureName, VirtualTextureDef vtDef, bool searchDiffuse, bool searchSpecular, bool searchNormals)
        {
            VTMapEntry tex = null;

            if (searchDiffuse)
            {
                tex = FindTexture(textureName, vtDef.DiffuseMap);
            }

            if (searchSpecular && tex == null)
            {
                tex = FindTexture(textureName, vtDef.SpecularMap);
            }

            if (searchNormals && tex == null)
            {
                tex = FindTexture(textureName, vtDef.NormalMap);
            }

            return tex;
        }

        public VTMapEntry FindTexture(string textureName, VTMap vtMap)
        {
            VTMapEntry tex = null;

            for (int i = 0; i < vtMap.Entries.Count; i++)
            {
                if (Path.GetFileName(vtMap.Entries[i].FileName).ToLower() == textureName.ToLower())
                {
                    return vtMap.Entries[i];
                }
            }

            return tex;
        }
        public void SaveAllTexturesFromVT(VirtualTextureDef vtDef, string pathName)
        {
            if (!Directory.Exists(pathName))
            {
                Directory.CreateDirectory(pathName);
            }

            foreach (VTMapEntry entry in vtDef.DiffuseMap.Entries)
            {
                SaveTexture(entry,
                    Path.Combine(pathName, "Diffuse",
                        Path.GetFileNameWithoutExtension(entry.FileName) + ".tga"),
                    vtDef.DiffusePages[1]);
            }

            foreach (VTMapEntry entry in vtDef.SpecularMap.Entries)
            {
                SaveTexture(entry,
                    Path.Combine(pathName, "Specular",
                        Path.GetFileNameWithoutExtension(entry.FileName) + ".tga"),
                    vtDef.SpecularPages[1]);
            }

            foreach (VTMapEntry entry in vtDef.NormalMap.Entries)
            {
                SaveTexture(entry,
                    Path.Combine(pathName, "Normal",
                        Path.GetFileNameWithoutExtension(entry.FileName) + ".tga"),
                    vtDef.NormalPages[1]);
            }
        }

        public void SaveTexture(VTMapEntry textureToExport, String FileName, VTPage vtPage)
        {
            var fileType = Path.GetExtension(FileName).ToUpper();
            ImageFormat imgFormat = ImageFormat.Png;

            if (fileType == ".TGA")
            {
                vtPage.SaveTexture(textureToExport, FileName, false, true, false, ImageFormat.Png);
            }
            else if (fileType == ".TDX")
            {
                vtPage.SaveTexture(textureToExport, FileName, true, false, false, ImageFormat.Png);
            }
            else
            {
                switch (fileType)
                {
                    case ".JPG":
                        imgFormat = ImageFormat.Jpeg;
                        break;
                    case ".JPEG":
                        imgFormat = ImageFormat.Jpeg;
                        break;
                    case ".TIFF":
                        imgFormat = ImageFormat.Tiff;
                        break;
                    case ".TIF":
                        imgFormat = ImageFormat.Tiff;
                        break;
                    case ".BMP":
                        imgFormat = ImageFormat.Bmp;
                        break;
                    case ".PNG":
                        imgFormat = ImageFormat.Png;
                        break;
                }

                vtPage.SaveTexture(textureToExport, FileName, imgFormat);
            }
        }
    }
}