using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Orleans;
using Origine.Models;
using System.Linq;

namespace Origine.Interfaces
{
    public class PlayerProfileState
    {
        public string NickName { get; set; } = "Anonymous"; //昵称
        public int IconID { get; set; } //角色图标ID
        public int Level { get; set; } //等级
        public int Exp { get; set; } //经验
        public int Coupon { get; set; } //点券
        public int Diamond { get; set; } //钻石
        public int Gold { get; set; } //金币
        public int DisplayMechaID { get; set; } //展示机甲ID
    }

    public interface IPlayerProfile : IPersistableWithStringKey<PlayerProfileState>
    {
        /// <summary>
        /// 更新昵称
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        Task UpdateNickName(string newName);

        Task<bool> AddCoupon(int value);

        Task<bool> AddDiamond(int value);

        Task<bool> AddGold(int value);

        Task<bool> AddExp(int value);
    }
}
