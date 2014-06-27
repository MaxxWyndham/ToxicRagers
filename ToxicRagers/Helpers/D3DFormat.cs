using System;

namespace ToxicRagers.Helpers
{
    public enum D3DFormat : uint
    {
        // Historic (occur in x360 files)
        X360_A8R8G8B8 = 6,
        X360_DXT1 = 18,
        X360_DXT2 = 19,

        A8R8G8B8 = 21,
        A8 = 28,
        ATI2 = 0x32495441,  // MakeFourCC('A', 'T', 'I', '2')
        DXT1 = 0x31545844,  // MakeFourCC('D', 'X', 'T', '1')
        DXT5 = 0x35545844   // MakeFourCC('D', 'X', 'T', '5')
    }
}
