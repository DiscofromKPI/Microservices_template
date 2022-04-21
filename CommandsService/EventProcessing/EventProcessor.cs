using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _factory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory factory, IMapper mapper)
    {
        _factory = factory;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notification)
    {
        Console.WriteLine("-->Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notification);

        switch (eventType?.Event)
        {
            case "Platform published":
                Console.WriteLine("--> Platform published Event detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("--> Cannot detect Event");
                return EventType.Undetermined;
        }
    }

    private void AddPlatform(string platformPublishedMsg)
    {
        using (var scope = _factory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMsg);

            try
            {
                var plat = _mapper.Map<Platform>(platformPublishedDto);
                if (!repo.ExternalPlatformExists(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Platform already exists");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Could not add platform " + ex.Message);
            }
        }
    }
}

enum EventType
{
    PlatformPublished,
    Undetermined
}