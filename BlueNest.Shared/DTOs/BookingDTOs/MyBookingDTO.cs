using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.BookingDTOs
{
    public class MyBookingDTO
    {
        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public string Status { get; set; } = null!;

        public decimal TotalAmount { get; set; }
    }
}
