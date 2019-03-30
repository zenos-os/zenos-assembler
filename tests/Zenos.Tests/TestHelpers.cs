using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace Zenos.Assembler.Tests
{
    public static class TestHelpers
    {
        public static readonly string SolutionRoot;
        public static readonly string ToolsRoot;
        public static readonly string NasmPath;

        static TestHelpers()
        {
            SolutionRoot = Environment.GetEnvironmentVariable("NCrunch") == "1"
                ? Path.GetDirectoryName(Environment.GetEnvironmentVariable("NCrunch.OriginalSolutionPath"))
                : Path.Combine("..", "..", "..", "..", "..");

            ToolsRoot = Path.Combine(SolutionRoot, "tools");
            NasmPath = Path.Combine(ToolsRoot, "nasm.exe");
        }

        public static string Resource(string name) => Path.Combine("resources", "Assembler", name);

        public static byte[] NasmBuildElf(string filename) => NasmBuild(filename, "-felf64");

        public static byte[] NasmBuild(string filename, string options)
        {
            var output = Path.GetTempFileName();
            try
            {
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = NasmPath,
                        Arguments = $"{options} {filename} -o {output}",
                        RedirectStandardError = true,
                        //RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };

                proc.Start();

                var errorText = proc.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(errorText))
                {
                    Assert.True(false, errorText);
                }
                Assert.Equal("", errorText);
                proc.WaitForExit();

                Assert.Equal(0, proc.ExitCode);
                return File.ReadAllBytes(output);
            }
            finally
            {
                File.Delete(output);
            }
        }

        public static byte[] NasmBuildBin(string filename) => NasmBuild(filename, "");

        public static byte[] NasmBuildBinFromString64(string assembler)
        {
            return NasmBuildBinFromString($"bits 64\r\n{assembler}");
        }

        public static byte[] NasmBuildBinFromString(string input)
        {
            var inputFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(inputFile, input);
                return NasmBuildBin(inputFile);
            }
            finally
            {
                File.Delete(inputFile);
            }
        }

        public static void AssertByteArrays(IList<byte> expected, IList<byte> actual, string message = "")
        {
            string BinaryValue(IList<byte> bytes, int i) =>
                i < bytes.Count
                    ? "0b" + Convert.ToString(bytes[i], 2).PadLeft(8, '0')
                    : "MISSING";

            string BuildErrString(int i, IList<byte> expectedBytes, IList<byte> actualBytes)
            {
                var expectedBinary = BinaryValue(expectedBytes, i);
                var actualBinary = BinaryValue(actualBytes, i);

                return message +
                       $"Expected: {string.Join(", ", expectedBytes.Select(x => $"0x{x:X2}"))}\r\n" +
                       $"Actual:   {string.Join(", ", actualBytes.Select(x => $"0x{x:X2}"))}\r\n" +
                       new string('-', 10 + i * 6) + "^^^^\r\n" +
                       "\r\n" +
                       $"At offset: {i}\r\n" +
                       $"Expected: {expectedBinary}\r\n" +
                       $"Actual:   {actualBinary}"
                    ;
            }

            for (var i = 0; i < expected.Count; i++)
            {
                if (i >= actual.Count || expected[i] != actual[i])
                {
                    throw new XunitException("\r\n" + BuildErrString(i, expected, actual));
                }
            }
        }
    }
}