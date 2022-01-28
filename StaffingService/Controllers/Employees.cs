using System.Text.Json;
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
    public async Task<IEnumerable<Employee?>> GetAsync()
    {
        var employeeKeys = await _dapr.GetStateAsync<IReadOnlyList<string>>("staffing", "employeeKeys");
        if (employeeKeys is null || employeeKeys.Count == 0) return Array.Empty<Employee>();
        var result = await _dapr.GetBulkStateAsync("staffing", employeeKeys, null);
        return result.Select(item => JsonSerializer.Deserialize<Employee>(item.Value));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetByIdAsync(string id)
    {
        var employee = await _dapr.GetStateAsync<Employee>("staffing", id);
        if (employee is null) return NotFound();
        return employee;
    }

    [HttpPost]
    public async Task<Employee> AddEmployeeAsync(Employee employee)
    {
        var employeeKeys = await _dapr.GetStateAsync<IEnumerable<string>>("staffing", "employeeKeys");
        if (employeeKeys is null)
        {
            employeeKeys = new List<string> { employee.Id! };
        }

        // TODO: Handle ETAG here
        var transactions = new List<StateTransactionRequest>
        {
            new StateTransactionRequest("employeeKeys", JsonSerializer.SerializeToUtf8Bytes(employeeKeys), StateOperationType.Upsert),
            new StateTransactionRequest($"employee_{employee.Id}", JsonSerializer.SerializeToUtf8Bytes(employee), StateOperationType.Upsert)
        };

        await _dapr.ExecuteStateTransactionAsync("staffing", transactions);

        _logger.LogInformation("New employee created, sending out event");
        await _dapr.PublishEventAsync<Employee>("events", "new_employees", employee);
        return employee;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployeeASync(string id)
    {
        var employeeKeys = await _dapr.GetStateAsync<IEnumerable<string>>("staffing", "employeeKeys");
        employeeKeys = employeeKeys.Where(e => e != id);

        // TODO: Handle ETAG here
        var transactions = new List<StateTransactionRequest> {
            new StateTransactionRequest("employeeKeys", JsonSerializer.SerializeToUtf8Bytes(employeeKeys), StateOperationType.Upsert),
            new StateTransactionRequest(id, null, StateOperationType.Delete)
        };

        await _dapr.ExecuteStateTransactionAsync("staffing", transactions);
        await _dapr.PublishEventAsync<string>("events", "employee_deleted", id);
        return Ok();
    }
}