using Microsoft.AspNetCore.Mvc;
using Staffing.Shared;

namespace StaffingService.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private static readonly Employee[] Employees = new Employee[]
    {
        new("Roel", "van de Grint", new DateTime(2017,9,1), "IMG_0609.JPG"),
        new("Donald", "Hessing", new DateTime(1900, 5, 4), null),
        new("Gerben", "de Vries", new DateTime(2021, 6, 6), null),
        new("Carl", "in 't Veld", new DateTime(2014, 7, 3), null)
    };

    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(ILogger<EmployeesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<Employee> Get()
    {
        Thread.Sleep(1000);
        return Employees;
    }
}
