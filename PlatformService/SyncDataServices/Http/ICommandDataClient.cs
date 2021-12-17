using System.Threading.Tasks;
using PlatformService.Models.DTOs;

namespace PlatformService.SyncDataServices.Http {
    public interface ICommandDataClient {
        Task SendPlatformToCommand(PlatformReadDTO plat);
    }
}