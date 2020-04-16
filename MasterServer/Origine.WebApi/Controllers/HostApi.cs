using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace Origine.WebService.Controllers
{
    [Route("api/host")]
    public class HostApi : GrainApi<HostApi>
    {
        public HostApi(IClusterClient factory, ILogger<HostApi> loggerFactory)
            : base(factory, loggerFactory)
        {
        }

        [HttpGet("gateways")]
        public async Task<IActionResult> GetGateways()
        {
            var mgt = _clusterClient.GetGrain<IManagementGrain>(0);
            var gateways = await mgt.GetSimpleGrainStatistics();
            return new JsonResult(gateways);
        }

        [HttpGet("silos")]
        public async Task<IActionResult> GetSilos()
        {
            var mgt = _clusterClient.GetGrain<IManagementGrain>(0);
            var silos = await mgt.GetHosts();
            return new JsonResult(silos);
        }
    }
}
