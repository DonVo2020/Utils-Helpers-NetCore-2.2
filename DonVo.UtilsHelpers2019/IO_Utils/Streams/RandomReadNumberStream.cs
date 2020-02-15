using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IO_Utils.Streams
{
    public class RandomReadNumberStream : WrapStream
    {
        private readonly Random random = new Random();

        public RandomReadNumberStream(Stream baseStream, bool leaveOpen = false) : base(baseStream, leaveOpen) { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = CalculateCountToRead(count);
            return base.Read(buffer, offset, count);
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            count = CalculateCountToRead(count);
            return base.ReadAsync(buffer, offset, count, cancellationToken);
        }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            count = CalculateCountToRead(count);
            return base.BeginRead(buffer, offset, count, callback, state);
        }

        private int CalculateCountToRead(int count)
        {
            return random.Next(1, count);
        }
    }
}
