using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Crypto
{
    [Serializable]
    public class HashResult : IEquatable<HashResult>, IComparable<HashResult>, IComparable, ISerializable
    {
        #region Operators
        public static bool operator ==(HashResult a, HashResult b) => a.Equals(b);
        public static bool operator !=(HashResult a, HashResult b) => !a.Equals(b);
        public static explicit operator byte[](HashResult hashResult) => hashResult.HashBytes;
        public static explicit operator string(HashResult hashResult) => hashResult.HashString;
        public static implicit operator HashResult(byte[] hashBytes)
        {
            //TODO: discover the hash type by the string length
            return new HashResult(default, hashBytes);
        }
        #endregion


        private string _hashString;

        public HashAlgorithmName HashAlgorithmName { get; }
        public byte[] HashBytes { get; }
        public string HashString
        {
            get
            {
                if (_hashString != null)
                {
                    return _hashString;
                }

                return _hashString = string.Concat(HashBytes.Select(b => b.ToString("X2")));
            }
        }

        public HashResult(HashAlgorithmName hashAlgorithmName, byte[] hashBytes)
        {
            HashAlgorithmName = hashAlgorithmName;
            HashBytes = hashBytes;
        }
        public HashResult(SerializationInfo info, StreamingContext context)
        {
            HashAlgorithmName = new HashAlgorithmName(info.GetString(nameof(HashAlgorithmName)));
            HashBytes = (byte[])info.GetValue(nameof(HashBytes), typeof(byte[]));
        }

        public override string ToString()
        {
            return HashString;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(HashAlgorithmName), HashAlgorithmName.Name);
            info.AddValue(nameof(HashBytes), HashBytes);
        }

        #region Operators Methods
        public bool Equals(HashResult other)
        {
            if (other is null)
            {
                return false;
            }

            return HashAlgorithmName.Equals(other.HashAlgorithmName) && HashBytes.SequenceEqual(other.HashBytes);
        }
        public override bool Equals(object obj)
        {
            return obj is HashResult otherHashResult && Equals(otherHashResult);
        }
        public int CompareTo(HashResult other)
        {
            if (HashAlgorithmName != other.HashAlgorithmName)
            {
                return HashAlgorithmName.Name.CompareTo(other.HashAlgorithmName.Name);
            }

            if (HashBytes.Length != other.HashBytes.Length)
            {
                return HashBytes.Length.CompareTo(other.HashBytes.Length);
            }

            for (int i = 0; i < HashBytes.Length; i++)
            {
                if (HashBytes[i] != other.HashBytes[i])
                {
                    return HashBytes[i].CompareTo(other.HashBytes[i]);
                }
            }

            return 0;
        }
        public int CompareTo(object obj)
        {
            HashResult otherHashResult = obj as HashResult;
            return otherHashResult != null ? CompareTo(otherHashResult) : 1;
        }

        public override int GetHashCode()
        {
            return 175587528 + HashAlgorithmName.GetHashCode() + HashBytes.GetHashCode();
        }
        #endregion
    }
}
