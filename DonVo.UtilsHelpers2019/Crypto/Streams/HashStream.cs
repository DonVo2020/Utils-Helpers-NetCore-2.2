using System.IO;
using System.Security.Cryptography;

namespace Crypto.Streams
{
    public class HashStream : CryptoStream
    {
        public HashResult HashResult => new HashResult(HashAlgorithmName, hashCalculator.Hash);

        public HashAlgorithmName HashAlgorithmName { get; }
        private readonly HashAlgorithm hashCalculator;

        public HashStream(Stream stream, CryptoStreamMode mode, HashAlgorithmName hashAlgorithmName, HashAlgorithm hashCalculator = null)
            : base(stream, hashCalculator ?? (hashCalculator = HashAlgorithm.Create(hashAlgorithmName.Name)), mode)
        {
            HashAlgorithmName = hashAlgorithmName;
            this.hashCalculator = hashCalculator;
        }
        public HashStream(HashAlgorithmName hashAlgorithmName, HashAlgorithm hashCalculator = null)
            : this(Stream.Null, CryptoStreamMode.Write, hashAlgorithmName, hashCalculator) { }
    }
}
