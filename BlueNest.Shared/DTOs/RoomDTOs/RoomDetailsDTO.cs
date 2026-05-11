using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.RoomDTOs
{
    public class RoomDetailsDTO
    {

        public string RoomType { get; set; } = null!;

        public string Description { get; set; }=null!;

        public decimal PricePerNight { get; set; }

        public string Amenities { get; set; } = null!;

        public List<string> ImageUrls { get; set; } = [];

        public string RoomStatus { get; set; } = null!;

    }
}
