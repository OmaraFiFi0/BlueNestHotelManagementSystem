using BlueNest.Shared.DTOs.AuthDTOs;
using BlueNest.Shared.Reponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Abstraction
{
    public interface IAuthenticationService
    {
        Task<GenericResponse<UserDTO>>RegisterUserAsync(RegisterUserDTO registerData);

        Task<GenericResponse<UserDTO>> LoginAsync(LoginUserDTO loginData);

        Task<GenericResponse<bool>> CreateStaffUserAsync(StaffUserDTO staffData);

        Task<GenericResponse<IEnumerable<GetUserDTO>>> GetAllUserForAdminAsync();

        Task<GenericResponse<bool>> ActivateUser(string userId);
        Task<GenericResponse<bool>> DeActivateUser(string userId);

        Task<GenericResponse<bool>> EmailExists(string email);

        Task<GenericResponse<ProfileUserDTO>>GetProfileUserAsync(string userId); 
    }
}
