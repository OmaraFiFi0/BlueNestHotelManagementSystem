using BlueNest.Shared.SharedEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.RoomDTOs
{
    public class RoomToUpdateDTO
    {


        public RoomType RoomType { get; set; }

        public string Description { get; set; }

        public decimal PricePerNight { get; set; }

        public string Amenities { get; set; }

        public RoomStatus RoomStatus { get; set; }


    }
}
