using BlueNest.Shared.SharedEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.RoomDTOs
{
    public class RoomToCreateDTO
    {

        [Required(ErrorMessage ="RoomType Is Required")]
        public RoomType RoomType { get; set; }

        [Required(ErrorMessage ="Description Is Required")]
        [MaxLength(200)]
        public string Description { get; set; } = null!;
        [Required(ErrorMessage ="Price Per Night Is Required")]
        [Range(0 , double.MaxValue,ErrorMessage ="Price Per Night Must Be a Positive Value")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage ="Amenities Is Required")]
        public string Amenities { get; set; } = null!;
    }
}
