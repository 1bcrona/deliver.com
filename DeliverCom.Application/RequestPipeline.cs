using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Request.Model;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

#pragma warning disable CS8601

namespace DeliverCom.Application
{
    public class RequestPipeline<TIn, TOut> : IPipelineBehavior<TIn, TOut> where TIn : IRequest<TOut>
    {
        private readonly IExecutionContext _context;
        private readonly HttpContext _httpContext;
        private readonly ILogger<RequestPipeline<TIn, TOut>> _logger;

        public RequestPipeline(IHttpContextAccessor accessor, IExecutionContext context,
            ILogger<RequestPipeline<TIn, TOut>> logger)
        {
            _context = context;
            _logger = logger;
            _httpContext = accessor.HttpContext;
        }


        public async Task<TOut> Handle(TIn request, RequestHandlerDelegate<TOut> next,
            CancellationToken cancellationToken)
        {
            if (request is not BaseRequest baseRequest) return await next();

            if (_httpContext.Request.Headers.TryGetValue("X-Correlation-Id", out var header))
                baseRequest.CorrelationId = header;

            baseRequest.CorrelationId ??= Guid.NewGuid().ToString("D");


            var remoteIpAddress = _httpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
            var localIpAddress = _httpContext?.Connection?.LocalIpAddress?.MapToIPv4().ToString();
            var xff = GetXff(_httpContext);


            _context.SetTraceId(baseRequest.CorrelationId);
            var serviceRequest = new ServiceRequest
            {
                RequestDateEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CorrelationId = baseRequest.CorrelationId,
                Id = Guid.NewGuid().ToString("D"),
                Request = new RequestProperties
                {
                    Type = request.GetType().FullName,
                    Assembly = request.GetType().Assembly.FullName,
                    Method = request.ToString()
                },
                RemoteIpAddress = remoteIpAddress,
                LocalIpAddress = localIpAddress,
                ClientIpAddress = xff?.Split(",")[^1].Trim(),
                XFF = xff,
                CallParameters = new CallParameters()
            };
            _context.SetRequest(serviceRequest);
            OnBeforeExecuting(request, _context);
            TOut act;
            try
            {
                act = await next();
                _context.Request.ResponseProperties = new ResponseProperties
                {
                    Type = act?.GetType().FullName
                };
                _context.Request.CallParameters.Return = act;
            }
            finally
            {
                _context.Request.RequestEndDateEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                _context.Request.Duration =
                    _context.Request.RequestEndDateEpoch - _context.Request.RequestDateEpoch;
            }


            _logger.LogInformation("{@ServiceRequestProperties}", _context.Request);

            return act;
        }

        private string GetXff(HttpContext context)
        {
            if (context?.Request == null) return null;
            var stringValues = context.Request.Headers["X-Forwarded-For"];
            if (StringValues.IsNullOrEmpty(stringValues)) return string.Empty;
            return stringValues.Count > 0 ? string.Join(",", stringValues.ToArray()) : string.Empty;
        }

        private void OnBeforeExecuting(TIn request, IExecutionContext context)
        {
            var typeOfRequest = typeof(TIn);
            context.Request.CallParameters.Inputs = new Dictionary<string, object>();

            foreach (var prop in typeOfRequest.GetProperties())
                context.Request.CallParameters.Inputs[prop.Name] = prop.GetValue(request);
        }
    }
}