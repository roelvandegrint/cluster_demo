using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Staffing.Shared;

namespace StaffingService.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private static readonly List<Employee> Employees = new List<Employee>
    {
        new("Roel", "van de Grint", new DateTime(2017,9,1), "IMG_0609.JPG"),
        new("Donald", "Hessing", new DateTime(1900, 5, 4), null),
        new("Gerben", "de Vries", new DateTime(2021, 6, 6), null),
        new("Carl", "in 't Veld", new DateTime(2014, 7, 3), null)
    };

    private readonly ILogger<EmployeesController> _logger;
    private readonly DaprClient _dapr;

    public EmployeesController(ILogger<EmployeesController> logger, DaprClient dapr)
    {
        _logger = logger;
        _dapr = dapr;
    }

    [HttpGet]
    public IEnumerable<Employee> Get()
    {
        Thread.Sleep(1000);
        return Employees;
    }

    [HttpPost]
    public void AddEmployee(Employee employee)
    {
        Employees.Add(employee);
        _dapr.PublishEventAsync<Employee>("events", "new_employees", employee);
    }
}