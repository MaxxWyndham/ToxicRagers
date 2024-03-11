namespace ToxicRagers.Helpers
{
    public enum D3DFormat : uint
    {
        PVRTC4 = 13,

        R4G4B4A4 = 16,
        R5G5B6 = 17,
        A8B8G8R8 = 18,
        R5G6B5 = 19,

        A8R8G8B8 = 21,
        X8R8G8B8 = 22,
        A4R4G4B4 = 26,
        A8 = 28,
        ATI2 = 0x32495441,  // MakeFourCC('A', 'T', 'I', '2')
        DXT1 = 0x31545844,  // MakeFourCC('D', 'X', 'T', '1')
        DXT3 = 0x33545844,  // MakeFourCC('D', 'X', 'T', '3')
        DXT5 = 0x35545844   // MakeFourCC('D', 'X', 'T', '5')
    }
}