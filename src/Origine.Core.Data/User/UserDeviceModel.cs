using System;

namespace PlayCity.Models
{
    /// <summary>
    /// 用户设备
    /// </summary>
    public class UserDeviceModel
    {
        public string Id { get; set; }

        /// <summary>
        /// 设备名
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Mac   
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// 手机操作系统
        /// </summary>
        public string OS { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime FirstUseDateTime { get; set; }

        /// <summary>
        /// 最后使用时间 
        /// </summary>
        public DateTime LastUserDateTime { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public ApplicationUser User { get; set; }
    }
}
