using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace WebAPP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            app.Run(async (HttpContext context) =>
            {

                if (context.Request.Path.StartsWithSegments("/"))
                {
                    context.Response.Headers["Content-Type"] = "text/html";
                    await context.Response.WriteAsync($"The method is: {context.Request.Method}<br/>");
                    await context.Response.WriteAsync($"The Url is: {context.Request.Path}<br/>");

                    await context.Response.WriteAsync($"<b>Headers:</b>:<br/>");
                    await context.Response.WriteAsync("<ul>");
                    foreach (var key in context.Request.Headers.Keys)
                    {
                        await context.Response.WriteAsync($"<li>{key}: {context.Request.Headers[key]}</li>");
                    }
                    await context.Response.WriteAsync("</ul>");
                }
                else if (context.Request.Path.StartsWithSegments("/employees"))
                {
                    if (context.Request.Method == "GET")
                    {
                        if (context.Request.Query.ContainsKey("id"))
                        {
                            var id = context.Request.Query["id"];
                            if (int.TryParse(id, out int employeeId))
                            {
                                var result = EmployeesRepository.GetEmployeeById(employeeId);
                                if (result is not null)
                                {
                                    context.Response.Headers["Content-Type"] = "text/html";
                                    await context.Response.WriteAsync($"Name=<b>{result.Name}</b></br>" +
                                        $"Position=<b>{result.Position}</b></br>" +
                                        $"Salary=<b>{result.Salary}</b></br>");
                                }
                                else
                                {
                                    context.Response.StatusCode = 404;
                                    await context.Response.WriteAsync("Employee not found.");
                                }
                            }
                        }
                        else
                        {
                            var employees = EmployeesRepository.GetEmployees();
                            context.Response.Headers["Content-Type"] = "text/html";

                            await context.Response.WriteAsync("<ul>");
                            foreach (var employee in employees)
                            {
                                await context.Response.WriteAsync($"<li><b>{employee.Name}</b>: {employee.Position}</li>");
                            }
                            await context.Response.WriteAsync("</ul>");

                            context.Response.StatusCode = 200;
                        }
                    } 
                    else if (context.Request.Method == "POST")
                    {
                        using var reader = new StreamReader(context.Request.Body);
                        var body = await reader.ReadToEndAsync();
                        try
                        {
                            var employee = JsonSerializer.Deserialize<Employee>(body);
                            EmployeesRepository.AddEmployee(employee);
                            await context.Response.WriteAsync("Employee added successfully.");
                            context.Response.StatusCode = 201;                           
                        }
                        catch (Exception ex) 
                        {
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync($"{ex.ToString}");
                            return;
                        }
                        
                        
                    }
                    else if (context.Request.Method == "PUT")
                    {
                        using var reader = new StreamReader(context.Request.Body);
                        var body = await reader.ReadToEndAsync();
                        var employee = JsonSerializer.Deserialize<Employee>(body);
                        var result = EmployeesRepository.UpdateEmployee(employee);
                        if (result)
                        {
                            context.Response.StatusCode = 204;
                            await context.Response.WriteAsync($"{employee} updated successfully.");
                            return;

                        }
                        else
                        {
                            await context.Response.WriteAsync($"{employee} not found.");

                        }
                    }
                    else if (context.Request.Method == "DELETE")
                    {
                        if (context.Request.Query.ContainsKey("id"))
                        {
                            var id = context.Request.Query["id"];
                            if (int.TryParse(id, out int employeeId))
                            {
                                if (context.Request.Headers["Authorization"] == "frank")
                                {
                                    var result = EmployeesRepository.DeleteEmployee(employeeId);

                                    if (result)
                                    {
                                        
                                        await context.Response.WriteAsync("Employee is deleted successfully.");
                                    }
                                    else
                                    {
                                        context.Response.StatusCode = 404;
                                        await context.Response.WriteAsync("Employee not found.");
                                    }
                                }
                                else
                                {
                                    context.Response.StatusCode = 401;
                                    await context.Response.WriteAsync("You are not authorize to delete.");
                                }
                            }
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                }


            });
            app.Run();
        }
        static class EmployeesRepository
        {
            private static List<Employee> employees = new List<Employee>
                 {
                     new Employee(1, "John Doe", "Engineer", 60000),
                     new Employee(2, "Jane Smith", "Manager", 75000),
                    new Employee(3, "Sam Brown", "Technician", 50000)
             };

            public static List<Employee> GetEmployees() => employees;

            public static void AddEmployee(Employee? employee)
            {
                if(employee is not null)
                {
                    employees.Add(employee);
                }
               
            }
            public static bool UpdateEmployee(Employee? employee)
            {
                if(employee is not null)
                {
                    var emp=employees.FirstOrDefault(x=> x.Id==employee.Id);
                    if (emp is not null)
                    {
                        emp.Name= employee.Name;
                        emp.Position= employee.Position;
                        emp.Salary= employee.Salary;
                        return true;
                    }
                }
                return false;
            }
            public static bool DeleteEmployee(int id)
            {
                var employee = employees.FirstOrDefault(x => x.Id == id);
                if (employee is not null)
                {
                    employees.Remove(employee);
                    return true;
                }
                return false;
            }
            public static Employee? GetEmployeeById(int id)
            {
                return employees.FirstOrDefault(x => x.Id == id);  
            }
        }

        public class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public double Salary { get; set; }

            public Employee(int id, string name, string position, double salary)
            {
                Id = id;
                Name = name;
                Position = position;
                Salary = salary;
            }
        }


    }
}

