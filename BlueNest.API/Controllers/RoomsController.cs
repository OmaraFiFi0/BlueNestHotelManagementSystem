using BlueNest.Infrastructure.Repository;
using BlueNest.Services.Abstraction;
using BlueNest.Shared.DTOs.QueryParamters;
using BlueNest.Shared.DTOs.RoomDTOs;
using BlueNest.Shared.Reponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlueNest.API.Controllers
{

    public class RoomsController : BaseApiController
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        //GET : BaseUrl/api/rooms/public
        [Authorize(Roles ="Guest")]
        [HttpGet("public")]

        public async Task<ActionResult<GenericResponse<IEnumerable<RoomDTO>>>> GetAllRooms(string? roomType, string? sort)
        {
            var result = await _roomService.GetAllRoomForGuestAsync(roomType, sort);

            return HandleResult(result);
        }

        // GET : BaseUrl/api/Rooms/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GenericResponse<RoomDetailsDTO>>>GetRoomDetails(int id)
        {
            var result = await _roomService.GetRoomDetailsAsync(id);

            return HandleResult(result); 
        }

        // GET : BaseUrl/api/Rooms/admin
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("admin")]

        public async Task<ActionResult<GenericResponse<RoomForAdminDTO>>>GetRoomsForAdmin ([FromQuery]RoomQueryParamters? queryParamters)
        {
            var result = await _roomService.GetAllRoomForAdminOrStaffAsync(queryParamters);

            return HandleResult(result);
        }


        //Post : BaseUrl/api/Rooms
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<bool>>>CreateRoom([FromBody]RoomToCreateDTO createRoom)
        {
            var result = await _roomService.CreateRoomAsync(createRoom);
            return HandleResult(result);
        }

        // Put : BaseUrl/api/Rooms/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]

         public async Task<ActionResult<GenericResponse<bool>>> UpdateRoom([FromRoute] int id ,[FromBody] RoomToUpdateDTO updateRoom)
        {
            var result = await _roomService.UpdateRoomAsync(id ,updateRoom);

            return HandleResult(result);
        }

        //Delete :BaseUrl/api/Rooms/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<GenericResponse<bool>>> DeleteRoom([FromRoute] int roomId)
        {
            var result = await _roomService.DeleteRoomAsync(roomId);

            return HandleResult(result);
        }

        // Post : BaseUrl/api/Rooms/{id}/images
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/images")]
        public async Task<ActionResult<GenericResponse<bool>>>UpdloadRoomImages(int id , [FromForm]List<IFormFile> files)
        {
            var result = await _roomService.UploadImagesAsync(id, files);

            return HandleResult(result);
        }

        // Delete : BaseUrl/api/Rooms/{id}/Images/{imageId}
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}/images/{imageId}")]

        public  async Task<ActionResult<GenericResponse<bool>>>DeleteRoomImage(int id,int imageId)
        {
            var result = await _roomService.DeleteRoomImageAsync(id, imageId);

            return HandleResult(result);
        }

    }
}
