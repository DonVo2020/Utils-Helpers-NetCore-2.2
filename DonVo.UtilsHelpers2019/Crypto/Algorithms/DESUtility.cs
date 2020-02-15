using System;
using System.Security.Cryptography;
using System.Text;

namespace Crypto.Algorithms
{
    public class DESUtility
    {
        private readonly Encoding _encoding;
        private readonly CipherMode _cipherMode;
        private readonly PaddingMode _paddingMode;

        public DESUtility()
        {
            _cipherMode = CipherMode.ECB;
            _paddingMode = PaddingMode.PKCS7;
            _encoding = Encoding.UTF8;
        }

        public DESUtility(CipherMode cipherMode, PaddingMode paddingMode, Encoding encoding)
        {
            _cipherMode = cipherMode;
            _paddingMode = paddingMode;
            _encoding = encoding;
        }

        public string Encrypt(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new Exception("PlainText is null or empty");
            }
            if (string.IsNullOrEmpty(key) || key.Length < 8)
            {
                throw new Exception("The length of the key must be 8 bits");
            }
            key = key.Length > 8 ? key.Substring(0, 8) : key;

            byte[] plainBytes = _encoding.GetBytes(plainText);
            byte[] keyBytes = _encoding.GetBytes(key);

            using (DES des = DES.Create())
            {
                des.Mode = _cipherMode;
                des.Padding = _paddingMode;
                des.Key = keyBytes;

                string cipherText = string.Empty;
                using (ICryptoTransform cryptoTransform = des.CreateEncryptor())
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
            if (string.IsNullOrEmpty(key) || key.Length < 8)
            {
                throw new Exception("The length of the key must be 8 bits");
            }
            if (string.IsNullOrEmpty(vector) || vector.Length < 8)
            {
                throw new Exception("The length of the vector must be 8 bits");
            }
            key = key.Length > 8 ? key.Substring(0, 8) : key;
            vector = vector.Length > 8 ? vector.Substring(0, 8) : vector;

            byte[] plainBytes = _encoding.GetBytes(plainText);
            byte[] keyBytes = _encoding.GetBytes(key);
            byte[] vectorBytes = _encoding.GetBytes(vector);

            using (DES des = DES.Create())
            {
                des.Mode = _cipherMode;
                des.Padding = _paddingMode;

                string cipherText = string.Empty;
                using (ICryptoTransform cryptoTransform = des.CreateEncryptor(keyBytes, vectorBytes))
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
                throw new Exception("PlainText is null or empty");
            }
            if (string.IsNullOrEmpty(key) || key.Length < 8)
            {
                throw new Exception("The length of the key must be 8 bits");
            }
            key = key.Length > 8 ? key.Substring(0, 8) : key;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] keyBytes = _encoding.GetBytes(key);

            using (DES des = DES.Create())
            {
                des.Mode = _cipherMode;
                des.Padding = _paddingMode;
                des.Key = keyBytes;

                string plainText = string.Empty;
                using (ICryptoTransform cryptoTransform = des.CreateDecryptor())
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
                throw new Exception("PlainText is null or empty");
            }
            if (string.IsNullOrEmpty(key) || key.Length < 8)
            {
                throw new Exception("The length of the key must be 8 bits");
            }
            if (string.IsNullOrEmpty(vector) || vector.Length < 8)
            {
                throw new Exception("The length of the vector must be 8 bits");
            }
            key = key.Length > 8 ? key.Substring(0, 8) : key;
            vector = vector.Length > 8 ? vector.Substring(0, 8) : vector;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] keyBytes = _encoding.GetBytes(key);
            byte[] vectorBytes = _encoding.GetBytes(vector);

            using (DES des = DES.Create())
            {
                des.Mode = _cipherMode;
                des.Padding = _paddingMode;

                string plainText = string.Empty;
                using (ICryptoTransform cryptoTransform = des.CreateDecryptor(keyBytes, vectorBytes))
                {
                    byte[] plainBytes = cryptoTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    plainText = _encoding.GetString(plainBytes).Trim('\0');
                }
                return plainText;
            }
        }
    }
}
