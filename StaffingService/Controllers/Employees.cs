using Microsoft.AspNetCore.Mvc;

namespace StaffingService.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private static readonly Employee[] Employees = new Employee[]
    {
        new("Roel", "van de Grint", "16-01-1985"),
        new("Donald", "Hessing", "16-01-1985"),
        new("Gerben", "De Vries", "16-01-1985"),
        new("Carl", "In 't Veld", "16-01-1985")
    };

    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(ILogger<EmployeesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<Employee> Get()  {
        Thread.Sleep(1000);
        return Employees;
    }

    public record Employee(string firstName, string lastName, string DateOfBirth);
}
