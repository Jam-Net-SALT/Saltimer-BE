
namespace Saltimer.Api.Middleware;
public class AuthUserMiddleware
{
    private readonly RequestDelegate _next;

    public AuthUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IAuthService service, SaltimerDBContext db)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = service.ValidateToken(token);
        if (userId != null)
        {
            // attach user to context on successful jwt validation
            context.Items["User"] = await db.User.FindAsync(userId.Value);
        }

        await _next(context);
    }
}