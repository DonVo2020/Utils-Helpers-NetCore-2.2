using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IO_Utils.Streams
{
    public abstract class WrapStream : Stream
    {
        public override bool CanRead => baseStream.CanRead;
        public override bool CanWrite => baseStream.CanWrite;
        public override bool CanSeek => baseStream.CanSeek;
        public override bool CanTimeout => baseStream.CanTimeout;
        public override long Length => baseStream.Length;
        public override long Position { get => baseStream.Position; set => baseStream.Position = value; }
        public override int ReadTimeout { get => baseStream.ReadTimeout; set => baseStream.ReadTimeout = value; }
        public override int WriteTimeout { get => baseStream.WriteTimeout; set => baseStream.WriteTimeout = value; }

        private bool disposed = false;
        private readonly bool leaveOpen;
        private readonly Stream baseStream;

        public WrapStream(Stream baseStream, bool leaveOpen = false)
        {
            this.baseStream = baseStream;
            this.leaveOpen = leaveOpen;
        }

        public override void Flush() => baseStream.Flush();
        public override Task FlushAsync(CancellationToken cancellationToken) => baseStream.FlushAsync(cancellationToken);
        public override void SetLength(long value) => baseStream.SetLength(value);
        public override long Seek(long offset, SeekOrigin origin) => baseStream.Seek(offset, origin);
        public override int Read(byte[] buffer, int offset, int count) => baseStream.Read(buffer, offset, count);
        public override int ReadByte() => baseStream.ReadByte();
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => baseStream.ReadAsync(buffer, offset, count, cancellationToken);
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => baseStream.BeginRead(buffer, offset, count, callback, state);
        public override int EndRead(IAsyncResult asyncResult) => baseStream.EndRead(asyncResult);
        public override void Write(byte[] buffer, int offset, int count) => baseStream.Write(buffer, offset, count);
        public override void WriteByte(byte value) => baseStream.WriteByte(value);
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => baseStream.WriteAsync(buffer, offset, count, cancellationToken);
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => baseStream.BeginWrite(buffer, offset, count, callback, state);
        public override void EndWrite(IAsyncResult asyncResult) => baseStream.EndWrite(asyncResult);

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            base.Dispose(disposing);

            if (!leaveOpen)
            {
                baseStream.Dispose();
            }

            disposed = true;
        }
    }
}
