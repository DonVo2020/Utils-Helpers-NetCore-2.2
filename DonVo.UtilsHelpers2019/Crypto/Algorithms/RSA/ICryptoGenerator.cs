using System;

namespace Crypto.Algorithms.RSA
{
    public interface ICryptoGenerator
    {
        Tuple<string, string> GenerateKeyPair();
    }
}
