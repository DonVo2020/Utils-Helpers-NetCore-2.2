using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Crypto.Algorithms.MD5Utils
{
    public class CryptoMD5
    {
        #region Variables
        private MD5 _hashGenerator = null;
        private ICryptoTransform _encryptor = null;
        private ICryptoTransform _decryptor = null;

        private byte[] _ivSeed = null;
        public string IvSeedGuid
        {
            get
            {
                return new Guid(this._ivSeed).ToString();
            }
            set
            {
                this._ivSeed = new Guid(value).ToByteArray();
                createEncryptor();
            }
        }

        private string _key = null;
        public string Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
                createEncryptor();
            }
        }
        #endregion

        #region Constructors
        public CryptoMD5(string key, string ivSeedGuid)
        {
            this.Key = key;
            this.IvSeedGuid = ivSeedGuid;
            this._hashGenerator = MD5.Create();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creating encyption variables
        /// </summary>
        private void createEncryptor()
        {
            if (this._ivSeed == null || this._key == null)
                return;

            var rfc = new Rfc2898DeriveBytes(this._key, this._ivSeed);
            byte[] rfcKey = rfc.GetBytes(16);
            byte[] rfcIV = rfc.GetBytes(16);

            var aesProvider = new AesCryptoServiceProvider();
            this._encryptor = aesProvider.CreateEncryptor(rfcKey, rfcIV);
            aesProvider.Padding = PaddingMode.None;
            this._decryptor = aesProvider.CreateDecryptor(rfcKey, rfcIV);
        }

        /// <summary>
        /// Get MD5 after encryption with key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public string HashMD5(string text)
        {
            var result = this._hashGenerator.ComputeHash(EncryptionAsString(text));
            return string.Join("", result.Select(x => x.ToString("X2")).ToArray());
        }

        /// <summary>
        /// Encryption byte array via key to byte array.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Encryption(byte[] data)
        {
            byte[] encrypted;
            using (MemoryStream mstream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(mstream, _encryptor, CryptoStreamMode.Write))
                    cryptoStream.Write(data, 0, data.Length);
                encrypted = mstream.ToArray();
            }

            var messageLengthAs32Bits = Convert.ToInt32(data.Length);
            var messageLength = BitConverter.GetBytes(messageLengthAs32Bits);

            encrypted = prepend(encrypted, this._ivSeed);
            encrypted = prepend(encrypted, messageLength);

            return encrypted;
        }

        /// <summary>
        /// Encryption string to byte array.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public byte[] EncryptionAsString(string text)
        {
            return Encryption(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// Encryption string to base64 string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public string EncryptionAsStringToBase64(string text)
        {
            return Convert.ToBase64String(EncryptionAsString(text));
        }

        /// <summary>
        /// Decryption byte array to byte array.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Decryption(byte[] data)
        {
            (byte[] messageLengthAs32Bits, byte[] bytesWithIv) = shift(data, 4); // get the message length
            (byte[] ivSeed, byte[] encrypted) = shift(bytesWithIv, 16); // get the initialization vector

            var length = BitConverter.ToInt32(messageLengthAs32Bits, 0);

            byte[] decrypted;
            using (MemoryStream mStream = new MemoryStream(encrypted))
            {
                using (CryptoStream cryptoStream = new CryptoStream(mStream, this._decryptor, CryptoStreamMode.Read))
                    cryptoStream.Read(encrypted, 0, length);
                decrypted = mStream.ToArray().Take(length).ToArray();
            }
            return decrypted;
        }

        /// <summary>
        /// Decryption byte array to string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string DecryptionAsString(byte[] data)
        {
            return Encoding.UTF8.GetString(Decryption(data));
        }

        /// <summary>
        /// Decryption base64 string to string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="base64"></param>
        /// <returns></returns>
        public string DecryptionAsStringFromBase64(string base64)
        {
            return Encoding.UTF8.GetString(Decryption(Convert.FromBase64String(base64)));
        }

        private byte[] prepend(byte[] bytes, byte[] bytesToPrepend)
        {
            var tmp = new byte[bytes.Length + bytesToPrepend.Length];
            bytesToPrepend.CopyTo(tmp, 0);
            bytes.CopyTo(tmp, bytesToPrepend.Length);
            return tmp;
        }

        private (byte[] left, byte[] right) shift(byte[] bytes, int size)
        {
            var left = new byte[size];
            var right = new byte[bytes.Length - size];

            Array.Copy(bytes, 0, left, 0, left.Length);
            Array.Copy(bytes, left.Length, right, 0, right.Length);

            return (left, right);
        }
        #endregion
    }
}
