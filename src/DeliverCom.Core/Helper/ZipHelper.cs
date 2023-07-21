using System.IO.Compression;

namespace DeliverCom.Core.Helper
{
    public static class ZipHelper
    {
        public static bool IsGZipped(byte[] data)
        {
            var head = (data[0] & 0xff) | ((data[1] << 8) & 0xff00);
            return (35615 == head); // GZip header value 
        }

        public static byte[] Compress(byte[] data)
        {
            using var compressedStream = new MemoryStream();
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Compress);
            using var memoryStream = new MemoryStream(data);
            StreamHelper.CopyStream(memoryStream, zipStream);
            zipStream.Close();
            return compressedStream.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            using var compressedStream = new MemoryStream(data);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var memory = new MemoryStream();
            StreamHelper.CopyStream(zipStream, memory);
            return memory.ToArray();
        }


    }
}