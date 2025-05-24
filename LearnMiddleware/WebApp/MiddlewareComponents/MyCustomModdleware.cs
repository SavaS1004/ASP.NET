
namespace WebApp.MiddlewareComponents
{
    public class MyCustomModdleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("My custom Middleware :Before calling next\n");
            await next(context);
            await context.Response.WriteAsync("My custom Middleware :After  calling next\n");
        }
    }
}
