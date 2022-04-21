using System.Text;
using System.Text.Json;
using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.Http;

public class CommandDataClient : ICommandDataClient
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;

    public CommandDataClient(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto plat)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(plat),
            Encoding.UTF8,
            "application/json"
        );

        var resp =
            await _client.PostAsync($"{_configuration["CommandService"]}", httpContent);

        if (resp.IsSuccessStatusCode)
            Console.WriteLine("Success");
        else
        {
            Console.WriteLine("Error");
        }
    }
}