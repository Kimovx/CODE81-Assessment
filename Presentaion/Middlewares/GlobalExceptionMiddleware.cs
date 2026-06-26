using CODE81_Assessment.Application.Exceptions;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CODE81_Assessment.Presentaion.Middlewares
{
    public class GlobalExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpException ex)
            {
                if (context.Response.HasStarted) throw;

                context.Response.Clear();
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    statusCode = ex.StatusCode,
                    messageEn = ex.Message,
                }, JsonOpts));
            }
            catch (Exception)
            {
                if (context.Response.HasStarted) throw;

                const int statusCode = StatusCodes.Status500InternalServerError;

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    statusCode,
                    messageEn = "Internal server error.",
                }, JsonOpts));
            }
        }
    }
}
