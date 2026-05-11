using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.AuthDTOs
{
    public class GetUserDTO
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
