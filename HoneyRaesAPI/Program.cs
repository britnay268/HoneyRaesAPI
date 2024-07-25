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
    new ServiceTicket()
    {
        Id = 6,
        CustomerId = 4,
        Description = "There is a leak in the roof of my car",
        Emergency = true,
    },
    new ServiceTicket()
    {
        Id = 7,
        CustomerId = 6,
        Description = "My tesla stairing loses controller and swerves constantly when driving it",
        Emergency = true,
    },
    new ServiceTicket()
    {
        Id = 8,
        CustomerId = 6,
        EmployeeId = 1,
        Description = "Car break lights are not working and I don't want to get pulled over for it",
        Emergency = true,
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

List<ServiceTicket> EmployeAndCustomerRecord (List<ServiceTicket> Tickets)
{
    foreach (ServiceTicket serviceTicket in Tickets)
    {
        serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
        serviceTicket.Customer = customers.FirstOrDefault(e => e.Id == serviceTicket.CustomerId);
    }

    return Tickets;
};

app.MapGet("/servicetickets", () =>
{
    EmployeAndCustomerRecord(serviceTickets);

    return serviceTickets;
});

// This is done to get service tickets by Id endpoint
app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket? serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = customers.FirstOrDefault(e => e.Id == serviceTicket.CustomerId);
    return serviceTicket == null ? Results.NotFound() : Results.Ok(serviceTicket);
});

// This is done to get all employees endpoint
app.MapGet("/employees", () =>
{
    return employees;
});

// This is done to get all employees by id endpoint
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

// This is done to get all customers by Id endpoint
app.MapGet("/customers/{id}", (int id) =>
{
    Customer? customer = customers.FirstOrDefault(c => c.Id == id);
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return customer == null ? Results.NotFound() : Results.Ok(customer);
});

// This done to get all open service tickets
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

// This is done to create a service ticket
app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

// This is done to remove an existing service ticket
app.MapDelete("servicetickets/{id}", (int id) =>
{
    ServiceTicket? serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    int index = serviceTickets.IndexOf(serviceTicket);
    serviceTickets.RemoveAt(index);
});

//This is done to add new data to an existing service ticket
app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null || id != serviceTicket.Id)
    {
        return id != serviceTicket.Id ? Results.BadRequest() : Results.NotFound();
    }

    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();
});

// This is done to update one entry or key in the servicticket object that is tores in ticketToComplete
app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
});

app.MapGet("/servicetickets/emergencies", () =>
{
    List<ServiceTicket> urgentTickets = serviceTickets.Where(st => st.DateCompleted is null && st.Emergency is true).ToList();

    EmployeAndCustomerRecord(urgentTickets);

    return urgentTickets;
});

app.Run();


