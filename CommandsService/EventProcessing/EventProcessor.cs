using System;
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper =  mapper;
        }
        
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType) {
                case EventType.PlatformPublished:

                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage) {
            Console.WriteLine("Determining event");

            var eventType = JsonSerializer.Deserialize<EventDto>(notificationMessage);

            switch (eventType.Event) {
                case "Platform Published":
                    Console.WriteLine("Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("Could not determine the event type");
                    return EventType.Undertermined;
            }
        }

        private void addPlatform(string platformPublishedMessage) {
            using (var scope = _scopeFactory.CreateScope()) {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if(!repo.ExternalPlatformExists(plat.ExternalId))
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
                    Console.WriteLine($"Could not add platform to DB {ex.Message}");
                }
            }
        }
    }

    enum EventType {
        PlatformPublished,
        Undertermined
    }
}