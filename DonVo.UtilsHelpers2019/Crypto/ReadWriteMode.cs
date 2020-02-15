using System;

namespace Crypto
{
    [Flags]
    public enum ReadWriteMode
    {
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write
    }
}
