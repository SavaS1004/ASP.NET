using LearnModelValidation.Models;  
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace LearnModelValidation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            
            app.MapGet("/", async (HttpContext context) =>
            {
                await context.Response.WriteAsync("Welcome to the home page.");
            });

            app.MapGet("/employees", async (HttpContext context) =>
            {
                var employees = EmployeesRepository.GetEmployees();
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<h1>Employees List</h1><ul>");
                await context.Response.WriteAsync("<ul>");
                foreach (var employee in employees)
                {
                    await context.Response.WriteAsync($"<li>{employee.Id} - {employee.Name} - {employee.Position} - ${employee.Salary}</li>");
                }
                await context.Response.WriteAsync("</ul>");

            });
            //quesry string example
            app.MapPost("/employees/query", async (HttpContext context) =>
            {
                var employee = await Employee.BindAsyncQueryString(context);
                if (employee == null || employee.Id <= 0)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid or missing employee data.");
                    return;
                }

                // Manual validation
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(employee);
                if (!Validator.TryValidateObject(employee, validationContext, validationResults, true))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(
                        "Validation failed: " + string.Join("; ", validationResults.Select(r => r.ErrorMessage))
                    );
                    return;
                }

                EmployeesRepository.AddEmployee(employee);  
                context.Response.StatusCode = 201;
                await context.Response.WriteAsync("Employee added successfully from query string.");
            });

            //bodyexample
            app.MapPost("/employees/body", async (HttpContext context) =>
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var employee = JsonSerializer.Deserialize<Employee>(body);

                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(employee);
                if (!Validator.TryValidateObject(employee, validationContext, validationResults, true))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(
                        "Validation failed: " + string.Join("; ", validationResults.Select(r => r.ErrorMessage))
                    );
                    return;
                }
                    EmployeesRepository.AddEmployee(employee);
                    context.Response.StatusCode = 201;
                await context.Response.WriteAsync("Employee added successfully.");
            });

            app.MapPut("/employees", async (HttpContext context) =>
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var employee = JsonSerializer.Deserialize<Employee>(body);

                var result = EmployeesRepository.UpdateEmployee(employee);
                if (result)
                {
                    context.Response.StatusCode = 204;
                    await context.Response.WriteAsync("Employee updated successfully.");
                    return;

                }
                else
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Employee not found.");

                }

            });



            app.Run();

        }
    }


}
