using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.BookingDTOs
{
    public class BookingDTO
    {
        public Guid Id { get; set; }

        public string GuestFullName { get; set; } = null!;

        public string GustEmail { get; set; } = null!;

        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
    }
}
