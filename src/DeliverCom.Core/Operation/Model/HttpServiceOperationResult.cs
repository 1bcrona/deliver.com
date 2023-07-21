using System.Net;

namespace DeliverCom.Core.Operation.Model
{
    public class HttpServiceOperationResult : OperationResult
    {
        public HttpStatusCode StatusCode { get; set; }
    }
}