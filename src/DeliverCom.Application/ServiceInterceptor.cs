using System.Data;
using Castle.DynamicProxy;
using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Data.Infrastructure.KeyValueStore;
using DeliverCom.Core.Helper;
using DeliverCom.Core.InvocationPropertiesGenerator.Infrastructure;
using DeliverCom.Core.Request.Model;
using Microsoft.Extensions.Logging;

namespace DeliverCom.Application
{
    public class ServiceInterceptor : IAsyncInterceptor
    {
        private readonly IExecutionContext _executionContext;
        private readonly IInvocationPropertiesGenerator _invocationPropertiesGenerator;
        private readonly ILogger<ServiceInterceptor> _logger;
        private readonly IKeyValueStore _store;
        public ServiceInterceptor(IExecutionContext executionContext, ILogger<ServiceInterceptor> logger,
            IInvocationPropertiesGenerator invocationPropertiesGenerator, IKeyValueStore store = null)
        {
            _executionContext = executionContext;
            _logger = logger;
            _store = store;
            _invocationPropertiesGenerator = invocationPropertiesGenerator;
        }
        public void InterceptSynchronous(IInvocation invocation)
        {
            var request = OnBeforeInvocation(invocation);
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                request.Exception = e;
                throw;
            }
            finally
            {
                FinalizeRequest(request, invocation.ReturnValue);
            }
        }
        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }
        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }
        private ServiceRequest OnBeforeInvocation(
            IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget;
            var request = new ServiceRequest
            {
                Id = Guid.NewGuid().ToString("D"),
                RequestDateEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CorrelationId = _executionContext.TraceId ?? Guid.NewGuid().ToString("D"),
                LocalIpAddress = _executionContext.Request?.LocalIpAddress,
                RemoteIpAddress = _executionContext.Request?.RemoteIpAddress,
                ClientIpAddress = _executionContext.Request?.ClientIpAddress,
                XFF = _executionContext.Request?.XFF,
                Request = new RequestProperties
                {
                    Method = method.Name,
                    Assembly = method.ReflectedType?.Assembly.FullName,
                    Type = method.ReflectedType?.FullName
                }
            };
            var invocationProperties = _invocationPropertiesGenerator?.Generate(method, invocation.Arguments);
            request.Request.CacheProperties = invocationProperties?.CachingProperties;
            request.Request.RequestParameters = invocationProperties?.RequestParameters;
            AddRequestToStack(request);
            return request;
        }
        private void OnAfterInvocation(object result, ServiceRequest request)
        {
            FillRequestCallParameters(request);
            if (request.Exception == null) SetResponseProperties(request, result);

            LogAfterInvocation(request);
        }
        private void AddRequestToStack(ServiceRequest request)
        {
            _executionContext.RequestStack.Push(request);
        }
        private void SetResponseProperties(ServiceRequest request, object result)
        {
            if (result == null) return;
            request.ResponseProperties = new ResponseProperties
            {
                Assembly = result.GetType().Assembly.FullName,
                Type = result.GetType().FullName
            };
            request.CallParameters.Return = SerializationHelper.Serialize(result);
        }
        private void CacheAfterInvocation(CachingProperties requestCacheProperties, object result)
        {
            _store?.SetAsync(requestCacheProperties.CacheKey, result,
                TimeSpan.FromSeconds(requestCacheProperties.CacheDuration));
        }
        private void LogAfterInvocation(ServiceRequest request)
        {
            try
            {
                if (request.Exception != null)
                    _logger.LogError("{@ServiceRequestProperties}", request);
                else
                    _logger.LogInformation("{@ServiceRequestProperties}", request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            var request = OnBeforeInvocation(invocation);
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            try
            {
                await task;
            }
            catch (Exception e)
            {
                request.Exception = e;
                throw;
            }
            finally
            {
                FinalizeRequest(request, null);
            }
        }

        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var request = OnBeforeInvocation(invocation);
            var result = default(TResult);
            if (!string.IsNullOrWhiteSpace(request.Request.CacheProperties?.CacheKey))
            {
                TResult cacheResult = default;
                var foundInCache = false;
                if (_store != null)
                {
                    cacheResult = _store.Get<TResult>(request.Request.CacheProperties?.CacheKey);
                    foundInCache = !EqualityComparer<TResult>.Default.Equals(cacheResult, default);
                }

                if (foundInCache)
                {
                    FinalizeRequest(request, cacheResult);
                    return cacheResult;
                }
            }
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            try
            {
                result = await task;
            }
            catch (Exception e)
            {
                request.Exception = e;
                throw;
            }
            finally
            {
                FinalizeRequest(request, result);
            }
            return result;
        }

        private void FinalizeRequest(ServiceRequest request, object result)
        {
            request.RequestEndDateEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            request.Duration = request.RequestEndDateEpoch - request.RequestDateEpoch;
            OnAfterInvocation(result, request);
            if (request.Request.CacheProperties == null || request.Exception != null) return;
            try
            {
                CacheAfterInvocation(request.Request.CacheProperties, result);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private static void FillRequestCallParameters(ServiceRequest request)
        {
            var callParameters = new CallParameters
            {
                Inputs = request.Request.RequestParameters
                    ?.Where(w => w.Key.ParameterDirection == ParameterDirection.Input)
                    .ToDictionary(pair => pair.Key.ParameterName, pair => pair.Value)
            };

            request.CallParameters = callParameters;
        }
    }
}