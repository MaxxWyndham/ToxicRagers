using System.Drawing;

using CoreFormats = ToxicRagers.Core.Formats;

using Xunit;

namespace ToxicRagers.Tests.Core.Formats
{
    public class TGA
    {
        [Fact]
        public void TGA_FromBitmap()
        {
            var tga = CoreFormats.TGA.FromBitmap((Bitmap)Image.FromFile(@"Files\Core\errol.256x256.png"));

            Assert.NotNull(tga);
        }
    }
}
