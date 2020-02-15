using Crypto.Algorithms;
using Crypto.Algorithms.MD5Utils;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;

namespace Tests
{
    public class AlgorithmsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("123-45-6789", "TSAC!9ANISHhi%XjjYY4YP2@Nob009Y")]
        [TestCase("123456789", "TSAC!9ANISHhi%XjjYY4YP2@Nob009Y")]
        public void TestMD5(string ssn, string key)
        {
            var cipherText = MD5Utility.Encrypt(ssn, key);
            Assert.AreEqual(ssn, MD5Utility.Decrypt(cipherText, key));
        }

        [TestCase(PaddingMode.PKCS7, "TSAC!9ANISHhi%Xj", "123456789")]
        [TestCase(PaddingMode.Zeros, "abCe6PBNNdyPhdVF", "123-45-6789")]
        public void TestAES(PaddingMode paddingMode, string key, string plainText)
        {
            AESUtility aesUtility = new AESUtility(CipherMode.ECB, paddingMode, Encoding.UTF8);
            var cipherText = aesUtility.Encrypt(plainText, key);
            Assert.AreEqual(plainText, aesUtility.Decrypt(cipherText, key));
        }

        [TestCase(PaddingMode.PKCS7, "TSAC!9ANISHhi%Xj", "123GgSKNRaBItkWY", "123456789")]
        [TestCase(PaddingMode.Zeros, "abCe6PBNNdyPhdVF", "ABCGgSKNRaBItkWY", "123-45-6789")]
        public void TestAESWithVector(PaddingMode paddingMode, string key, string vector, string plainText)
        {
            AESUtility aesUtility = new AESUtility(CipherMode.CBC, paddingMode, Encoding.UTF8);
            var cipherText = aesUtility.Encrypt(plainText, key, vector);
            Assert.AreEqual(plainText, aesUtility.Decrypt(cipherText, key, vector));
        }

        [TestCase(PaddingMode.PKCS7, "TSAC!9ANISHhi%Xj", "123456789")]
        [TestCase(PaddingMode.Zeros, "abCe6PBNNdyPhdVF", "123-45-6789")]
        public void TestDES(PaddingMode paddingMode, string key, string plainText)
        {
            DESUtility desUtility = new DESUtility(CipherMode.ECB, paddingMode, Encoding.UTF8);
            var cipherText = desUtility.Encrypt(plainText, key);
            Assert.AreEqual(plainText, desUtility.Decrypt(cipherText, key));
        }

        [TestCase(PaddingMode.PKCS7, "TSAC!9ANISHhi%Xj", "123GgSKNRaBItkWY", "123456789")]
        [TestCase(PaddingMode.Zeros, "abCe6PBNNdyPhdVF", "ABCGgSKNRaBItkWY", "123-45-6789")]
        public void TestDESWithVector(PaddingMode paddingMode, string key, string vector, string plainText)
        {
            DESUtility desUtility = new DESUtility(CipherMode.CBC, paddingMode, Encoding.UTF8);
            var cipherText = desUtility.Encrypt(plainText, key, vector);
            Assert.AreEqual(plainText, desUtility.Decrypt(cipherText, key, vector));
        }

        [TestCase(PaddingMode.PKCS7, "TSAC!9ANISHhi%Xj12345678", "123456789")]
        [TestCase(PaddingMode.Zeros, "123IxIwx49nFGdR6cXwu5FQQ", "123-45-6789")]
        public void TestTripleDES(PaddingMode paddingMode, string key, string plainText)
        {
            TripleDESUtility tripleDESUtility = new TripleDESUtility(CipherMode.ECB, paddingMode, Encoding.UTF8);
            var cipherText = tripleDESUtility.Encrypt(plainText, key);
            Assert.AreEqual(plainText, tripleDESUtility.Decrypt(cipherText, key));
        }

        [TestCase(PaddingMode.PKCS7, "TSAC!9ANISHhi%Xj12345678", "123GgSKNRaBItkWY", "123456789")]
        [TestCase(PaddingMode.Zeros, "123IxIwx49nFGdR6cXwu5FQQ", "ABCGgSKNRaBItkWY", "123-45-6789")]
        public void TestTripleDESWithVector(PaddingMode paddingMode, string key, string vector, string plainText)
        {
            TripleDESUtility tripleDESUtility = new TripleDESUtility(CipherMode.CBC, paddingMode, Encoding.UTF8);
            var cipherText = tripleDESUtility.Encrypt(plainText, key, vector);
            Assert.AreEqual(plainText, tripleDESUtility.Decrypt(cipherText, key, vector));
        }
    }
}
