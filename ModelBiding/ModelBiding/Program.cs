

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using ModelBiding.Models;
using System.Net.Sockets;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async (HttpContext context) =>
    {
        await context.Response.WriteAsync("Welcome to the home page.");
    });

    endpoints.MapGet("/people", (Person? p) =>
    {
        return $"Id:{p?.Id},name:{p?.Name}";
    });

    //endpoints.MapGet("/employees", async (HttpContext context) =>
    //{
    //    // Get all of the employees' information
    //    var employees = EmployeesRepository.GetEmployees();

    //    context.Response.ContentType = "text/html";
    //    await context.Response.WriteAsync("<h2>Employees</h2>");
    //    await context.Response.WriteAsync("<ul>");
    //    foreach (var employee in employees)
    //    {
    //        await context.Response.WriteAsync($"<li><b>{employee.Name}</b>: {employee.Position}</li>");
    //    }
    //    await context.Response.WriteAsync("</ul>");

    //});

    endpoints.MapGet("/employees/{id:int}",([AsParameters]GetEmployeeParameters param)  =>
    {    
       
            var employee = EmployeesRepository.GetEmployeeById(param.Id);
        employee.Name = param.Name;
        employee.Position = param.Position;
        return employee;





    });

    endpoints.MapPost("/employees", (Employee employee) =>
    {

            if (employee is null || employee.Id <= 0)
            {
                
                return "Employee added successfully.";
            }
            EmployeesRepository.AddEmployee(employee);
            return"Employee added successfully";
        

    });

    endpoints.MapPut("/employees", async (HttpContext context) =>
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
            await context.Response.WriteAsync("Employee not found.");
        }
    });

    endpoints.MapDelete("/employees/{id}", async (HttpContext context) =>
    {

        var id = context.Request.RouteValues["id"];
        var employeeId = int.Parse(id.ToString());

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
            await context.Response.WriteAsync("You are not authorized to delete.");
        }

    });

});

app.Run();

class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public static ValueTask<Person? > BindAsync(HttpContext context)
    {
        var idStr = context.Request.Query["id"];
        var nameStr = context.Request.Query["name"];
        if(int.TryParse(idStr, out var id) )
        {
            return new ValueTask<Person?>(new Person { Id = id, Name = nameStr });
        }
        return new ValueTask<Person?>(Task.FromResult<Person?>(null));
    }

}
struct GetEmployeeParameters
{
    public int Id { get; set; }
    public string? Name { get; set; }
    [FromHeader]
    public string? Position { get; set; }

}






