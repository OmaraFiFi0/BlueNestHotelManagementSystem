using BlueNest.Services.Abstraction;
using BlueNest.Shared.DTOs.AuthDTOs;
using BlueNest.Shared.Reponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueNest.API.Controllers
{
   
    public class AuthController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        //POST : BaseUrl/api/Auth/register
        [HttpPost("register")]
        public async Task<ActionResult<GenericResponse<UserDTO>>>Register([FromBody]RegisterUserDTO registerUserDTO)
        {
            var result = await _authenticationService.RegisterUserAsync(registerUserDTO);

            return HandleResult(result);
        }

        //POST : BaseUrl/api/Auth/login
        [HttpPost("login")]

        public async Task<ActionResult<GenericResponse<UserDTO>>>Login([FromBody]LoginUserDTO loginUserDTO)
        {
            var result = await _authenticationService.LoginAsync(loginUserDTO);
            return HandleResult(result);
        }

        // POST : BaseUrl/api/Auth/Create-Staff
        [Authorize(Roles ="Admin")]
        [HttpPost("Create-Staff")]
        public async Task<ActionResult<GenericResponse<bool>>> CreateStaffUser([FromBody] StaffUserDTO staffUserDTO)
        {
            var result = await _authenticationService.CreateStaffUserAsync(staffUserDTO);
            return HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        // GET : BaseUrl/api/Auth/users
        [HttpGet("users")]

        public async Task<ActionResult<GenericResponse<IEnumerable<GetUserDTO>>>> GetAllUsers()
        {
            var result = await _authenticationService.GetAllUserForAdminAsync();
            return HandleResult(result);
        }


        // PUT : BaseUrl/api/Auth/users/{id}/activate
        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}/activate")]
        public async Task<ActionResult<GenericResponse<bool>>>ActivateUser([FromRoute]string id)
        {
            var result = await _authenticationService.ActivateUser(id);
            return HandleResult(result);
        }
        [Authorize(Roles ="Admin")]
        // PUT : BaseUrl/api/Auth/users/{id}/deactivate
        [HttpPut("users/{id}/deactivate")]

        public async Task<ActionResult<GenericResponse<bool>>> DeActivateUser([FromRoute] string id)
        {
            var result = await _authenticationService.DeActivateUser(id);
            return HandleResult(result);
        }

        //GET : BaseUrl/api/Auth/emailExists
        [HttpGet("emailExists")]
        public async Task<ActionResult<GenericResponse<bool>>>CheckEmail([FromQuery]string email)
        {
            var result = await _authenticationService.EmailExists(email);
            return HandleResult(result);
        }

        [Authorize]
        // Get : BaseUrl/api/Auth/profile
        [HttpGet("profile")]
        public async Task<ActionResult<GenericResponse<ProfileUserDTO>>> GetProfileAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _authenticationService.GetProfileUserAsync(userId!);
            return HandleResult(result);
        }

    }
}
