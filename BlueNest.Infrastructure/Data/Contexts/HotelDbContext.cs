using BlueNest.Core.Entities.BookingModule;
using BlueNest.Core.Entities.SecurityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Infrastructure.Data.Contexts
{
    public class HotelDbContext:IdentityDbContext<HotelUser>
    {

        public HotelDbContext(DbContextOptions<HotelDbContext>options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HotelUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<StaffUser>().ToTable("StaffUsers");
            modelBuilder.Entity<Booking>().Property(B => B.TotalAmount).HasPrecision(8, 2);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
