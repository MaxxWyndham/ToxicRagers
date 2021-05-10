using TR = ToxicRagers.Carmageddon2.Formats;

using Xunit;

namespace ToxicRagers.Tests.Carmageddon2.Formats
{
    public class OpponentTXT
    {
        [Fact]
        public void OpponentTXT_Load_IsNotNull()
        {
            var opponents = TR.OpponentTXT.Load(@"Files\Carmageddon2\OPPONENT.TXT");

            Assert.NotNull(opponents);
        }
    }
}
