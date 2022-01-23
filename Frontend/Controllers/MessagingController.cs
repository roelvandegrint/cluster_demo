using Dapr;
using FrontEnd.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Staffing.Shared;

namespace Frontend.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagingController : ControllerBase
{

    private readonly ILogger<MessagingController> _logger;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public MessagingController(
        ILogger<MessagingController> logger,
        IHubContext<NotificationsHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    // [Topic("events", "new_employees")]
    [HttpPost]
    public async Task<IActionResult> NewJoiner(Employee employee)
    {
        _logger.LogInformation("Someone new has joined the company!");
        await _hubContext.Clients.All.SendAsync("EmployeeJoined", employee);
        return Ok();
    }
}