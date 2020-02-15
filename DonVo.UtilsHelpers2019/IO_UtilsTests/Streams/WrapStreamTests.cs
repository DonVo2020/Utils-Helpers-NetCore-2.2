using System.IO;
using System.Threading.Tasks;
using IO_Utils.Streams;
using IO_UtilsTests.TestUtils;
using IO_Utils.Extentions;
using NUnit.Framework;

namespace IO_UtilsTests.Streams
{
    [TestFixture]
    internal class WrapStreamTests
    {
        private class WrapStreamImpl : WrapStream
        {
            public WrapStreamImpl(Stream basseStream, bool leaveOpen = false) : base(basseStream, leaveOpen) { }
        }


        private readonly FileInfo readFileInfo = new FileInfo(typeof(WrapStreamTests).Assembly.Location);

        [Test]
        public void TestRead()
        {
            using (FileStream fileStream = readFileInfo.OpenRead())
            using (RandomReadNumberStream randomReadNumberStream = new RandomReadNumberStream(fileStream))
            using (WrapStream wrapStream = new WrapStreamImpl(randomReadNumberStream))
            {
                byte[] wrapContent = wrapStream.ReadToEnd();
                fileStream.Seek(0, SeekOrigin.Begin);
                byte[] fileContent = fileStream.ReadToEnd();

                Assert.AreEqual(wrapContent, fileContent);
            }
        }
        [Test]
        public async Task TestReadAsync()
        {
            using (FileStream fileStream = readFileInfo.OpenRead())
            using (RandomReadNumberStream randomReadNumberStream = new RandomReadNumberStream(fileStream))
            using (WrapStream wrapStream = new WrapStreamImpl(randomReadNumberStream))
            {
                byte[] wrapContent = await wrapStream.ReadToEndAsync();
                fileStream.Seek(0, SeekOrigin.Begin);
                byte[] plainContent = await fileStream.ReadToEndAsync();

                Assert.AreEqual(plainContent, wrapContent);
            }
        }
        [Test]
        public void TestWrite()
        {
            byte[] wrapContent, plainContent;
            using (FileStream fileStream = readFileInfo.OpenRead())
            using (RandomReadNumberStream randomReadNumberStream = new RandomReadNumberStream(fileStream))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (WrapStream wrapStream = new WrapStreamImpl(memoryStream))
                {
                    randomReadNumberStream.CopyTo(wrapStream);
                    wrapContent = memoryStream.ToArray();
                }

                fileStream.Seek(0, SeekOrigin.Begin);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    randomReadNumberStream.CopyTo(memoryStream);
                    plainContent = memoryStream.ToArray();
                }
            }

            Assert.AreEqual(plainContent, wrapContent);
        }
        [Test]
        public async Task TestWriteAsync()
        {
            byte[] wrapContent, plainContent;
            using (FileStream fileStream = readFileInfo.OpenRead())
            using (RandomReadNumberStream randomReadNumberStream = new RandomReadNumberStream(fileStream))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (WrapStream wrapStream = new WrapStreamImpl(memoryStream))
                {
                    await randomReadNumberStream.CopyToAsync(wrapStream);
                    wrapContent = memoryStream.ToArray();
                }

                fileStream.Seek(0, SeekOrigin.Begin);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await randomReadNumberStream.CopyToAsync(memoryStream);
                    plainContent = memoryStream.ToArray();
                }
            }

            Assert.AreEqual(plainContent, wrapContent);
        }
        [Test]
        public void TestSetLength()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (WrapStream wrapStream = new WrapStreamImpl(memoryStream))
            {
                Assert.AreEqual(0, memoryStream.Length);
                wrapStream.SetLength(2);
                Assert.AreEqual(2, memoryStream.Length);
            }
        }
        [Test]
        public void TestSeek()
        {
            using (FileStream fileStream = readFileInfo.OpenRead())
            using (WrapStream wrapStream = new WrapStreamImpl(fileStream))
            {
                Assert.AreEqual(0, fileStream.Position);
                wrapStream.Seek(2, SeekOrigin.Current);
                Assert.AreEqual(2, fileStream.Position);
                wrapStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(0, fileStream.Position);
                wrapStream.Seek(-1, SeekOrigin.End);
                Assert.AreEqual(fileStream.Length - 1, fileStream.Position);
            }
        }
        [Test]
        public void TestDispose()
        {
            DisposeCheckStream disposeCheckStream = new DisposeCheckStream();
            using (WrapStream wrapStream = new WrapStreamImpl(disposeCheckStream, false)) { }
            Assert.IsTrue(disposeCheckStream.Disposed);

            disposeCheckStream = new DisposeCheckStream();
            using (WrapStream wrapStream = new WrapStreamImpl(disposeCheckStream, true)) { }
            Assert.IsFalse(disposeCheckStream.Disposed);
        }
    }
}
