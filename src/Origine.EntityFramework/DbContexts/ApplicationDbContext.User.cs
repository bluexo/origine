using Microsoft.EntityFrameworkCore;
using Origine.Models;

namespace Origine.StorageProviders
{
    public partial class ApplicationDbContext
    {
        /// <summary>
        /// 用户设备
        /// </summary>
        public DbSet<UserDeviceModel> UserDevices { get; set; }
    }
}
