using Dapr.Client;
using Frontend.Models;

namespace Frontend.Data;

public class StaffingService
{
    private readonly DaprClient dapr;

    public StaffingService(DaprClient dapr)
    {
        this.dapr = dapr;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync() =>
        await dapr.InvokeMethodAsync<IEnumerable<Employee>>(HttpMethod.Get, "staffingsvc", "/employees");

    public async Task<Employee> CreateAsync(Employee employee) =>
        await dapr.InvokeMethodAsync<Employee, Employee>(HttpMethod.Post, "staffingsvc", "/employees", employee);

    public async Task DeleteEmployeeAsync(string id) =>
        await dapr.InvokeMethodAsync(HttpMethod.Delete, "staffingsvc", $"/employees/{id}");
}
