using BlueNest.Infrastructure.Repository;
using BlueNest.Services.Abstraction;
using BlueNest.Shared.DTOs;
using BlueNest.Shared.DTOs.QueryParamters;
using BlueNest.Shared.Reponse;
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
        [HttpGet("public")]

        public async Task<ActionResult<GenericResponse<IEnumerable<RoomDTO>>>> GetAllRooms(string? roomType, string? sort)
        {
            var result = await _roomService.GetAllRoomForGuestAsync(roomType, sort);

            return HandleResult(result);
        }

        // GET : BaseUrl/api/Rooms/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GenericResponse<RoomDetailsDTO>>>GetRoomDetails(int id)
        {
            var result = await _roomService.GetRoomDetailsAsync(id);

            return HandleResult(result); 
        }

        // GET : BaseUrl/api/Rooms/admin
        [HttpGet("admin")]
        public async Task<ActionResult<GenericResponse<RoomForAdminDTO>>>GetRoomsForAdmin ([FromQuery]RoomQueryParamters? queryParamters)
        {
            var result = await _roomService.GetAllRoomForAdminOrStaffAsync(queryParamters);

            return HandleResult(result);
        }


        //Post : BaseUrl/api/Rooms
        [HttpPost]
        public async Task<ActionResult<GenericResponse<bool>>>CreateRoom([FromBody]RoomToCreateDTO createRoom)
        {
            var result = await _roomService.CreateRoomAsync(createRoom);
            return HandleResult(result);
        }

        // Put : BaseUrl/api/Rooms/{id}

        [HttpPut("{id}")]

         public async Task<ActionResult<GenericResponse<bool>>> UpdateRoom([FromRoute] int id ,[FromBody] RoomToUpdateDTO updateRoom)
        {
            var result = await _roomService.UpdateRoomAsync(id ,updateRoom);

            return HandleResult(result);
        }

        //Delete :BaseUrl/api/Rooms/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<GenericResponse<bool>>> DeleteRoom([FromRoute] int roomId)
        {
            var result = await _roomService.DeleteRoomAsync(roomId);

            return HandleResult(result);
        }

        // Post : BaseUrl/api/Rooms/{id}/images
        [HttpPost("{id}/images")]
        public async Task<ActionResult<GenericResponse<bool>>>UpdloadRoomImages(int id , [FromForm]List<IFormFile> files)
        {
            var result = await _roomService.UploadImagesAsync(id, files);

            return HandleResult(result);
        }

        // Delete : BaseUrl/api/Rooms/{id}/Images/{imageId}

        [HttpDelete("{id}/images/{imageId}")]

        public  async Task<ActionResult<GenericResponse<bool>>>DeleteRoomImage(int id,int imageId)
        {
            var result = await _roomService.DeleteRoomImageAsync(id, imageId);

            return HandleResult(result);
        }

    }
}
