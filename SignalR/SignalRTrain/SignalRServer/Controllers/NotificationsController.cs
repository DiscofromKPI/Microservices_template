using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR_Common;
using SignalRServer.Common;
using SignalRServer.Hubs;

namespace SignalRServer.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    
    [HttpPost]
    public async Task<IActionResult> Push([FromBody]Message message, [FromServices] IHubContext<NotificationHub, INotificationClient> hubContext)
    {
        await hubContext.Clients.All.Send(message);
        return Ok();
    }
}