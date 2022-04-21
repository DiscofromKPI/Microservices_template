using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace DefaultNamespace;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly ICommandRepo _repo;
    private readonly IMapper _mapper;

    public PlatformController(ICommandRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting platforms");

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(_repo.GetAllPlatforms()));
    }

    [HttpPost]
    public ActionResult Test()
    {
        Console.WriteLine("--OK");
        return Ok();
    }
}