using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Staffing.Shared;

namespace StaffingService.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ILogger<EmployeesController> _logger;
    private readonly DaprClient _dapr;

    public EmployeesController(ILogger<EmployeesController> logger, DaprClient dapr)
    {
        _logger = logger;
        _dapr = dapr;
    }

    [HttpGet]
    public async Task<IEnumerable<Employee>> GetAsync()
    {
        var employees = await _dapr.GetStateAsync<IEnumerable<Employee>>("staffing", "employees");
        if (employees is null) return new Employee[] { };
        return employees;
    }

    [HttpGet("{id}")]
    public async Task<Employee?> GetByIdAsync(string id)
    {
        var employees = await _dapr.GetStateAsync<IEnumerable<Employee>>("staffing", "employees");
        return employees?.FirstOrDefault(e => e.Id == id);
    }

    [HttpPost]
    public async Task<Employee> AddEmployeeAsync(Employee employee)
    {
        SetEmployeeId(employee);
        var employees = await _dapr.GetStateAsync<IEnumerable<Employee>>("staffing", "employees");
        if (employees is null)
        {
            await _dapr.SaveStateAsync("staffing", "employees", new Employee[] { employee });
        }
        else
        {
            var newState = employees.ToList();
            newState.Add(employee);
            await _dapr.SaveStateAsync("staffing", "employees", newState);
        }

        _logger.LogInformation("New employee created, sending out event");
        await _dapr.PublishEventAsync<Employee>("events", "new_employees", employee);
        return employee;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployeeASync(string id)
    {
        var employees = await _dapr.GetStateAsync<IEnumerable<Employee>>("staffing", "employees");

        if (employees is null) return Ok();

        var employeeToGo = employees.FirstOrDefault(e => e.Id == id);
        if (employeeToGo is null) return Ok();

        var newState = employees.Where(e => e.Id != id);
        await _dapr.SaveStateAsync("staffing", "employees", newState);

        await _dapr.PublishEventAsync<Employee>("events", "employee_deleted", employeeToGo);
        return Ok();
    }

    private void SetEmployeeId(Employee employee)
    {
        employee.Id = Guid.NewGuid().ToString();
        switch (employee.FirstName)
        {
            case "Roel":
                employee.Picture = "Roel.jpg";
                return;
            case "Donald":
                employee.Picture = "Donald.png";
                return;
            default:
                return;
        }
    }
}