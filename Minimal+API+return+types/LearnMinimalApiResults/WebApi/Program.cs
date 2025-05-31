using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WebApi.Models;
using WebApi.Results;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
app.UseStatusCodePages();


app.MapGet("/", HtmlResult() =>

{
    string html = "<h2>Welcome to our API</h2> Our API is used to learn ASP.NET CORE";
    return new HtmlResult(html);
});

app.MapGet("/employees", () =>
{
    var employees = EmployeesRepository.GetEmployees();

    return TypedResults.Ok(employees);
});

app.MapGet("/employees/{id:int}", (int id) =>
{
    var employee = EmployeesRepository.GetEmployeeById(id);
    if (employee is null)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            {"id",new[] {$"Employee with the {employee.Id} does not exist."} }
        }, statusCode:404);
    }
    return TypedResults.Ok(employee);
});

app.MapPost("/employees", (Employee employee) =>
{
    if (employee is null)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            {"id",new[] {"Employee is not provided or is not valid"} }
        }, statusCode:400);
    }

    EmployeesRepository.AddEmployee(employee);
    return TypedResults.Created($"/employees/{employee.Id}",employee);

}).WithParameterValidation();


app.MapPut("/employees/{id:int}", (int id, Employee employee) =>
{
    if (employee is null || employee.Id != id)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            {"id",new[] {$"Employee id:{employee.Id} does not exist."} }
        });
    }
    return EmployeesRepository.UpdateEmployee(employee)
    ? TypedResults.NoContent()
    : Results.ValidationProblem(new Dictionary<string, string[]>
    {
        { "id",new[] {$"Employee with id:{employee.Id} does not exist."} }
        }, statusCode:404 );     
});

app.MapDelete("/employees/{id:int}", (int id) =>
{
    var employee = EmployeesRepository.GetEmployeeById(id);
    return EmployeesRepository.DeleteEmployee(employee)
    ? TypedResults.NoContent()
    : Results.ValidationProblem(new Dictionary<string, string[]>
    {
        {"id",new[] {$"Employee with id:{id} does not exist."} }
    }, statusCode: 404);
}).WithParameterValidation();



app.Run();
