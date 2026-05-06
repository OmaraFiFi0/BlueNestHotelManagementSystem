using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.QueryParamters
{
    public class RoomQueryParamters
    {
        public string? roomType { get; set; }

        public string? roomStatus { get; set; }

        public string? Sort { get; set; }
    }
}
