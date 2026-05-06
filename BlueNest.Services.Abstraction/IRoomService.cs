using BlueNest.Shared.DTOs;
using BlueNest.Shared.DTOs.QueryParamters;
using BlueNest.Shared.Reponse;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Abstraction
{
    public interface IRoomService
    {
        Task<GenericResponse<IEnumerable<RoomDTO>>> GetAllRoomForGuestAsync(string? roomType, string? sort);

        Task<GenericResponse<RoomDetailsDTO>> GetRoomDetailsAsync(int roomId);

        Task<GenericResponse<IEnumerable<RoomForAdminDTO>>> GetAllRoomForAdminOrStaffAsync(RoomQueryParamters? queryParamters);

        Task<GenericResponse<bool>> CreateRoomAsync(RoomToCreateDTO createRoom);

        Task<GenericResponse<bool>> UpdateRoomAsync(int roomId, RoomToUpdateDTO updateRoom);

        Task<GenericResponse<bool>> DeleteRoomAsync(int roomId);

        Task<GenericResponse<bool>> UploadImagesAsync(int roomId , List<IFormFile> files);

        Task<GenericResponse<bool>> DeleteRoomImageAsync(int roomId, int imageId);
    
    }
}
