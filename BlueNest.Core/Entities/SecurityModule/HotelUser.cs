using BlueNest.Core.Entities.BookingModule;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Core.Entities.SecurityModule
{
    public class HotelUser:IdentityUser
    {
        public string FullName { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Booking> GuestBookings { get; set; } = [];
    }
}
