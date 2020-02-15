using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Crypto;
using Crypto.Streams;
using IO_Utils.Streams;
using NUnit.Framework;

namespace CryptoTests.Streams
{
    [TestFixture]
    internal class MultipleHashStreamTests
    {
        private readonly ICollection<(HashAlgorithmName, Func<HashAlgorithm>)> hashAlgorithms =
            new (HashAlgorithmName, Func<HashAlgorithm>)[]
            {
                (HashAlgorithmName.MD5, ()=>HashAlgorithm.Create(HashAlgorithmName.MD5.Name)),
                (HashAlgorithmName.SHA1, ()=>HashAlgorithm.Create(HashAlgorithmName.SHA1.Name)),
                (HashAlgorithmName.SHA256, ()=>HashAlgorithm.Create(HashAlgorithmName.SHA256.Name)),
                (HashAlgorithmName.SHA384, ()=>HashAlgorithm.Create(HashAlgorithmName.SHA384.Name)),
                (HashAlgorithmName.SHA512, ()=>HashAlgorithm.Create(HashAlgorithmName.SHA512.Name))
            };

        [Test]
        public void TestHashCalculationWithName()
        {
            Dictionary<HashAlgorithmName, byte[]> realHashes = new Dictionary<HashAlgorithmName, byte[]>();
            FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);

            using (FileStream fileStream = fileInfo.OpenRead())
            using (RandomReadNumberStream randomReadStream = new RandomReadNumberStream(fileStream))
            {
                foreach ((HashAlgorithmName hashAlgorithmName, Func<HashAlgorithm> hashAlgorithmProvider) in hashAlgorithms)
                {
                    using (HashAlgorithm hashCalculator = hashAlgorithmProvider())
                    {
                        realHashes.Add(hashAlgorithmName, hashCalculator.ComputeHash(randomReadStream));
                    }

                    randomReadStream.Seek(0, SeekOrigin.Begin);
                }

                MultipleHashStream multipleHashStream;
                using (multipleHashStream = new MultipleHashStream(randomReadStream, hashAlgorithms.Select((ha) => ha.Item1), ReadWriteMode.Read))
                {
                    multipleHashStream.CopyTo(Stream.Null);
                }

                foreach (HashAlgorithmName hashAlgorithmName in hashAlgorithms.Select((ha) => ha.Item1))
                {
                    Assert.AreEqual(realHashes[hashAlgorithmName],
                        multipleHashStream.GetHashResult(hashAlgorithmName, ReadWriteMode.Read).HashBytes,
                        $"Hash {hashAlgorithmName.Name} is not the same at {nameof(multipleHashStream.GetHashResult)}");
                    Assert.AreEqual(realHashes[hashAlgorithmName],
                        multipleHashStream.GetAllHashResults(ReadWriteMode.Read).First((hr) => hr.HashAlgorithmName == hashAlgorithmName).HashBytes,
                        $"Hash {hashAlgorithmName.Name} is not the same at {nameof(multipleHashStream.GetAllHashResults)}");
                }
            }
        }
        [Test]
        public void TestHashCalculationWithProvider()
        {
            Dictionary<HashAlgorithmName, byte[]> realHashes = new Dictionary<HashAlgorithmName, byte[]>();
            FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);

