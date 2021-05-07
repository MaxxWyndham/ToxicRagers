using TR = ToxicRagers.Carmageddon2.Formats;

using Xunit;

namespace ToxicRagers.Tests.Carmageddon2.Formats
{
    public class RacesTXT
    {
        [Fact]
        public void RacesTXT_Load_IsNotNull()
        {
            var races = TR.RacesTXT.Load(@"D:\Carmageddon Installations\Carmageddon 2\data\RACES.TXT");

            Assert.NotNull(races);
        }
    }
}
