using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace CustomerAPI.Exceptions.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An error occurred while processing your request.",
                Detailed = exception.Message
            };

            context.Response.ContentType = "application/json";

            if (exception is DbUpdateException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; 
                response = new
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "A database update error occurred during the transaction.",
                    Detailed = exception.Message
                };
            }
            else if (exception is DbUpdateConcurrencyException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response = new
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Message = "A concurrency conflict occurred while processing the request.",
                    Detailed = exception.Message
                };
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
