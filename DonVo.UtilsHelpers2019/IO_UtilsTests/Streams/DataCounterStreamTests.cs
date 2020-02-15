using System.IO;
using System.Threading.Tasks;
using IO_Utils.Streams;
using NUnit.Framework;

namespace IO_UtilsTests.Streams
{
    [TestFixture]
    internal class DataCounterStreamTests
    {
        private readonly FileInfo readFileInfo = new FileInfo(typeof(WrapStreamTests).Assembly.Location);

        [Test]
        public void TestCounter()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (DataCounterStream dataCounterStream = new DataCounterStream(memoryStream))
            {
                using (FileStream fileStream = readFileInfo.OpenRead())
                {
                    fileStream.CopyTo(dataCounterStream);
                }
                Assert.AreEqual(readFileInfo.Length, dataCounterStream.WritedBytes.Bytes);

                dataCounterStream.Seek(0, SeekOrigin.Begin);
                dataCounterStream.CopyTo(Stream.Null);
                Assert.AreEqual(readFileInfo.Length, dataCounterStream.ReadedBytes.Bytes);
            }
        }
        [Test]
        public void TestCounterByteByByte()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (DataCounterStream dataCounterStream = new DataCounterStream(memoryStream))
            {
                using (FileStream fileStream = readFileInfo.OpenRead())
                {
                    for (int i = 0; i < readFileInfo.Length; i++)
                    {
                        dataCounterStream.WriteByte((byte)fileStream.ReadByte());
                    }
                }
                Assert.AreEqual(readFileInfo.Length, dataCounterStream.WritedBytes.Bytes);

                dataCounterStream.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < readFileInfo.Length; i++)
                {
                    Assert.AreNotEqual(-1, dataCounterStream.ReadByte());
                }
                Assert.AreEqual(readFileInfo.Length, dataCounterStream.ReadedBytes.Bytes);
                Assert.AreEqual(-1, dataCounterStream.ReadByte());
                Assert.AreEqual(readFileInfo.Length, dataCounterStream.ReadedBytes.Bytes);
            }
        }
        [Test]
        public async Task TestCounterAsync()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (DataCounterStream dataCounterStream = new DataCounterStream(memoryStream))
            {
                using (FileStream fileStream = readFileInfo.OpenRead())
                {
                    await fileStream.CopyToAsync(dataCounterStream);
                }
                Assert.AreEqual(readFileInfo.Length, dataCounterStream.WritedBytes.Bytes);

                dataCounterStream.Seek(0, SeekOrigin.Begin);
                await dataCounterStream.CopyToAsync(Stream.Null);
                Assert.AreEqual(readFileInfo.Length, dataCounterStream.ReadedBytes.Bytes);
            }
        }
    }
}
