using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Shared.DTOs.AuthDTOs
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage ="Enter Valid Email Address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage ="Password Is Required")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage ="FullName IsRequired")]
        public string FullName { get; set; }=null!;

        [Required(ErrorMessage ="Phone Is Required")]
        [Phone(ErrorMessage ="Enter Vaild Phone Number")]
        public string Phone { get; set; } = null!;


    }
}
