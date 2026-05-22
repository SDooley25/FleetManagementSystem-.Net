using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FleetManagementSystem_.Net.Middleware
{
    public class SQLExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SQLExceptionHandlerMiddleware> _logger;

        public SQLExceptionHandlerMiddleware(RequestDelegate next, ILogger<SQLExceptionHandlerMiddleware> logger)
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
                //if (ex is SqlException sqlEx)
                //{
                    if(ex.Message.ToLower().Contains("timeout expired"))
                    {
                        _logger.LogWarning(ex, "SQL timeout exception handled: {Message}", ex.Message);
                        if (!context.Response.HasStarted)
                        {
                            // Clear any partial response
                            context.Response.Clear();
                            context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
                            //Redirect to specific error page.
                            context.Response.Redirect("/Error/SQLTimeout");
                            return;
                        }
                        _logger.LogError("Response already started; cannot redirect for SQL timeout exception.");
                    }
                    
                //}

                // rethrow so other handlers (or default error pages) can handle it
                throw;
            }
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseSQLExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SQLExceptionHandlerMiddleware>();
        }
    }
}
