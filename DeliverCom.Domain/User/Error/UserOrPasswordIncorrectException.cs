using System.Net;
using DeliverCom.Core.Exception.Impl;

namespace DeliverCom.Domain.User.Error
{
    [Serializable]
    public class UserOrPasswordIncorrectException : HttpException
    {
        public UserOrPasswordIncorrectException(string message) : base(message)
        {
        }

        public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
    }
}