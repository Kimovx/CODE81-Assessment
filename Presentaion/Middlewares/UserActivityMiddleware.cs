using CODE81_Assessment.Domain.Entities;
using CODE81_Assessment.Infrastructure;
using System.Security.Claims;

namespace CODE81_Assessment.Presentaion.Middlewares
{
    public class UserActivityMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var path = context.Request.Path;
            var method = context.Request.Method;

            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? context.Connection.RemoteIpAddress?.ToString();

            var userAgent = context.Request.Headers.UserAgent.ToString();

            int statusCode = 500;

            try
            {
                await _next(context);
                statusCode = context.Response.StatusCode;
            }
            catch
            {
                statusCode = 500;
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var log = new UserLoginLog
                    {
                        UserId = int.Parse(userId),
                        Action = $"{method} {path}",
                        IsSuccess = statusCode < 400,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        LoginTime = DateTimeOffset.UtcNow
                    };

                    await dbContext.UserLoginLogs.AddAsync(log);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
