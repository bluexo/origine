using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Origine.Models
{
    public enum GenderType
    {
        None,
        Male,
        Female,
        Unknown
    }

    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; } = "Anonymous";

        /// <summary>
        /// 性别
        /// </summary>
        public GenderType Gender { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUri { get; set; }

        /// <summary>
        /// 国家代码
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// 注册时间戳
        /// </summary>
        public DateTime RegisterDateTime { get; set; }

        /// <summary>
        /// 用户口令
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 用户设备
        /// </summary>
        public ICollection<UserDeviceModel> UserDevices { get; set; }
    }
}
