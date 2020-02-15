using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Types.BytesSize;

namespace IO_Utils.Streams
{
    public class DataCounterStream : WrapStream
    {
        public Size ReadedBytes { get; private set; }
        public Size WritedBytes { get; private set; }

        public DataCounterStream(Stream baseStream, bool leaveOpen = false) : base(baseStream, leaveOpen) { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readedBytes = base.Read(buffer, offset, count);
            ReadedBytes += readedBytes;
            return readedBytes;
        }
        public override int ReadByte()
        {
            int b = base.ReadByte();
            if (b != -1)
            {
                ReadedBytes += 1;
            }
            return b;
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int readedBytes = await base.ReadAsync(buffer, offset, count, cancellationToken);
            ReadedBytes += readedBytes;
            return readedBytes;
        }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            WritedBytes += count;
        }
        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
            WritedBytes += 1;
        }
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Task task = base.WriteAsync(buffer, offset, count, cancellationToken);
            await task;
            if (!task.IsCanceled)
            {
                WritedBytes += count;
            }
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }
    }
}
