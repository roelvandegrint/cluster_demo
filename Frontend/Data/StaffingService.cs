using Dapr.Client;
using Staffing.Shared;

namespace Frontend.Data;

public class StaffingService
{
    private readonly DaprClient dapr;

    public StaffingService(DaprClient dapr)
    {
        this.dapr = dapr;

    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync() =>
        await dapr.InvokeMethodAsync<IEnumerable<Employee>>(HttpMethod.Get, "staffingsvc-rvdg", "/employees");

    public async Task<Employee> CreateAsync(Employee employee) =>
        await dapr.InvokeMethodAsync<Employee, Employee>(HttpMethod.Post, "staffingsvc-rvdg", "/employees", employee);

    public async Task DeleteEmployeeAsync(string id) =>
        await dapr.InvokeMethodAsync(HttpMethod.Delete, "staffingsvc-rvdg", $"/employees/{id}");
}
