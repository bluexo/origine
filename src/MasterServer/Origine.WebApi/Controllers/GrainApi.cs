using Orleans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Origine.WebService.Controllers
{
    [Authorize]
    [ApiController]
    public abstract class GrainApi<T> : ControllerBase where T : ControllerBase
    {
        protected readonly IClusterClient _clusterClient;
        protected readonly ILogger _logger;

        public GrainApi(IClusterClient clusterClient, ILogger<T> logger)
        {
            _logger = logger;
            _clusterClient = clusterClient;
        }
    }

}