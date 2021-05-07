using TR = ToxicRagers.Carmageddon2.Formats;

using Xunit;

namespace ToxicRagers.Tests.Carmageddon2.Formats
{
    public class TWT
    {
        [Fact]
        public void TWT_Load_IsNotNull()
        {
            var twt = TR.TWT.Load(@"Files\Carmageddon2\test.twt");

            Assert.NotNull(twt);
        }

        [Fact]
        public void TWT_Extract()
        {
            var twt = TR.TWT.Load(@"Files\Carmageddon2\test.twt");
            var entry = twt.Contents[1];

            byte[] data = twt.Extract(entry);

            Assert.True(entry.Name == "Monkey.txt" && data.Length == 0x35);
        }
    }
}