            using (FileStream fileStream = fileInfo.OpenRead())
            using (RandomReadNumberStream randomReadStream = new RandomReadNumberStream(fileStream))
            {
                foreach ((HashAlgorithmName hashAlgorithmName, Func<HashAlgorithm> hashAlgorithmProvider) in hashAlgorithms)
                {
                    using (HashAlgorithm hashCalculator = hashAlgorithmProvider())
                    {
                        realHashes.Add(hashAlgorithmName, hashCalculator.ComputeHash(randomReadStream));
                    }

                    randomReadStream.Seek(0, SeekOrigin.Begin);
                }

                MultipleHashStream multipleHashStream;
                using (multipleHashStream = new MultipleHashStream(randomReadStream, hashAlgorithms, ReadWriteMode.Read))
                {
                    multipleHashStream.CopyTo(Stream.Null);
                }

                foreach (HashAlgorithmName hashAlgorithmName in hashAlgorithms.Select((ha) => ha.Item1))
                {
                    Assert.AreEqual(realHashes[hashAlgorithmName],
                        multipleHashStream.GetHashResult(hashAlgorithmName, ReadWriteMode.Read).HashBytes,
                        $"Hash {hashAlgorithmName.Name} is not the same at {nameof(multipleHashStream.GetHashResult)}");
                    Assert.AreEqual(realHashes[hashAlgorithmName],
                        multipleHashStream.GetAllHashResults(ReadWriteMode.Read).First((hr) => hr.HashAlgorithmName == hashAlgorithmName).HashBytes,
                        $"Hash {hashAlgorithmName.Name} is not the same at {nameof(multipleHashStream.GetAllHashResults)}");
                }
            }
        }
        [Test]
        public async Task TestHashCalculationWithNameAsync()
        {
            Dictionary<HashAlgorithmName, byte[]> realHashes = new Dictionary<HashAlgorithmName, byte[]>();
            FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);

            using (FileStream fileStream = fileInfo.OpenRead())
            using (RandomReadNumberStream randomReadStream = new RandomReadNumberStream(fileStream))
            {
                foreach ((HashAlgorithmName hashAlgorithmName, Func<HashAlgorithm> hashAlgorithmProvider) in hashAlgorithms)
                {
                    using (HashAlgorithm hashCalculator = hashAlgorithmProvider())
                    {
                        realHashes.Add(hashAlgorithmName, hashCalculator.ComputeHash(randomReadStream));
                    }

                    randomReadStream.Seek(0, SeekOrigin.Begin);
                }

                MultipleHashStream multipleHashStream;
                using (multipleHashStream = new MultipleHashStream(randomReadStream, hashAlgorithms.Select((ha) => ha.Item1), ReadWriteMode.Read))
                {
                    await multipleHashStream.CopyToAsync(Stream.Null);
                }

                foreach (HashAlgorithmName hashAlgorithmName in hashAlgorithms.Select((ha) => ha.Item1))
                {
                    Assert.AreEqual(realHashes[hashAlgorithmName],
                        multipleHashStream.GetHashResult(hashAlgorithmName, ReadWriteMode.Read).HashBytes,
                        $"Hash {hashAlgorithmName.Name} is not the same at {nameof(multipleHashStream.GetHashResult)}");
                    Assert.AreEqual(realHashes[hashAlgorithmName],
                        multipleHashStream.GetAllHashResults(ReadWriteMode.Read).First((hr) => hr.HashAlgorithmName == hashAlgorithmName).HashBytes,
                        $"Hash {hashAlgorithmName.Name} is not the same at {nameof(multipleHashStream.GetAllHashResults)}");
                }
            }
        }
        [Test]
        public async Task TestHashCalculationWithProviderAsync()
        {
            Dictionary<HashAlgorithmName, byte[]> realHashes = new Dictionary<HashAlgorithmName, byte[]>();
            FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);

            using (FileStream fileStream = fileInfo.OpenRead())
            using (RandomReadNumberStream randomReadStream = new RandomReadNumberStream(fileStream))
            {
                foreach ((HashAlgorithmName hashAlgorithmName, Func<HashAlgorithm> hashAlgorithmProvider) in hashAlgorithms)
                {
                    using (HashAlgorithm hashCalculator = hashAlgorithmProvider())
                    {
                        realHashes.Add(hashAlgorithmName, hashCalculator.ComputeHash(randomReadStream));
                    }

                    randomReadStream.Seek(0, SeekOrigin.Begin);
                }

                MultipleHashStream multipleHashStream;
                using (multipleHashStream = new MultipleHashStream(randomReadStream, hashAlgorithms, ReadWriteMode.Read))
                {
                    await multipleHashStream.CopyToAsync(Stream.Null);
                }

                foreach (HashAlgorithmName hashAlgorithmName in hashAlgorithms.Select((ha) => ha.Item1))
                {
                    Assert.AreEqual(realHashes[hashAlgorithmName],
                        multipleHashStream.GetHashResult(hashAlgorithmName, ReadWriteMode.Read).HashBytes,
                        $"Hash {hashAlgorithmName.Name} is not the same at {nameof(multipleHashStream.GetHashResult)}");
                    Assert.AreEqual(realHashes[hashAlgorithmName],
                        multipleHashStream.GetAllHashResults(ReadWriteMode.Read).First((hr) => hr.HashAlgorithmName == hashAlgorithmName).HashBytes,
                        $"Hash {hashAlgorithmName.Name} is not the same at {nameof(multipleHashStream.GetAllHashResults)}");
                }
            }
        }
        [Test]
        public void TestReadWriteModeValidation()
        {
            byte[] buffer = Array.Empty<byte>();
            using (MemoryStream memoryStream = new MemoryStream(buffer, false))
            {
                using (MultipleHashStream multipleHashStream = new MultipleHashStream(memoryStream, hashAlgorithms, ReadWriteMode.Read, leaveOpen: true))
                {
                    Assert.AreEqual(ReadWriteMode.Read, multipleHashStream.ReadWriteMode);
                }

                using (MultipleHashStream multipleHashStream = new MultipleHashStream(memoryStream, hashAlgorithms, ReadWriteMode.ReadWrite, leaveOpen: true))
                {
                    Assert.AreEqual(ReadWriteMode.Read, multipleHashStream.ReadWriteMode);
                }

                Assert.Throws<ArgumentException>(() => new MultipleHashStream(memoryStream, hashAlgorithms, ReadWriteMode.Write, leaveOpen: true));
            }

            using (HashStream hashStream = new HashStream(HashAlgorithmName.MD5))
            {
                using (MultipleHashStream multipleHashStream = new MultipleHashStream(hashStream, hashAlgorithms, ReadWriteMode.Write, leaveOpen: true))
                {
                    Assert.AreEqual(ReadWriteMode.Write, multipleHashStream.ReadWriteMode);
                }

                using (MultipleHashStream multipleHashStream = new MultipleHashStream(hashStream, hashAlgorithms, ReadWriteMode.ReadWrite, leaveOpen: true))
                {
                    Assert.AreEqual(ReadWriteMode.Write, multipleHashStream.ReadWriteMode);
                }

                Assert.Throws<ArgumentException>(() => new MultipleHashStream(hashStream, hashAlgorithms, ReadWriteMode.Read, leaveOpen: true));
            }
        }
    }
}
