using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Origine.Models;

namespace Origine.StorageProviders.Configuration
{
    /// <summary>
    /// 用户实体配置
    /// </summary>
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(p => p.RegisterDateTime)
             .IsRequired()
             .HasColumnType("Date")
             .HasDefaultValueSql("Now()");

            builder.HasMany(user => user.UserDevices)
                .WithOne(userDevice => userDevice.User);
        }
    }

    /// <summary>
    /// 用户设备实体配置
    /// </summary>
    public class UserDeviceEntityTypeConfiguration : IEntityTypeConfiguration<UserDeviceModel>
    {
        public void Configure(EntityTypeBuilder<UserDeviceModel> builder)
        {
            builder.Property(p => p.FirstUseDateTime)
               .IsRequired()
               .HasColumnType("Date")
               .HasDefaultValueSql("Now()");
        }
    }
}
