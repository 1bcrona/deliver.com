using System.Net;

namespace DeliverCom.Core.Exception.Impl
{
    public class NotFoundException : HttpException
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}