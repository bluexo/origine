using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Providers;
using Orleans.Runtime;


using Origine.Interfaces;
using Origine.Models;

namespace Origine.Grains
{
    [StorageProvider(ProviderName = "MongoDb")]
    public class PlayerProfile : Grain, IPlayerProfile
    {
        readonly IPersistentState<PlayerProfileState> _state;
        readonly ILogger<PlayerProfile> _logger;

        public PlayerProfile(
            [PersistentState(nameof(PlayerProfile))]
            IPersistentState<PlayerProfileState> state, ILogger<PlayerProfile> logger)
        {
            _state = state;
            _logger = logger;
        }

        public Task UpdateNickName(string newName)
        {
            _state.State.NickName = newName;
            _logger.LogInformation($"Update userName {newName}");
            return _state.WriteStateAsync();
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
        }

        public async Task<bool> AddCoupon(int value)
        {
            int tmpCoupon = _state.State.Coupon + value;

            if (tmpCoupon < 0)
                return false;

            _state.State.Coupon = tmpCoupon;
            await _state.WriteStateAsync();

            return true;
        }

        public async Task<bool> AddDiamond(int value)
        {
            int tmpDiamond = _state.State.Diamond + value;

            if (tmpDiamond < 0)
                return false;

            _state.State.Diamond = tmpDiamond;
            await _state.WriteStateAsync();

            return true;
        }

        public async Task<bool> AddGold(int value)
        {
            int tmpGold = _state.State.Gold + value;

            if (tmpGold < 0)
                return false;

            _state.State.Gold = tmpGold;
            await _state.WriteStateAsync();
            return true;
        }



        public async Task<bool> AddExp(int value)
        {
            if (value < 0)
                return false;

            _state.State.Exp += value;
            await _state.WriteStateAsync();

            return true;
        }

        public ValueTask<PlayerProfileState> GetState() => new ValueTask<PlayerProfileState>(_state.State);

    }
}
