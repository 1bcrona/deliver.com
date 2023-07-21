using System.Net;

namespace DeliverCom.Core.Exception.Impl
{
    public class AuthenticationException : HttpException
    {
        public AuthenticationException(string mesage) : base(mesage)
        {
        }

        public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
    }
}