namespace DeliverCom.Core.Request.Model
{
    public class RequestProperties
    {
        public string Assembly { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public CachingProperties CacheProperties { get; set; }

        public IDictionary<RequestParameter, object> RequestParameters { get; set; }
    }
}