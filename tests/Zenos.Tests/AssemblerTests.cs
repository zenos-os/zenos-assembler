using System.IO;
using Xunit;
using Zenos.Assembly;

namespace Zenos.Assembler.Tests
{
    public class AssemblerTests
    {
        [Theory]
        [InlineData("basic.asm")]
        [InlineData("comments.asm")]
        [InlineData("empty-file.asm")]
        [InlineData("hello.asm")]
        [InlineData("minimal.asm")]
        //[InlineData("variables.asm")]
        public void NasmTest(string filename)
        {
            var fullPath = Path.Combine("resources", "Assembler", filename);
            var fileContent = File.ReadAllText(fullPath);
            var expectedBytes = TestHelpers.NasmBuildBinFromString64(fileContent);
            Assert.NotNull(expectedBytes);

            var listing = new AssemblerParser().Parse(fileContent);
            var assembler = new Assembly.Assembler();

            var error = assembler.Assemble(listing);
            if (!string.IsNullOrWhiteSpace(error))
            {
                Assert.True(false, "\r\n" + error + "\r\n");
            }
            var actualBytes = assembler.GetBytes();
            Assert.NotNull(actualBytes);
            TestHelpers.AssertByteArrays(expectedBytes, actualBytes);
        }
    }
}