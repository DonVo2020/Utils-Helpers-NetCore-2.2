using Crypto;
using Crypto.Streams;
using CryptoTests.TestUtils;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    internal class HashResultTests
    {
        private static IEnumerable<IFormatter> GetFormatters()
        {
            return new IFormatter[] { new JsonFormatter(), new BinaryFormatter() };
        }


        [Test]
        [TestCaseSource(nameof(GetFormatters))]
        public async Task TestSerialization(IFormatter formatter)
        {
            HashResult hashResult = await GetExampleHashResult();

            using (Stream stream = new MemoryStream())
            {
                formatter.Serialize(stream, hashResult);
                stream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(hashResult, formatter.Deserialize(stream));
            }
        }

        private async Task<HashResult> GetExampleHashResult()
        {
            FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);
            HashStream hashStream;
            using (FileStream fileStream = fileInfo.OpenRead())
            using (hashStream = new HashStream(HashAlgorithmName.SHA256))
            {
                await fileStream.CopyToAsync(hashStream);
            }
            hashStream.HashResult.ToString();
            return hashStream.HashResult;
        }
    }
}