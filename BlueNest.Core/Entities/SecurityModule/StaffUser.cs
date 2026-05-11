using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Core.Entities.SecurityModule
{
    public class StaffUser:HotelUser
    {

        public StaffSpecialities Specialities { get; set; }
    }
}
