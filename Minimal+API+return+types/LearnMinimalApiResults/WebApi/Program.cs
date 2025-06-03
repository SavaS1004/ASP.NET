using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WebApi.Models;
using WebApi.Results;
using WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IEmployeesRepository, EmployeesRepository>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
app.UseStatusCodePages();

app.MapEmployeeEndpoints();

app.Run();
