using WebApp.MiddlewareComponents;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddTransient<ExceptionMiddleware>();

            var app = builder.Build();
            
            app.UseMiddleware<ExceptionMiddleware>();
            
            //MIddleware #1
            app.Use(async(context, next) =>
            {
                
                await context.Response.WriteAsync("Middleware #1:Before calling next\n");
                await next(context);
                await context.Response.WriteAsync("Middleware #1:After  calling next\n");

            });
            //MIddleware #2
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Middleware #2:Before calling next\n");
                await next(context);
                await context.Response.WriteAsync("Middleware #2:After  calling next\n");

            });
            //MIddleware #3
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Middleware #3:Before calling next\n");
                await next(context);
                await context.Response.WriteAsync("Middleware #3:After  calling next\n");
                throw new ApplicationException("Exception for testing");

            });
            //Middleware #4
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Middleware #4:Before calling next\n");
                await next(context);
                await context.Response.WriteAsync("Middleware #4:After  calling next\n");

            });

            app.Run();
        }
    }
}
