using System.Net;

namespace DeliverCom.Core.Exception.Impl
{
    public class ArgumentNotValidException : HttpException
    {
        public ArgumentNotValidException(string message) : base(message)
        {
        }

        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }
}