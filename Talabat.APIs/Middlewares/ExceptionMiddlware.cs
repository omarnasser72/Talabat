using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
    public class ExceptionMiddlware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddlware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddlware(RequestDelegate next, ILogger<ExceptionMiddlware> logger, IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = env.IsDevelopment()
                                  ? new ApiExceptionResponse(500, ex.Message, ex.StackTrace?.ToString())
                                  : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace?.ToString());

                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonResponse = JsonSerializer.Serialize(response, options);

                await httpContext.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
