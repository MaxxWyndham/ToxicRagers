using System.Collections.Generic;

using ToxicRagers.Helpers;

namespace ToxicRagers.Generics
{
    public interface ITexture
    {
        string Name { get; set; }

        string Extension { get; }

        List<MipMap> MipMaps { get; set; }

        D3DFormat Format { get; }
    }
}