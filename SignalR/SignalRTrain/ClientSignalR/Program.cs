using Microsoft.AspNetCore.SignalR.Client;
using SignalR_Common;

HubConnection connection;

await InitSignalRConnection();

bool isExited = false;

while (!isExited)
{
    Console.WriteLine("Enter a command or message: ");
    var input = Console.ReadLine();
    if(string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (input == "setname")
    {
        Console.WriteLine("Enter your name: ");
        var name = Console.ReadLine();
        if(string.IsNullOrWhiteSpace(name))
        {
            continue;
        }
        
        await connection.SendAsync("SetName", name);
        Console.WriteLine("Name saved");
    }
    else if (input == "Exit")
    {
        isExited = true;
    }
    else
    {
        var message = new Message()
        {
            Title = "Message from client",
            Body = input
        };
        await connection.SendAsync("SendMessage", message);
    }
        
}

Task InitSignalRConnection()
{
    connection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7012/notification")
        .Build();
    
    connection.On<Message>("Send", (message) =>
    {
        Console.WriteLine("Message from server " + message.Title);

        Console.WriteLine(message.Body);
    });
    
    return connection.StartAsync();
}