using System.Collections.Concurrent;
using System.Globalization;
using DeliverCom.Core.Request.Model;

namespace DeliverCom.Core.Context.Infrastructure
{
    public interface IExecutionContext
    {
        void SetRoute(string route);

        #region Public Properties

        IIdentity Identity { get; }


        ServiceRequest Request { get; }


        void SetIdentity(IIdentity identity);


        void SetRequest(ServiceRequest serviceRequest);
        void SetTraceId(string traceId);
        string Id { get; }
        string TraceId { get; }

        string Route { get; }
        bool IsEmpty { get; }

        CultureInfo CurrentCulture { get; }

        ConcurrentStack<ServiceRequest> RequestStack { get; }

        ServiceRequest CurrentRequest => RequestStack.TryPeek(out var request) ? request : null;

        #endregion Public Properties
    }
}