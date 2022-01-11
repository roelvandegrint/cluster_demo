using Dapr.Client;
using Staffing.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDaprClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapGet("/employees", async (DaprClient dapr) =>
{
    var employees = await dapr.GetStateAsync<IEnumerable<Employee>>("staffing", "employees");
    if (employees is null) return new Employee[] { };
    return employees;
});

app.MapPost("/employees", async (Employee employee, DaprClient _dapr) =>
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

    await _dapr.PublishEventAsync<Employee>("events", "new_employees", employee);
    return employee;
});

app.MapDelete("/employees/{id}", async (string id, DaprClient _dapr) =>
{
    var employees = await _dapr.GetStateAsync<IEnumerable<Employee>>("staffing", "employees");

    if (employees is null) return Results.Ok();

    var employeeToGo = employees.FirstOrDefault(e => e.Id == id);
    if (employeeToGo is null) return Results.Ok();

    var newState = employees.Where(e => e.Id != id);
    await _dapr.SaveStateAsync("staffing", "employees", newState);

    await _dapr.PublishEventAsync<Employee>("events", "employee_deleted", employeeToGo);
    return Results.Ok();
});

app.Run();

void SetEmployeeId(Employee employee)
{
    employee.Id = Guid.NewGuid().ToString();
    employee.ProcessedBy = "Minimal API";
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