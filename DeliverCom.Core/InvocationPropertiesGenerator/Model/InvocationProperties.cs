using DeliverCom.Core.Request.Model;

namespace DeliverCom.Core.InvocationPropertiesGenerator.Model
{
    public class InvocationProperties
    {
        public CachingProperties CachingProperties { get; set; }
        public IDictionary<RequestParameter, object> RequestParameters { get; set; }
    }
}