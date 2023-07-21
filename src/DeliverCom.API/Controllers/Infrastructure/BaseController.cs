using System.Net;
using DeliverCom.API.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliverCom.API.Controllers.Infrastructure
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly IMediator Mediator;

        public BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected Task<IActionResult> ReturnResult<T>(ApiResponse<T> result,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var jsonResult = new JsonResult(result)
            {
                StatusCode = (int)statusCode
            };
            return Task.FromResult<IActionResult>(jsonResult);
        }
    }
}