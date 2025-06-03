using WebApi.Models;
using WebApi.Results;

namespace WebApi.Endpoints
{
    public static class EmployeeEndpoints
    {
        public static void MapEmployeeEndpoints(this WebApplication app)
        {
            app.MapGet("/", HtmlResult () =>
            {
                string html = "<h2>Welcome to our API</h2> Our API is used to learn ASP.NET CORE.";

                return new HtmlResult(html);
            });

            app.MapGet("/employees", (IEmployeesRepository employeesRepository) =>
            {
                var employees = employeesRepository.GetEmployees();

                return TypedResults.Ok(employees);
            });

            app.MapGet("/employees/{id:int}", (int id, IEmployeesRepository employeesRepository) =>
            {
                var employee = employeesRepository.GetEmployeeById(id);
                return employee is not null
                    ? TypedResults.Ok(employee)
                    : Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        {"id", new[] { $"Employee with the id {id} doesn't exist." } }
                    },
                    statusCode: 404);
            });

            app.MapPost("/employees", (Employee employee, IEmployeesRepository employeesRepository) =>
            {
                if (employee is null || employee.Id < 0)
                {
                    return Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        {"id", new[] { "Employee is not provided or is not valid." } }
                    },
                    statusCode: 400);
                }

                employeesRepository.AddEmployee(employee);
                return TypedResults.Created($"/employees/{employee.Id}", employee);

            }).WithParameterValidation();

            app.MapPut("/employees/{id:int}", (int id, Employee employee, IEmployeesRepository employeesRepository) =>
            {

                if (id != employee.Id)
                {
                    return Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        {"id", new[] { "Employee id is not the same as id." } }
                    },
                    statusCode: 400);
                }

                return employeesRepository.UpdateEmployee(employee)
                    ? TypedResults.NoContent()
                    : Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        {"id", new[] { "Employee doesn't exist." } }
                    },
                    statusCode: 404);
            });

            app.MapDelete("/employees/{id:int}", (int id, IEmployeesRepository employeesRepository) =>
            {
                var employee = employeesRepository.GetEmployeeById(id);

                return employeesRepository.DeleteEmployee(employee)
                    ? TypedResults.Ok(employee)
                    : Microsoft.AspNetCore.Http.Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        {"id", new[] { $"Employee with the id {id} doesn't exist." } }
                    },
                    statusCode: 404);
            });
        }
    }
}
