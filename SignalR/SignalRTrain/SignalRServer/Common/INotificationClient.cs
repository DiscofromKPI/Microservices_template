using SignalR_Common;

namespace SignalRServer.Common;

public interface INotificationClient
{
    Task Send(Message message);
}