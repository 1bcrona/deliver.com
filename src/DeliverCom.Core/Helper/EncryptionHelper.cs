using System.Text;

namespace DeliverCom.Core.Helper
{
    public static class EncryptionHelper
    {
        public static string Encrypt(this string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }
    }
}