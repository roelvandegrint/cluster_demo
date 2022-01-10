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
        await dapr.InvokeMethodAsync<IEnumerable<Employee>>(HttpMethod.Get, "staffingsvc", "/employees");

    public async Task<Employee> CreateAsync(Employee employee) =>
        await dapr.InvokeMethodAsync<Employee, Employee>(HttpMethod.Post, "staffingsvc", "/employees", employee);
}
