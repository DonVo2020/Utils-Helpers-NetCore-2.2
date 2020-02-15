using System;
using System.Security.Cryptography;
using System.Text;

namespace Crypto.Algorithms
{
    public class AESUtility
    {
        private readonly Encoding _encoding;
        private readonly int _keySize;

        private readonly CipherMode _cipherMode;
        private readonly PaddingMode _paddingMode;

        public AESUtility()
        {
            _cipherMode = CipherMode.ECB;
            _paddingMode = PaddingMode.PKCS7;
            _keySize = 128;
            _encoding = Encoding.UTF8;
        }
        public AESUtility(CipherMode cipherMode, PaddingMode paddingMode, Encoding encoding)
        {
            _cipherMode = cipherMode;
            _paddingMode = paddingMode;
            _keySize = 128;
            _encoding = encoding;
        }

        public string Encrypt(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new Exception("PlainText is null or empty");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key is null or empty");
            }

            byte[] plainBytes = _encoding.GetBytes(plainText);
            byte[] keyBytes = _encoding.GetBytes(key);

            using (Aes aes = Aes.Create())
            {
                aes.Mode = _cipherMode;
                aes.Padding = _paddingMode;
                aes.KeySize = _keySize;
                aes.Key = keyBytes;

                string cipherText = string.Empty;
                using (ICryptoTransform cryptoTransform = aes.CreateEncryptor())
                {
                    byte[] cipherBytes = cryptoTransform.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    cipherText = Convert.ToBase64String(cipherBytes);
                }
                return cipherText;
            }
        }
        public string Encrypt(string plainText, string key, string vector)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new Exception("PlainText is null or empty");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key is null or empty");
            }

            byte[] plainBytes = _encoding.GetBytes(plainText);
            byte[] keyBytes = _encoding.GetBytes(key);
            byte[] vectorBytes = _encoding.GetBytes(vector);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = _keySize;
                aes.Mode = _cipherMode;
                aes.Padding = _paddingMode;

                string cipherText = string.Empty;
                using (ICryptoTransform cryptoTransform = aes.CreateEncryptor(keyBytes, vectorBytes))
                {
                    byte[] cipherBytes = cryptoTransform.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    cipherText = Convert.ToBase64String(cipherBytes);
                }
                return cipherText;
            }
        }
        public string Decrypt(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new Exception("CipherText is null or empty");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key is null or empty");
            }

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] keyBytes = _encoding.GetBytes(key);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = _keySize;
                aes.Key = keyBytes;
                aes.Mode = _cipherMode;
                aes.Padding = _paddingMode;

                string plainText = string.Empty;
                using (ICryptoTransform cryptoTransform = aes.CreateDecryptor())
                {
                    byte[] plainBytes = cryptoTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    plainText = _encoding.GetString(plainBytes).Trim('\0');
                }
                return plainText;
            }
        }
        public string Decrypt(string cipherText, string key, string vector)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new Exception("CipherText is null or empty");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key is null or empty");
            }

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] keyBytes = _encoding.GetBytes(key);
            byte[] vectorBytes = _encoding.GetBytes(vector);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = _keySize;
                aes.Mode = _cipherMode;
                aes.Padding = _paddingMode;

                string plainText = string.Empty;
                using (ICryptoTransform cryptoTransform = aes.CreateDecryptor(keyBytes, vectorBytes))
                {
                    byte[] plainBytes = cryptoTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    plainText = _encoding.GetString(plainBytes).Trim('\0');
                }
                return plainText;
            }
        }
    }
}
