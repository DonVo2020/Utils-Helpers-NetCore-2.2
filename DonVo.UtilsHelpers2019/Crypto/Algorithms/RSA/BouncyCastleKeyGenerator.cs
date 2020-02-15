using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Crypto.Algorithms.RSA
{
    public class BouncyCastleKeyGenerator : ICryptoGenerator
    {
        private const string Algorithm = "RSA";
        private readonly IAsymmetricCipherKeyPairGenerator _keyPairGenerator;

        public BouncyCastleKeyGenerator() : this(4096) { }

        public BouncyCastleKeyGenerator(int bits)
        {
            _keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator(Algorithm);
            _keyPairGenerator.Init(
                new RsaKeyGenerationParameters(
                    BigInteger.ValueOf(17),
                    new SecureRandom(),
                    bits,
                    25));
        }

        public Tuple<string, string> GenerateKeyPair()
        {
            var keys = _keyPairGenerator.GenerateKeyPair();
            return new Tuple<string, string>(PublicConvert(keys.Public), PrivateConvert(keys.Private));
        }

        private string PublicConvert(AsymmetricKeyParameter key)
        {
            var privateKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key);
            var serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();

            return Convert.ToBase64String(serializedPrivateBytes);
        }

        private string PrivateConvert(AsymmetricKeyParameter key)
        {
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(key);
            var serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();

            return Convert.ToBase64String(serializedPrivateBytes);
        }

    }
}
