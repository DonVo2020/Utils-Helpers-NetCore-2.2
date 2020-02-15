using System.IO;
using System.Threading.Tasks;
using IO_Utils.Extentions;
using NUnit.Framework;

namespace IO_UtilsTests.Extentions
{
    [TestFixture]
    internal class StreamExtentionTests
    {
        private readonly FileInfo readFileInfo = new FileInfo(typeof(StreamExtentionTests).Assembly.Location);

        [Test]
        public void TestReadToEnd()
        {
            using (FileStream fileStream = readFileInfo.OpenRead())
            {
                byte[] realContent;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    realContent = memoryStream.ToArray();
                }

                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadToEnd());
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadToEnd(readFileInfo.Length));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadToEnd(readFileInfo.Length + 1));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadToEnd(1));
            }
        }
        [Test]
        public async Task TestReadToEndAsync()
        {
            using (FileStream fileStream = readFileInfo.OpenRead())
            {
                byte[] realContent;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    realContent = memoryStream.ToArray();
                }

                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadToEndAsync());
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadToEndAsync(readFileInfo.Length));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadToEndAsync(readFileInfo.Length + 1));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadToEndAsync(1));
            }
        }
        [Test]
        public void TestReadMax()
        {
            using (FileStream fileStream = readFileInfo.OpenRead())
            {
                byte[] realContent = fileStream.ReadToEnd();

                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadMax(readFileInfo.Length));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadMax(readFileInfo.Length, readFileInfo.Length));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadMax(readFileInfo.Length, readFileInfo.Length + 1));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadMax(readFileInfo.Length, 1));

                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadMax(readFileInfo.Length + 1));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadMax(readFileInfo.Length + 1, readFileInfo.Length));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadMax(readFileInfo.Length + 1, readFileInfo.Length + 1));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, fileStream.ReadMax(readFileInfo.Length + 1, 1));

                byte[] partialContent = { realContent[0], realContent[1] };
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(partialContent, fileStream.ReadMax(2));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(partialContent, fileStream.ReadMax(2, 2));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(partialContent, fileStream.ReadMax(2, 3));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(partialContent, fileStream.ReadMax(2, 1));
            }
        }
        [Test]
        public async Task TestReadMaxAsync()
        {
            using (FileStream fileStream = readFileInfo.OpenRead())
            {
                byte[] realContent = await fileStream.ReadToEndAsync();

                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadMaxAsync(readFileInfo.Length));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadMaxAsync(readFileInfo.Length, readFileInfo.Length));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadMaxAsync(readFileInfo.Length, readFileInfo.Length + 1));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadMaxAsync(readFileInfo.Length, 1));

                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadMaxAsync(readFileInfo.Length + 1));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadMaxAsync(readFileInfo.Length + 1, readFileInfo.Length));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadMaxAsync(readFileInfo.Length + 1, readFileInfo.Length + 1));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(realContent, await fileStream.ReadMaxAsync(readFileInfo.Length + 1, 1));

                byte[] partialContent = { realContent[0], realContent[1] };
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(partialContent, await fileStream.ReadMaxAsync(2));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(partialContent, await fileStream.ReadMaxAsync(2, 2));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(partialContent, await fileStream.ReadMaxAsync(2, 3));
                fileStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(partialContent, await fileStream.ReadMaxAsync(2, 1));
            }
        }
        [Test]
        public void TestWriteAllBuffer()
        {
            byte[] realContent;
            using (FileStream fileStream = readFileInfo.OpenRead())
            {
                realContent = fileStream.ReadToEnd();
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(realContent);
                Assert.AreEqual(realContent, memoryStream.ToArray());
            }
        }
        [Test]
        public async Task TestWriteAllBufferAsync()
        {
            byte[] realContent;
            using (FileStream fileStream = readFileInfo.OpenRead())
            {
                realContent = await fileStream.ReadToEndAsync();
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await memoryStream.WriteAsync(realContent);
                Assert.AreEqual(realContent, memoryStream.ToArray());
            }
        }
    }
}
