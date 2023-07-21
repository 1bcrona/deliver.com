using System.Text;

namespace DeliverCom.Core.Helper
{
    public static class ExceptionHelper
    {
        public static string GetExceptionMessage(this System.Exception ex)
        {
            var builder = new StringBuilder(64);
            while (true)
            {
                if (ex == null) break;
                builder.AppendLine(ex.Message);
                ex = ex.InnerException;
            }

            return builder.ToString().Trim();
        }
    }
}