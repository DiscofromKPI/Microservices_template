using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace DefaultNamespace;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly IPlatformRepo _platformRepo;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _dataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformController(IPlatformRepo platformRepo, IMapper mapper, 
        ICommandDataClient dataClient, IMessageBusClient messageBusClient)
    {
        _platformRepo = platformRepo;
        _mapper = mapper;
        _dataClient = dataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(_platformRepo.GetAllPlatforms()));
    }
    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platform = _platformRepo.GetPlatformById(id);

        if (platform is not null)
        {
            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        _platformRepo.CreatePlatform(platformModel);
        _platformRepo.SaveChanges();

        var platform = _mapper.Map<PlatformReadDto>(platformModel);

        try
        {
            await _dataClient.SendPlatformToCommand(platform);
        }
        catch (Exception exception)
        {
            Console.WriteLine("Could not send sync " + exception.Message);
        }
        
        //Send async
        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platform);
            platformPublishedDto.Event = "Platform published";
            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not send async " + ex.Message);
        }
        return CreatedAtRoute(nameof(GetPlatformById), new {platform.Id}, platform);
    }
}