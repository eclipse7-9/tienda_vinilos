using System.Net;
using System.Text.Json;

namespace MiPrimeraAPI.Middleware
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    error = "Ocurrió un error interno en el servidor",
                    status = 500
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
                    
            }
        }
        
    }
}
