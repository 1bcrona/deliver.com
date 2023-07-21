using System.Collections.Concurrent;
using System.Globalization;
using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Request.Model;

namespace DeliverCom.Core.Context.Impl
{
    public class AmbientExecutionContext : IExecutionContext
    {
        #region Public Constructors

        public AmbientExecutionContext() : this(Impl.Identity.Empty)
        {
        }

        private AmbientExecutionContext(IIdentity identity)
        {
            Id = Guid.NewGuid().ToString("D");
            TraceId = Guid.NewGuid().ToString("D");
            Identity = identity;
        }

        public void SetTraceId(string traceId)
        {
            TraceId = traceId;
        }

        public string Id { get; }
        public string TraceId { get; private set; }
        public string Route { get; private set; }

        #endregion Public Constructors

        #region Public Properties

        public ServiceRequest Request { get; private set; }


        public void SetRequest(ServiceRequest serviceRequest)
        {
            Request = serviceRequest;
            if (RequestStack.IsEmpty)
                RequestStack.Push(serviceRequest);
        }


        public static AmbientExecutionContext Empty => new(Impl.Identity.Empty);

        public IIdentity Identity { get; private set; }

        public void SetIdentity(IIdentity identity)
        {
            Identity = identity;
        }

        public bool IsEmpty => Identity.Equals(Impl.Identity.Empty);
        public CultureInfo CurrentCulture { get; } = CultureInfo.GetCultureInfo("tr-TR");
        public ConcurrentStack<ServiceRequest> RequestStack { get; } = new();

        public ServiceRequest CurrentRequest => RequestStack.TryPeek(out var request) ? request : null;

        public void SetRoute(string route)
        {
            Route = route;
        }

        #endregion Public Properties
    }
}