using System.Net;

namespace DeliverCom.Core.Exception.Impl
{
    public class AlreadyExistsException : HttpException
    {
        public AlreadyExistsException(string mesage) : base(mesage)
        {
        }

        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    }
}