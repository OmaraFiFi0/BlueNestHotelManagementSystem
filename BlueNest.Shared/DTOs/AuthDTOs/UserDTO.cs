using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.AuthDTOs
{
    public class UserDTO
    {
        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
