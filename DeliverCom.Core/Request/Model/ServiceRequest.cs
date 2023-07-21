namespace DeliverCom.Core.Request.Model
{
    public class ServiceRequest
    {
        public string Id { get; set; }
        public string CorrelationId { get; set; }

        public long RequestDateEpoch { get; set; }
        public RequestProperties Request { get; set; }
        public ResponseProperties ResponseProperties { get; set; }

        public CallParameters CallParameters { get; set; }
        public string RemoteIpAddress { get; set; }
        public string LocalIpAddress { get; set; }

        public string ClientIpAddress { get; set; }
        public System.Exception Exception { get; set; }

        public long Duration { get; set; }
        public string XFF { get; set; }
        public long RequestEndDateEpoch { get; set; }
    }
}