using BlueNest.Core.Contracts;
using BlueNest.Core.Entities.SecurityModule;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Infrastructure.Data.DataSeed
{
    public class IdentityDataIntializer : IDataIntializer
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityDataIntializer(UserManager<HotelUser> userManager , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task InitializeAdminAndRoleAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                var AdminRole = new IdentityRole() { Name = "Admin"};
                var StaffRole= new IdentityRole() { Name = "Staff"};
                var GuestRole = new IdentityRole() { Name = "Guest"};

                await _roleManager.CreateAsync(AdminRole);
                await _roleManager.CreateAsync(StaffRole);
                await _roleManager.CreateAsync(GuestRole);
            }

            if (!_userManager.Users.Any())
            {
                var Adminuser = new HotelUser()
                {
                    FullName = "Admin.BlueNest",
                    Email = "Admin.BlueNest@gmail.com",
                    UserName = "Admin_BlueNest",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                };
                await _userManager.CreateAsync(Adminuser, "P@ssw0rd");
              await   _userManager.AddToRoleAsync(Adminuser, "Admin");
            }
        }
    }
}
