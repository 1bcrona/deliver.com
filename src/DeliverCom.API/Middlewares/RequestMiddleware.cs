using System.Net;
using DeliverCom.API.Model;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Exception.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DeliverCom.API.Middlewares
{
    public class RequestMiddleware
    {
        private static readonly Lazy<JsonSerializerSettings> _jsonSerializerSettings = new(() =>
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }, true);

        private readonly RequestDelegate _next;

        public RequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private static JsonSerializerSettings JsonSerializerSettings => _jsonSerializerSettings.Value;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                var request = httpContext.Request;
                request.EnableBuffering();
                if (!request.Headers.ContainsKey("X-Correlation-Id"))
                    request.Headers["X-Correlation-Id"] = Guid.NewGuid().ToString("D");
                await _next(httpContext);
                if (httpContext.Response.StatusCode == (int)HttpStatusCode.NoContent)
                    httpContext.Response.ContentLength = 0;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new ApiErrorResponse();
            var statusCode = HttpStatusCode.InternalServerError;
            if (exception is not BaseException) exception = BaseException.FromException<UnknownException>(exception);

            if (exception is HttpException httpException) statusCode = httpException.StatusCode;

            response.Error = exception.GetType().Name;
            response.Message = ((BaseException)exception).ExceptionMessage;
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, JsonSerializerSettings));
        }
    }
}