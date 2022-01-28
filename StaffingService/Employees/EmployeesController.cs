using System.Text.Json;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace StaffingService.Employees;

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
    public async Task<IEnumerable<Employee?>> GetAsync()
    {
        var employeeKeys = await _dapr.GetStateAsync<IEnumerable<string>>("staffing", "employeeKeys");
        if (employeeKeys is null || !employeeKeys.Any())
        {
            return Array.Empty<Employee>();
        }

        var prefixedKeys = employeeKeys.Select(e => $"employee_{e}").ToList();
        var result = await _dapr.GetBulkStateAsync("staffing", prefixedKeys, null);
        return result.Select(item => JsonSerializer.Deserialize<Employee>(item.Value));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetByIdAsync(string id)
    {
        var employee = await _dapr.GetStateAsync<Employee>("staffing", $"employee_{id}");
        if (employee is null) return NotFound();
        return employee;
    }

    [HttpPost]
    public async Task<Employee> AddEmployeeAsync(Employee employee)
    {
        var newEmployee = employee with
        {
            Id = Guid.NewGuid().ToString(),
            ProcessedBy = Environment.GetEnvironmentVariable("RVDG_ENVIRONMENT_NAME"),
            Picture = DeterminePicture(employee.FirstName!),
            JoinedOn = DateTime.Now.Date
        };

        var employeeKeys = await _dapr.GetStateAsync<List<string>>("staffing", "employeeKeys");
        if (employeeKeys is null) employeeKeys = new List<string>();
        employeeKeys.Add(newEmployee.Id);

        // TODO: Handle ETAG here
        var transactions = new List<StateTransactionRequest>
        {
            new StateTransactionRequest("employeeKeys", JsonSerializer.SerializeToUtf8Bytes(employeeKeys), StateOperationType.Upsert),
            new StateTransactionRequest($"employee_{newEmployee.Id}", JsonSerializer.SerializeToUtf8Bytes(newEmployee), StateOperationType.Upsert)
        };

        await _dapr.ExecuteStateTransactionAsync("staffing", transactions);

        _logger.LogInformation("New employee created, sending out event");
        await _dapr.PublishEventAsync<Employee>("events", "new_employees", newEmployee);
        return employee;
    }

    private string? DeterminePicture(string name)
    {
        switch (name.ToUpperInvariant())
        {
            case "ROEL":
                return "Roel.jpg";
            case "DONALD":
                return "Donald.png";
            default:
                return null;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployeeASync(string id)
    {
        var employeeKeys = await _dapr.GetStateAsync<IEnumerable<string>>("staffing", "employeeKeys");
        employeeKeys = employeeKeys.Where(e => e != id);

        // TODO: Handle ETAG here
        var transactions = new List<StateTransactionRequest> {
            new StateTransactionRequest("employeeKeys", JsonSerializer.SerializeToUtf8Bytes(employeeKeys), StateOperationType.Upsert),
            new StateTransactionRequest($"employee_{id}", null, StateOperationType.Delete)
        };

        await _dapr.ExecuteStateTransactionAsync("staffing", transactions);
        await _dapr.PublishEventAsync<string>("events", "employee_deleted", id);
        return Ok();
    }
}