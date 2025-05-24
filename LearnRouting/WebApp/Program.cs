namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            app.Use(async (context, next) =>
            {
                await next(context);
            }
            );

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                await next(context);
            }
);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/employees", async (HttpContext context) =>
                {
                    await context.Response.WriteAsync("Get employees");

                });

                endpoints.MapPost("/employees", async (HttpContext context) =>
                {
                    await context.Response.WriteAsync("Create an  employee");

                });

                endpoints.MapPut("/employees", async (HttpContext context) =>
                {
                    await context.Response.WriteAsync("Update an  employee");

                });

                endpoints.MapDelete("/employees", async (HttpContext context) =>
                {
                    await context.Response.WriteAsync("Delete an  employee");

                });
            });
            

            app.Run();
        }
    }
}
