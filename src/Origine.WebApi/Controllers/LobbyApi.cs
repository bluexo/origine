using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using Origine.WebService.Controllers;
using System.Threading.Tasks;

namespace Origine.WebService.Api
{
    [Route("api/lobby")]
    [ApiController]
    public class LobbyApi : GrainApi<LobbyApi>
    {
        public LobbyApi(IClusterClient factory, ILogger<LobbyApi> logger)
            : base(factory, logger)
        {

        }
    }
}