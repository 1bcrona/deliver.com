namespace DeliverCom.Core.Helper
{
    public static class StreamHelper
    {
        public static void CopyStream(Stream inputStream, Stream outputStream, int bufferLength = 2048)
        {
            long originalPosition = 0;

            if (inputStream.CanSeek)
            {
                originalPosition = inputStream.Position;
                inputStream.Position = 0;
            }

            var buffer = new byte[bufferLength];
            int bytesRead;
            do
            {
                bytesRead = inputStream.Read(buffer, 0, bufferLength);
                outputStream.Write(buffer, 0, bytesRead);
            } while (bytesRead > 0);

            if (inputStream.CanSeek)
            {
                inputStream.Position = originalPosition;
            }
        }

        public static byte[] ReadStream(Stream stream, int bufferLength = 2048)
        {
            if (stream.Length == 0) return Array.Empty<byte>();
            using var outputStream = new MemoryStream();
            CopyStream(stream, outputStream, bufferLength);
            return outputStream.ToArray();
        }

        public static void WriteStream(Stream stream, byte[] bytes)
        {
            if (bytes.Length == 0) return;
            using var inputStream = new MemoryStream(bytes);
            CopyStream(inputStream, stream);
        }
    }
}