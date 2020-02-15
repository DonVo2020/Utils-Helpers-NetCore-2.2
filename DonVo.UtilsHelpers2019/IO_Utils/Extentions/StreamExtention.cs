using IO_Utils.Streams;
using System.IO;
using System.Threading.Tasks;
using Types.BytesSize;

namespace IO_Utils.Extentions
{
    public static class StreamExtention
    {
        public static byte[] ReadToEnd(this Stream stream, Size? estimatedSize = null)
        {
            try
            {
                estimatedSize = estimatedSize ?? stream.Length;
            }
            catch { }

            byte[] content;
            using (MemoryStream memoryStream = new MemoryStream((int)(estimatedSize?.Bytes ?? 0)))
            {
                //TODO: limit the max file size
                stream.CopyTo(memoryStream);
                if (memoryStream.Length == memoryStream.Capacity)
                {
                    content = memoryStream.GetBuffer();
                }
                else
                {
                    content = memoryStream.ToArray();
                }
            }

            return content;
        }
        public static async Task<byte[]> ReadToEndAsync(this Stream stream, Size? estimatedSize = null)
        {
            try
            {
                estimatedSize = estimatedSize ?? stream.Length;
            }
            catch { }

            byte[] content;
            using (MemoryStream memoryStream = new MemoryStream((int)(estimatedSize?.Bytes ?? 0)))
            {
                //TODO: limit the max file size
                await stream.CopyToAsync(memoryStream);
                if (memoryStream.Length == memoryStream.Capacity)
                {
                    content = memoryStream.GetBuffer();
                }
                else
                {
                    content = memoryStream.ToArray();
                }
            }

            return content;
        }
        public static byte[] ReadMax(this Stream stream, Size limit, Size? estimatedSize = null)
        {
            using (LimitedStream limitedStream = new LimitedStream(stream, limit, leaveOpen: true))
            {
                return limitedStream.ReadToEnd(estimatedSize);
            }
        }
        public static async Task<byte[]> ReadMaxAsync(this Stream stream, Size limit, Size? estimatedSize = null)
        {
            using (LimitedStream limitedStream = new LimitedStream(stream, limit, leaveOpen: true))
            {
                return await limitedStream.ReadToEndAsync(estimatedSize);
            }
        }
        public static void Write(this Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }
        public static Task WriteAsync(this Stream stream, byte[] buffer)
        {
            return stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
