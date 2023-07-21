using DeliverCom.Core.Exception.Infrastructure;

namespace DeliverCom.Core.Exception.Impl
{
    public class UnknownException : BaseException
    {
        public UnknownException() : this("Unknown exception is occured")
        {
        }

        private UnknownException(string message) : base(message)
        {
        }
    }
}