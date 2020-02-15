using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Crypto.Streams;
using IO_Utils.Streams;
using NUnit.Framework;

namespace CryptoTests.Streams
{
    [TestFixture("MD5")]
    [TestFixture("SHA1")]
    [TestFixture("SHA256")]
    [TestFixture("SHA384")]
    [TestFixture("SHA512")]
    public class HashStreamTests
    {
        public HashAlgorithmName HashAlgorithmName { get; }

        public HashStreamTests(string hashAlgorithmName)
        {
            HashAlgorithmName = new HashAlgorithmName(hashAlgorithmName);
        }

        [Test]
        public void TestHashCalculation()
        {
            FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);
            byte[] hash;
            using (FileStream fileStream = fileInfo.OpenRead())
            using (HashAlgorithm hashCalculator = HashAlgorithm.Create(HashAlgorithmName.Name))
            {
                hash = hashCalculator.ComputeHash(fileStream);
            }

            HashStream hashStream;
            using (FileStream fileStream = fileInfo.OpenRead())
            using (RandomReadNumberStream randomReadStream = new RandomReadNumberStream(fileStream))
            using (hashStream = new HashStream(randomReadStream, CryptoStreamMode.Read, HashAlgorithmName))
            {
                hashStream.CopyTo(Stream.Null);
            }
            Assert.AreEqual(hash, hashStream.HashResult.HashBytes);

            using (FileStream fileStream = fileInfo.OpenRead())
            using (RandomReadNumberStream randomReadStream = new RandomReadNumberStream(fileStream))
            using (hashStream = new HashStream(Stream.Null, CryptoStreamMode.Write, HashAlgorithmName))
            {
                randomReadStream.CopyTo(hashStream);
            }
            Assert.AreEqual(hash, hashStream.HashResult.HashBytes);
        }
        [Test]
        public async Task TestHashCalculationAsync()
        {
            FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);
            byte[] hash;
            using (FileStream fileStream = fileInfo.OpenRead())
            using (HashAlgorithm hashCalculator = HashAlgorithm.Create(HashAlgorithmName.Name))
            {
                hash = hashCalculator.ComputeHash(fileStream);
            }

            HashStream hashStream;
            using (FileStream fileStream = fileInfo.OpenRead())
            using (RandomReadNumberStream randomReadStream = new RandomReadNumberStream(fileStream))
            using (hashStream = new HashStream(randomReadStream, CryptoStreamMode.Read, HashAlgorithmName))
            {
                await hashStream.CopyToAsync(Stream.Null);
            }
            Assert.AreEqual(hash, hashStream.HashResult.HashBytes);

            using (FileStream fileStream = fileInfo.OpenRead())
            using (RandomReadNumberStream randomReadStream = new RandomReadNumberStream(fileStream))
            using (hashStream = new HashStream(Stream.Null, CryptoStreamMode.Write, HashAlgorithmName))
            {
                await randomReadStream.CopyToAsync(hashStream);
            }
            Assert.AreEqual(hash, hashStream.HashResult.HashBytes);
        }
        [Test]
        public async Task TestNoStreamConstructor()
        {
            FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);
            HashStream hashStream1, hashStream2;

            using (FileStream fileStream = fileInfo.OpenRead())
            using (hashStream1 = new HashStream(fileStream, CryptoStreamMode.Read, HashAlgorithmName))
            using (hashStream2 = new HashStream(HashAlgorithmName))
            {
                await hashStream1.CopyToAsync(Stream.Null);
                fileStream.Seek(0, SeekOrigin.Begin);
                await fileStream.CopyToAsync(hashStream2);
            }

            Assert.AreEqual(hashStream1.HashResult, hashStream2.HashResult);
        }
        [Test]
        public async Task TestNoHashAlgorithmConstructor()
        {
            FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);
            HashStream hashStream1, hashStream2;

            using (FileStream fileStream = fileInfo.OpenRead())
            using (hashStream1 = new HashStream(HashAlgorithmName, HashAlgorithm.Create(HashAlgorithmName.Name)))
            using (hashStream2 = new HashStream(HashAlgorithmName))
            {
                await fileStream.CopyToAsync(hashStream1);
                fileStream.Seek(0, SeekOrigin.Begin);
                await fileStream.CopyToAsync(hashStream2);
            }

            Assert.AreEqual(hashStream1.HashResult, hashStream2.HashResult);
        }
    }
}
