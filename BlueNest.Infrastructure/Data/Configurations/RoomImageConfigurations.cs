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
    public class RoomImageConfigurations : BaseConfigurations<RoomImage,int>,IEntityTypeConfiguration<RoomImage>
    {
        public new void Configure(EntityTypeBuilder<RoomImage> builder)
        {
           base.Configure(builder);

            builder.Property(X => X.PictureUrl)
                .HasMaxLength(500);
        }
    }
}
