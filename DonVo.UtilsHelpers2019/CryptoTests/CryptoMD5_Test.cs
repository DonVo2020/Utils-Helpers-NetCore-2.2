using Crypto.Algorithms.MD5Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoTests
{
    [TestClass]
    public class CryptoMD5_Test
    {
        #region Variables
        private CryptoMD5 _crypto
        {
            get
            {
                var key = "â'SETFDgdæß2^!'Sdfg½{$£";
                var ivSeed = "03c5dacc-1756-4cec-be29-fd691fbc0a95";
                return new CryptoMD5(key, ivSeed);
            }
        }
        #endregion

        [TestMethod]
        public void EncryptionAndDecryption()
        {
            var text = "Hello, my name is Muhammed Kandemir.";

            var encryptedBase64 = this._crypto.EncryptionAsStringToBase64(text);
            Assert.AreNotEqual(encryptedBase64, text);
            var encryptedBase64Again = this._crypto.EncryptionAsStringToBase64(text);
            Assert.AreEqual(encryptedBase64Again, encryptedBase64);

            var decryptedText = this._crypto.DecryptionAsStringFromBase64(encryptedBase64);
            Assert.AreEqual(decryptedText, text);
            var decryptedTextAgain = this._crypto.DecryptionAsStringFromBase64(encryptedBase64);
            Assert.AreEqual(decryptedTextAgain, decryptedText);
        }

        [TestMethod]
        public void Md5()
        {
            string text = "Hello, my name is Muhammed Kandemir.";

            var md5 = this._crypto.HashMD5(text);
            var md5Again = this._crypto.HashMD5(text);
            using (var hashGenerator = MD5.Create())
            {
                var result = string.Join("", hashGenerator.ComputeHash(Encoding.UTF8.GetBytes(text)).Select(x => x.ToString("X2")).ToArray());
                Assert.AreEqual(result.Length, md5.Length);
                Assert.AreNotEqual(result, md5);
                Assert.AreEqual(md5Again, md5);
            }
        }

        #region Model Test
        public class TestCryptoModel : CryptoModel<TestCryptoModel>
        {
            public TestCryptoModel(CryptoMD5 crypto)
            {
                this.Crypto = crypto;
            }

            [CryptoModelKey]
            public string Id { get; set; }
            [CryptoModelKey]
            public string[] SubIds { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
        }

        [TestMethod]
        public void Model()
        {
            var id = "123abc456dfe";
            var subIds = new string[]
            {
                "subId1",
                "subId2",
                "subId3"
            };
            var name = "Muhammed";
            var surname = "Kandemir";

            var model = new TestCryptoModel(this._crypto)
            {
                Id = id,
                SubIds = new string[]
                {
                      subIds[0],
                      subIds[1],
                      subIds[2]
                },
                Name = name,
                Surname = surname
            };

            model.Encryption();

            Assert.AreNotEqual(model.Id, id);
            Assert.AreNotEqual(model.SubIds[0], subIds[0]);
            Assert.AreNotEqual(model.SubIds[1], subIds[1]);
            Assert.AreNotEqual(model.SubIds[2], subIds[2]);
            Assert.AreEqual(model.Name, name);
            Assert.AreEqual(model.Surname, surname);

            model.Decryption();

            Assert.AreEqual(model.Id, id);
            Assert.AreEqual(model.SubIds[0], subIds[0]);
            Assert.AreEqual(model.SubIds[1], subIds[1]);
            Assert.AreEqual(model.SubIds[2], subIds[2]);
            Assert.AreEqual(model.Name, name);
            Assert.AreEqual(model.Surname, surname);
        }
        #endregion
    }
}
