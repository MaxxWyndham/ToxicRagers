using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToxicRagers.CarmageddonReincarnation.VirtualTextures;
using Xunit;

namespace ToxicRagers.Tests.CarmageddonReincarnation.VirtualTextures
{
    public class VirtualTextures
    {
        [Fact]
        public void ExtractTexture()
        {
            vtUtils vttools = new vtUtils(@"G:\steam\steamapps\common\Carmageddon Max Damage");
            var texresult = vttools.FindTexture("t_rock_arid_smooth_d");
            vttools.SaveTexture(texresult.Texture,
                @"G:\steam\steamapps\common\Carmageddon Max Damage\zad_vt\tower\extracted\test\t_rock_arid_smooth_d.tga",
                texresult.vtDef.GetPage(texresult.Texture.Map, 1));
        }
    }
}
