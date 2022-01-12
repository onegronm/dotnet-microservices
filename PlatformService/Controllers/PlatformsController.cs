using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Models;
using PlatformService.Models.DTOs;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBus;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(
            IPlatformRepo repository, 
            IMapper mapper,
            IMessageBusClient messageBus,
            ICommandDataClient commandDataClient)
        {
            _repository = repository;
            _mapper = mapper;
            _messageBus = messageBus;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms()
        {
            var platformItem = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platformItem));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDTO> GetPlatformById(int id)
        {
            var platformItem = _repository.GetPlatformById(id);
            if(platformItem != null) 
            {
                return Ok(_mapper.Map<PlatformReadDTO>(platformItem));
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDTO>> CreatePlatform([FromBody]PlatformCreateDTO platform)
        {
            var platformModel = _mapper.Map<Platform>(platform);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDTO>(platformModel);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Something terrible has happened.", ex);
            }

            // Send async method
            try {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDTO>(platformReadDto);
                platformPublishedDto.Event = "Platform Published";
                _messageBus.PublishNewPlatform(platformPublishedDto);
            }
            catch(Exception ex) {
                Console.WriteLine("Something terrible has happened.", ex);
            }

            return CreatedAtRoute(
                nameof(GetPlatformById),                        
                new { Id = platformReadDto },                   
                platformReadDto                 
            );
        }
    }
}