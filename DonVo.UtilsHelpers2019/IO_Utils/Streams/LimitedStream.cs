using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Types.BytesSize;

namespace IO_Utils.Streams
{
    public class LimitedStream : WrapStream
    {
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => Math.Min(base.Length, Limit.Bytes);
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public Size Limit { get; set; }

        private Size readedBytes = Size.ZERO_SIZE;

        public LimitedStream(Stream baseStream, Size limit, bool leaveOpen = false) : base(baseStream, leaveOpen)
        {
            Limit = limit;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesToRead = (int)Math.Min(count, (Limit - this.readedBytes).Bytes);
            int readedBytes = base.Read(buffer, offset, bytesToRead);
            this.readedBytes += readedBytes;
            return readedBytes;
        }
        public override int ReadByte()
        {
            if ((Limit - readedBytes).Bytes > 0)
            {
                int b = base.ReadByte();
                if (b != -1)
                {
                    readedBytes += 1;
                }
                return b;
            }

            return -1;
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int bytesToRead = (int)Math.Min(count, (Limit - this.readedBytes).Bytes);
            int readedBytes = await base.ReadAsync(buffer, offset, bytesToRead, cancellationToken);
            this.readedBytes += readedBytes;
            return readedBytes;
        }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }
    }
}
