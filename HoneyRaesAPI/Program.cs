using HoneyRaesAPI.Models;
using System.Collections.Generic;
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
        Id = 1,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "My screen in my car will not turn on when my car starts",
        Emergency = false,
        DateCompleted = new DateTime(2024, 2, 4),
    },
    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 4,
        Description = "My windsheild is shattered. The glass is all over in the car and is undrivable currently",
        Emergency = true,
        DateCompleted = new DateTime(2024, 12, 20),
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 6,
        EmployeeId = 1,
        Description = "My tesla cameras are glitching while I am driving and I would like them to work",
        Emergency = false,
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 2,
        EmployeeId = 3,
        Description = "I got into a car crash and my car is dented all over - I need some body work done on it.",
        Emergency = false,
        DateCompleted = new DateTime(2024, 1, 15),
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 4,
        EmployeeId = 1,
        Description = "I think something is wrong with my spark plugs",
        Emergency = true,
        DateCompleted = new DateTime(2024, 7, 13),
    },
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

// Get all service tickets endpoint
app.MapGet("/servicetickets", () =>
{
    foreach (ServiceTicket serviceTicket in serviceTickets)
    {
        serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
        serviceTicket.Customer = customers.FirstOrDefault(e => e.Id == serviceTicket.CustomerId);
    }
    return serviceTickets;
});

// Get service tickets by Id endpoint
app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket? serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = customers.FirstOrDefault(e => e.Id == serviceTicket.CustomerId);
    return serviceTicket == null ? Results.NotFound() : Results.Ok(serviceTicket);
});

// Get all employees endpoint
app.MapGet("/employees", () =>
{
    return employees;
});

// Get all employees by id endpoint
app.MapGet("/employees/{id}", (int id) =>
{
    // Gets employee by Id
    Employee? employee = employees.FirstOrDefault(e => e.Id == id);
    // Gets serviceTickets where the serviceTickets employeeId matches employee's Id
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return employee == null ? Results.NotFound() : Results.Ok(employee);
});

// Get all customers endpoint
app.MapGet("/customers", () =>
{
    return customers;
});

// Get all customers by Id endpoint
app.MapGet("/customers/{id}", (int id) =>
{
    Customer? customer = customers.FirstOrDefault(c => c.Id == id);
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return customer == null ? Results.NotFound() : Results.Ok(customer);
});

// Get all open service tickets
app.MapGet("/servicetickets/open", () =>
{
    List<ServiceTicket> serviceTicket = serviceTickets
    .Where(st => st.EmployeeId is null)
    .Select(st => {
        st.Employee = employees.FirstOrDefault(e => e.Id == st.EmployeeId);
        st.Customer = customers.FirstOrDefault(c => c.Id == st.CustomerId);
        return st;
        }).ToList();
 
    return serviceTicket is null ? Results.NotFound() : Results.Ok(serviceTicket);
});

// Create a service ticket
app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("servicetickets/{id}", (int id) =>
{
    ServiceTicket? serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    int index = serviceTickets.IndexOf(serviceTicket);
    serviceTickets.RemoveAt(index);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

