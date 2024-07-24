using System;
namespace HoneyRaesAPI.Models;

public class Employee
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Speciality { get; set; }
	// Adding service ticket object which will be shown as a array of objects in a get request if it is more than 1
    public List<ServiceTicket>? ServiceTickets { get; set; }
}

