using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Crypto.Streams
{
    public class MultipleHashStream : Stream
    {
        public override bool CanRead => baseStream.CanRead;
        public override bool CanWrite => baseStream.CanWrite;
        public override bool CanSeek => false;
        public override bool CanTimeout => baseStream.CanTimeout;
        public override long Length => baseStream.Length;
        public override long Position { get => baseStream.Position; set => baseStream.Position = value; }
        public override int ReadTimeout { get => baseStream.ReadTimeout; set => baseStream.ReadTimeout = value; }
        public override int WriteTimeout { get => baseStream.WriteTimeout; set => baseStream.WriteTimeout = value; }
        public bool HasFlushedFinalBlock => readHashCalculators.Values.All(hashCalculator => hashCalculator.HasFlushedFinalBlock) &&
                    writeHashCalculators.Values.All(hashCalculator => hashCalculator.HasFlushedFinalBlock);
        public ReadWriteMode ReadWriteMode { get; }

        private bool disposed = false;
        private readonly bool leaveOpen;
        private readonly Stream baseStream;
        private readonly IDictionary<HashAlgorithmName, HashStream> readHashCalculators;
        private readonly IDictionary<HashAlgorithmName, HashStream> writeHashCalculators;

        public MultipleHashStream(Stream baseStream, IEnumerable<(HashAlgorithmName hashAlgorithmName, Func<HashAlgorithm> hashAlgorithmProvider)> hashAlgorithms, ReadWriteMode readWriteMode = ReadWriteMode.ReadWrite, bool leaveOpen = false)
        {
            this.baseStream = baseStream;
            this.leaveOpen = leaveOpen;
            ReadWriteMode = ValidateReadWriteMode(readWriteMode);

            if (ReadWriteMode.HasFlag(ReadWriteMode.Read))
            {
                readHashCalculators = new Dictionary<HashAlgorithmName, HashStream>();
            }
            if (ReadWriteMode.HasFlag(ReadWriteMode.Write))
            {
                writeHashCalculators = new Dictionary<HashAlgorithmName, HashStream>();
            }
            foreach ((HashAlgorithmName hashAlgorithmName, Func<HashAlgorithm> hashAlgorithmProvider) in hashAlgorithms)
            {
                if (readHashCalculators != null)
                {
                    readHashCalculators.Add(hashAlgorithmName, new HashStream(hashAlgorithmName, hashAlgorithmProvider?.Invoke()));
                }
                if (writeHashCalculators != null)
                {
                    writeHashCalculators.Add(hashAlgorithmName, new HashStream(hashAlgorithmName, hashAlgorithmProvider?.Invoke()));
                }
            }
        }
        public MultipleHashStream(Stream baseStream, IEnumerable<HashAlgorithmName> hashAlgorithms, ReadWriteMode readWriteMode = ReadWriteMode.ReadWrite, bool leaveOpen = false)
            : this(baseStream,
                  hashAlgorithms.Select<HashAlgorithmName, (HashAlgorithmName hashAlgorithmName, Func<HashAlgorithm> hashAlgorithmProvider)>((h) => (h, null)),
                  readWriteMode,
                  leaveOpen)
        { }

        public override void Flush() => baseStream.Flush();
        public override Task FlushAsync(CancellationToken cancellationToken) => baseStream.FlushAsync(cancellationToken);
        public override void SetLength(long value) => baseStream.SetLength(value);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public void FlushFinalBlock()
        {
            if (readHashCalculators != null)
            {
                foreach (HashStream hashCalculator in readHashCalculators.Values)
                {
                    hashCalculator.FlushFinalBlock();
                }
            }
            if (writeHashCalculators != null)
            {
                foreach (HashStream hashCalculator in writeHashCalculators.Values)
                {
                    hashCalculator.FlushFinalBlock();
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count).Result;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).Wait();
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int readedBytes = await baseStream.ReadAsync(buffer, offset, count, cancellationToken);

            if (readHashCalculators != null && readedBytes > 0)
            {
                await Task.WhenAll(readHashCalculators.Values.Select(hashCalculator => hashCalculator.WriteAsync(buffer, offset, readedBytes)).ToList());
            }

            return readedBytes;
        }
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Task baseStreamWriteTask = baseStream.WriteAsync(buffer, offset, count, cancellationToken);
            await baseStreamWriteTask;

            if (writeHashCalculators != null && !baseStreamWriteTask.IsCanceled)
            {
                await Task.WhenAll(writeHashCalculators.Values.Select(hashCalculator => hashCalculator.WriteAsync(buffer, offset, count)).ToList());
            }
        }

        public HashResult GetHashResult(HashAlgorithmName hashAlgorithmName, ReadWriteMode readWriteMode)
        {
            if (readWriteMode == ReadWriteMode.ReadWrite)
            {
                if (ReadWriteMode.HasFlag(ReadWriteMode.Read) && !ReadWriteMode.HasFlag(ReadWriteMode.Write))
                {
                    readWriteMode = ReadWriteMode.Read;
                }
                else if (ReadWriteMode.HasFlag(ReadWriteMode.Write) && !ReadWriteMode.HasFlag(ReadWriteMode.Read))
                {
                    readWriteMode = ReadWriteMode.Write;
                }
                else
                {
                    throw new ArgumentException("You must decide. do you want the read hash or the write hash?", nameof(readWriteMode));
                }
            }

            if (readWriteMode == ReadWriteMode.Read)
            {
                if (readHashCalculators == null)
                {
                    throw new ArgumentException("You didn't calculated the read hash, so how do you expect to get it?", nameof(readWriteMode));
                }
                return readHashCalculators[hashAlgorithmName].HashResult;
            }
            else
            {
                if (writeHashCalculators == null)
                {
                    throw new ArgumentException("You didn't calculated the write hash, so how do you expect to get it?", nameof(readWriteMode));
                }
                return writeHashCalculators[hashAlgorithmName].HashResult;
            }
        }
        public ICollection<HashResult> GetAllHashResults(ReadWriteMode readWriteMode)
        {
            if (readWriteMode == ReadWriteMode.ReadWrite)
            {
                if (ReadWriteMode.HasFlag(ReadWriteMode.Read) && !ReadWriteMode.HasFlag(ReadWriteMode.Write))
                {
                    readWriteMode = ReadWriteMode.Read;
                }
                else if (ReadWriteMode.HasFlag(ReadWriteMode.Write) && !ReadWriteMode.HasFlag(ReadWriteMode.Read))
                {
                    readWriteMode = ReadWriteMode.Write;
                }
                else
                {
                    throw new ArgumentException("You must decide. do you want the read hashes or the write hashes?", nameof(readWriteMode));
                }
            }

            if (readWriteMode == ReadWriteMode.Read)
            {
                if (readHashCalculators == null)
                {
                    throw new ArgumentException("You didn't calculated the read hash, so how do you expect to get it?", nameof(readWriteMode));
                }
                return readHashCalculators.Values.Select((hs) => hs.HashResult).ToList();
            }
            else
            {
                if (writeHashCalculators == null)
                {
                    throw new ArgumentException("You didn't calculated the write hash, so how do you expect to get it?", nameof(readWriteMode));
                }
                return writeHashCalculators.Values.Select((hs) => hs.HashResult).ToList();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            base.Dispose(disposing);

            if (readHashCalculators != null)
            {
                foreach (HashStream hashStream in readHashCalculators.Values)
                {
                    hashStream.Dispose();
                }
            }
            if (writeHashCalculators != null)
            {
                foreach (HashStream hashStream in writeHashCalculators.Values)
                {
                    hashStream.Dispose();
                }
            }

            if (!leaveOpen)
            {
                baseStream.Dispose();
            }
        }

        private ReadWriteMode ValidateReadWriteMode(ReadWriteMode readWriteMode)
        {
            bool canRead = CanRead, canWrite = CanWrite;

            if (!canRead && !canWrite)
            {
                throw new ArgumentException("This stream is prevented from read or write, so how do you expect to calculate any hash?", nameof(readWriteMode));
            }

            if (readWriteMode == ReadWriteMode.ReadWrite)
            {
                if (!canRead)
                {
                    readWriteMode = ReadWriteMode.Write;
                }
                if (!canWrite)
                {
                    readWriteMode = ReadWriteMode.Read;
                }
            }
            else if (readWriteMode == ReadWriteMode.Read)
            {
                if (!canRead)
                {
                    throw new ArgumentException("You can't read from this stream, so how do you expect to calculate the read hash?", nameof(readWriteMode));
                }
            }
            else if (readWriteMode == ReadWriteMode.Write)
            {
                if (!canWrite)
                {
                    throw new ArgumentException("You can't write to this stream, so how do you expect to calculate the write hash?", nameof(readWriteMode));
                }
            }

            return readWriteMode;
        }
    }
}
