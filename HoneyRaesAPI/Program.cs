using System.Collections.Generic;
using HoneyRaesAPI.Models;
using System;

List<Customer> customers = new List<Customer>
{
    new Customer()
    {
        Id = 2,
        Name = "Chelsea Gray",
        Address = "45 Ave Park"
    },
    new Customer()
    {
        Id = 4,
        Name = "Josh Nickles",
        Address = "67 Buick Blvd"
    },
    new Customer()
    {
        Id = 6,
        Name = "Daniel Vibe",
        Address = "2 Anarchy Street"
    },
};
List<Employee> employees = new List<Employee>
{
    new Employee()
    {
        Id = 1,
        Name = "Tomilton Vivic",
        Speciality = "Electric"
    },
    new Employee()
    {
        Id = 3,
        Name = "Yona Vue",
        Speciality = "Hardware/Body Mechanic"
    },
};
List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "My screen in my car will not turn on when my car starts",
        Emergency = false,
        DateCompleted = new DateTime(2, 2, 2024);
    },
    new ServiceTicket()
    {
        Id = 10,
        CustomerId = 4,
        Description = "My windsheild is shattered. The glass is all over in the car and is undrivable currently",
        Emergency = true,
        DateCompleted = new DateTime(8, 12, 2023);
    },
    new ServiceTicket()
    {
        Id = 15,
        CustomerId = 6,
        EmployeeId = 1,
        Description = "My tesla cameras are glitching while I am driving and I would like them to work",
        Emergency = false,
    },
    new ServiceTicket()
    {
        Id = 20,
        CustomerId = 2,
        EmployeeId = 3,
        Description = "I got into a car crash and my car is dented all over - I need some body work done on it.",
        Emergency = false,
        DateCompleted = new DateTime(1, 15, 2024);
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 4,
        EmployeeId = 1,
        Description = "I think something is wrong with my spark plugs",
        Emergency = true,
        DateCompleted = new DateTime(7, 7, 2024);
    }
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/hello", () =>
{
    return "hello";
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

