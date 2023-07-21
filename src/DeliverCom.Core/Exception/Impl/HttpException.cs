using System.Net;
using DeliverCom.Core.Exception.Infrastructure;

namespace DeliverCom.Core.Exception.Impl
{
    public class HttpException : BaseException
    {
        protected HttpException(string message) : base(message)
        {
        }

        public virtual HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
    }
}