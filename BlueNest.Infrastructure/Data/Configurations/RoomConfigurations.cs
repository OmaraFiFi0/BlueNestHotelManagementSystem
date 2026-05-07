using BlueNest.Core.Entities.RoomModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Infrastructure.Data.Configurations
{
    internal class RoomConfigurations : BaseConfigurations<Room,int>,IEntityTypeConfiguration<Room>
    {
        public new void Configure(EntityTypeBuilder<Room> builder)
        {
            base.Configure(builder);

            builder.Property(X => X.Id)
                 .UseIdentityColumn(100, 1);

            builder.Property(X => X.Description)
                .HasMaxLength(150);

            builder.Property(X => X.PricePerNight)
                .HasPrecision(18, 2);



        }
    }
}
