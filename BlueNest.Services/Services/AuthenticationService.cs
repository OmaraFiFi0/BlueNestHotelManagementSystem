using BlueNest.Core.Entities.SecurityModule;
using BlueNest.Services.Abstraction;
using BlueNest.Shared.DTOs.AuthDTOs;
using BlueNest.Shared.Message;
using BlueNest.Shared.Reponse;
using BlueNest.Shared.SharedEnums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        // Q1 : Register Must Be in Try,Catch ..??
        private readonly UserManager<HotelUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IEmailService _emailService;

        public AuthenticationService(UserManager<HotelUser> userManager,IConfiguration configuration ,
            ILogger<AuthenticationService> logger , IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }
       
        public async Task<GenericResponse<UserDTO>> RegisterUserAsync(RegisterUserDTO registerData)
        {
            var genericResponse = new GenericResponse<UserDTO>();

            try
            {
                if (registerData is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No Data To Register New Guset is Provided";

                    return genericResponse;
                }

                var userEmailExist = await _userManager.FindByEmailAsync(registerData.Email);

                if (userEmailExist is not null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "This is Already Existed Email";
                    return genericResponse;
                }

                var hotelUser = new HotelUser()
                {
                    Email = registerData.Email,
                    PhoneNumber = registerData.Phone,
                    FullName = registerData.FullName,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UserName = registerData.Email.Split('@')[0]

                };

                var result = await _userManager.CreateAsync(hotelUser, registerData.Password);

                if (!result.Succeeded)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = string.Join('|', result.Errors.Select(E => E.Description));
                    return genericResponse;

                }

                await _userManager.AddToRoleAsync(hotelUser, "Guest");

                var email = new Email
                {
                    To = registerData.Email,
                    Subject = $"Welcome to HotelSystem, {registerData.FullName}!",
                    Body = $"Dear {registerData.FullName},\n\nThank you for joining HotelSystem. Your account has been successfully created." +
                    " We are delighted to have you with us and look forward to providing you with the best hotel booking experience." +
                    "\n\nBest Regards,\n BlueNest HotelSystem Team"
                };

                await _emailService.SendEmail(email);

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Success To Register New Guest";
                genericResponse.Data = new UserDTO
                {
                    Email = registerData.Email,
                    FullName = registerData.FullName,
                    Token = await CreateTokenAsync(hotelUser)
                };
                return genericResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An unExpected error Occurred While Register ");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Unexpected Error Occurred";

                return genericResponse;
            }
        }

        public async Task<GenericResponse<UserDTO>> LoginAsync(LoginUserDTO loginData)
        {
            var genericResponse = new GenericResponse<UserDTO>();


                if (loginData is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No Login Data Is Provided";
                    return genericResponse;
                }

                var user = await _userManager.FindByEmailAsync(loginData.Email);
                if (user is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status401Unauthorized;
                    genericResponse.Message = "Invaild Email Or Password";
                    return genericResponse;
                }

                if (!user.IsActive)
                {
                    genericResponse.StatusCode = StatusCodes.Status403Forbidden;
                    genericResponse.Message = "Your Account has been DeActivated, Please Contect The Adminstration ";
                    return genericResponse;
                }

                var passwordCorrect = await _userManager.CheckPasswordAsync(user, loginData.Password);
                if (!passwordCorrect)
                {
                    genericResponse.StatusCode = StatusCodes.Status401Unauthorized;
                    genericResponse.Message = "Invaild Email Or Password";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;

                genericResponse.Message = "Login Successfully";

                genericResponse.Data = new UserDTO
                {
                    Email = loginData.Email,
                    FullName = user.FullName,
                    Token = await CreateTokenAsync(user),
                };

                return genericResponse;
        }
            
        

        private async Task<string>CreateTokenAsync(HotelUser user)
        {
            // Token [ Issure - Audience - ExpireDate - Claims - SignInCreditional ] 


            var claim = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim("Activity", user.IsActive.ToString()),
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
                claim.Add(new Claim(ClaimTypes.Role, role));


            var securityKey = _configuration["JwtOptions:SecretKey"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"],
                audience: _configuration["JwtOptions:Audience"],
                expires: DateTime.UtcNow.AddHours(1),
                claims: claim,
                signingCredentials:cred 

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
            // To Validate Part's of Token And Return it As String



        }

        public async Task<GenericResponse<bool>> CreateStaffUserAsync(StaffUserDTO staffData)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                if (staffData is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid Staff Data ";

                    return genericResponse;
                }

                var StaffUserEmail = await _userManager.FindByEmailAsync(staffData.Email);

                if (StaffUserEmail is not null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Email is Already Exist";
                    return genericResponse;
                }

                var resultOfParse =  Enum.TryParse(staffData.Speciality , out StaffSpecialities specialities);
                if (!resultOfParse)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid Staff Specality";
                }
                var NewStaffUser = new StaffUser
                {
                    Email = staffData.Email,
                    PhoneNumber = staffData.Phone,
                    FullName = staffData.FullName,
                    Specialities = specialities,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UserName = staffData.Email.Split('@')[0]
                };

                var result = await _userManager.CreateAsync(NewStaffUser, staffData.Password);

                if (!result.Succeeded)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = string.Join('|', result.Errors.Select(E => E.Description));
                    return genericResponse;
                }

                await _userManager.AddToRoleAsync(NewStaffUser, "Staff");
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Success To Create New Staff User";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unExpected error Occurred While Create Staff");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Unexpected Error Occurred";

                return genericResponse;

            }

        }

        public async Task<GenericResponse<IEnumerable<GetUserDTO>>> GetAllUserForAdminAsync()
        {
            var genericResponse= new GenericResponse<IEnumerable<GetUserDTO>>();

            var users = await _userManager.Users.ToListAsync();

            if(users is null || users.Count == 0)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "No Users To Display";
                return genericResponse;
            }

            var listOfUsersToReturn = new List<GetUserDTO>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    continue;

                var roles = await _userManager.GetRolesAsync(user);
                var UserToRetrunDTO = new GetUserDTO()
                {
                    Email = user.Email!,
                    Id = user.Id,
                    IsActive = user.IsActive,
                    Role = roles.FirstOrDefault()!
                };
                listOfUsersToReturn.Add(UserToRetrunDTO);
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success To Retrive All users [Staff-Guest]";
            genericResponse.Data = listOfUsersToReturn;
            return genericResponse;
        }


        public async Task<GenericResponse<bool>> DeActivateUser(string userId)
        {
            var genericResponse = new GenericResponse<bool>();
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "User Not Exsit to Deactivate ";
                return genericResponse;
            }

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);
            if(!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = string.Join('|', result.Errors.Select(E => E.Description));
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success To Deactivate Guest Or Staff";
            genericResponse.Data = true;

            return genericResponse;
        }
        public async Task<GenericResponse<bool>> ActivateUser(string userId)
        {
            var genericResponse = new GenericResponse<bool>();
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "User Not Exsit to Activate ";
                return genericResponse;
            }

            user.IsActive = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = string.Join('|', result.Errors.Select(E => E.Description));
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success To Activate Guest Or Staff";
            genericResponse.Data = true;

            return genericResponse;
        }

        public async Task<GenericResponse<bool>> EmailExists(string email)
        {
            var genericResponse = new GenericResponse<bool>();

            var user = await _userManager.FindByEmailAsync(email);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = " Success To Check On Email";
            genericResponse.Data = user is not null ? true : false;

            return genericResponse; 
        }

        public async Task<GenericResponse<ProfileUserDTO>> GetProfileUserAsync(string userId)
        {
            var genericResponse= new GenericResponse<ProfileUserDTO>();

            var user = await _userManager.FindByIdAsync(userId);

            if(user is null)
            {
                genericResponse.StatusCode= StatusCodes.Status404NotFound;
                genericResponse.Message = "Profile Not Found";

                return genericResponse;
            }

            var profileDataToReturn = new ProfileUserDTO()
            {
                Email = user.Email!,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber!,
                UserName = user.UserName!
            };

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success To Retrive User Profile";
            genericResponse.Data = profileDataToReturn;

            return genericResponse;
        }
    }

    
}
