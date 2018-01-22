namespace ToxicRagers.CarmageddonReincarnation.VirtualTextures
{
    public class VTMapTile
    {
        int column;
        int row;
        int page;
        uint hash;
        string tileName;
        string zadTileName;

        public int Column
        {
            get => column;
            set => column = value;
        }

        public int Row
        {
            get => row;
            set => row = value;
        }

        public int Page
        {
            get => page;
            set => page = value;
        }

        public uint Hash
        {
            get => hash;
            set => hash = value;
        }

        public string TileName
        {
            get => tileName;
            set => tileName = value;
        }

        public string ZadTileName
        {
            get => zadTileName;
            set => zadTileName = value;
        }

        public VTMapTileTDX TDXTile { get; set; }
    }
}