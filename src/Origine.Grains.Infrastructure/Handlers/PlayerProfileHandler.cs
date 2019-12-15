using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orleans;
using Origine.Authorization;
using Origine.Interfaces;

namespace Origine.Grains.CommandHandlers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class PlayerProfileHandler : JsonCommandHandler
    {
        public PlayerProfileHandler()
        {

        }

    }
}
