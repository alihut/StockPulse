using StockPulse.Application.Enums;
using StockPulse.Application.Models;
using System.Net;

namespace StockPulse.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var result = new Result
                {
                    StatusCode = StatusCode.InternalServerError,
                    Message = "An unexpected error occurred."
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        }
    }

}
