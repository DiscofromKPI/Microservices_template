using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using SignalR_Common;
using SignalRServer.Common;

namespace SignalRServer.Hubs;

public class NotificationHub : Hub<INotificationClient>
{
    public Task SendMessage(Message message)
    {
        Debug.WriteLine(Context.ConnectionId);
        
        if(Context.Items.ContainsKey("user_name"))
        {
            message.Title = $"Message from user: {Context.Items["user_name"]}";
        }

        return Clients.Others.Send(message);
    }

    public Task SetName(string name)
    {
        Context.Items.TryAdd("user_name", name);
        return Task.CompletedTask;
    }

    public override Task OnConnectedAsync()
    {
        var message = new Message()
        {
            Title = $"New Client Connected: {Context.ConnectionId}",
        };
        Clients.Others.Send(message);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var message = new Message()
        {
            Title = $"New Client DisConnected: {Context.ConnectionId}",
        };

        Clients.Others.Send(message);
        Clients.Others.Send(message);
        
        return base.OnDisconnectedAsync(exception);
    }
}