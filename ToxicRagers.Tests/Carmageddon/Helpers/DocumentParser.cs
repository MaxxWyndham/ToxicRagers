using System.Text;

using TR = ToxicRagers.Carmageddon.Helpers;

using Xunit;

namespace ToxicRagers.Tests.Carmageddon.Helpers
{
    public class DocumentParser
    {
        [Fact]
        public void DocumentParser_New()
        {
            var file = new TR.DocumentParser(@"Files\Carmageddon\TestWithCommentsEncoded.txt");

            Assert.NotNull(file);
        }

        [Fact]
        public void DocumentParser_Decode()
        {
            var file = new TR.DocumentParser(@"Files\Carmageddon\TestWithCommentsEncoded.txt");

            file.ReadLine();

            Assert.True(file.ReadLine() == "Or a llama.");
        }

        [Fact]
        public void DocumentParser_Encode()
        {
            var file = new TR.DocumentParser(@"Files\Carmageddon\TestWithCommentsEncoded.txt");

            file.Encode();

            Assert.True(file.Data.Length == 0x68 && file.Data[7] == 0x93);
        }
    }
}
