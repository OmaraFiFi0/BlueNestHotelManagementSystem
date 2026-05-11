using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.AuthDTOs
{
    public class ProfileUserDTO
    {
        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
    }
}
