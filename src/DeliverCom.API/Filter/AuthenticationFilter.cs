using System.Security.Claims;
using DeliverCom.Core.Context.Impl;
using DeliverCom.Core.Context.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DeliverCom.API.Filter
{
    public class AuthenticationFilter : IActionFilter
    {
        private readonly IExecutionContext _context;
        private readonly ILogger _logger;

        public AuthenticationFilter(ILogger<AuthenticationFilter> logger, IExecutionContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Equals(_context.Identity, Identity.Empty)) return;
            var claims = context.HttpContext.User.Claims.ToList();
            if (claims.Count == 0) return;
            var companyId = claims.FirstOrDefault(f => f.Type == "CompanyId")?.Value;
            var email = claims.FirstOrDefault(f => f.Type == ClaimTypes.Email)?.Value;
            var identity = new Identity(email, companyId);
            _context.SetIdentity(identity);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("Action executed");
        }
    }
}