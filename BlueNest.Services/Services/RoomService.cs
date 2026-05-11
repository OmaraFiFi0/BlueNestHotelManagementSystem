using AutoMapper;
using BlueNest.Core.Contracts;
using BlueNest.Core.Entities.RoomModule;
using BlueNest.Services.Abstraction;
using BlueNest.Shared.DTOs.QueryParamters;
using BlueNest.Shared.DTOs.RoomDTOs;
using BlueNest.Shared.Reponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;
        private readonly IAttachmentService _attachmentService;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper,ILogger<RoomService> logger , IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _attachmentService = attachmentService;
        }

 

        public async Task<GenericResponse<IEnumerable<RoomForAdminDTO>>> GetAllRoomForAdminOrStaffAsync(RoomQueryParamters? queryParamters)
        {
            var genericResponse = new GenericResponse<IEnumerable<RoomForAdminDTO>>();
            IEnumerable<Room>?rooms = null;
            if(queryParamters is not null)
            {
                Enum.TryParse(queryParamters.roomType, out Core.Entities.RoomModule.RoomType roomTypeEnum);
                Enum.TryParse(queryParamters.roomStatus, out RoomStatus roomStatusEnum);

                Expression<Func<Room, bool>> filter = R => (queryParamters.roomType == null || R.RoomType == roomTypeEnum)
                && (queryParamters.roomStatus == null || R.RoomStatus == roomStatusEnum);


                Expression<Func<Room, object>>? OrderByAsyncExp = null;
                Expression<Func<Room, object>>? OrderByDescExp = null;
                if (queryParamters.Sort is not null)
                {
                    switch (queryParamters.Sort)
                    {
                        case "PriceAsc":
                            OrderByAsyncExp = R => R.PricePerNight;
                            break;
                        case "PriceDesc":
                            OrderByDescExp = R => R.PricePerNight;
                            break;
                        default:
                            OrderByAsyncExp = R => R.Id;
                            break;
                    }
                }
                else
                {
                    OrderByDescExp = R => R.CreatedAt;
                }

                 rooms = await _unitOfWork.GetRepository<Room, int>()
                    .GetAllAsync(filter, OrderByAsyncExp, OrderByDescExp);

            }
            else
            {
                rooms = await _unitOfWork.GetRepository<Room, int>()
                    .GetAllAsync();
            }
                

            if (rooms is null || !rooms.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Rooms Found Available";
                return genericResponse;
            }

            var MappedRooms = _mapper.Map<IEnumerable<RoomForAdminDTO>>(rooms);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = " Rooms Retrieved Successfully . ";
            genericResponse.Data = MappedRooms;

            return genericResponse;
        }

        public async Task<GenericResponse<IEnumerable<RoomDTO>>> GetAllRoomForGuestAsync(string? roomType, string? sort)
        {
            var genericResponse = new GenericResponse<IEnumerable<RoomDTO>>();


            Enum.TryParse(roomType, out RoomType RoomTypeEnum);
            Expression<Func<Room,bool>>filter = R=>(roomType==null || R.RoomType== RoomTypeEnum) &&
            (R.RoomStatus==RoomStatus.Available|| R.RoomStatus == RoomStatus.Reserved );
            Expression<Func<Room, object>>? OrderByAsc = null;
            Expression<Func<Room, object>>? OrderByDescending = null;

            if( sort is not null)
            {
                switch (sort)
                {
                    case "PriceAsc":
                        OrderByAsc = R => R.PricePerNight;
                        break;
                    case "PriceDesc":
                        OrderByDescending = R => R.PricePerNight;
                        break;
                    default:
                        OrderByAsc = R => R.Id;
                        break;
                }
            }
            else
            {
                OrderByAsc = R => R.Id;
            }

            var rooms = await _unitOfWork.GetRepository<Room,int>()
                .GetAllAsync(filter,OrderByAsc,OrderByDescending);

            if(rooms is null || !rooms.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = " Room Is Not Found";
                return genericResponse;

            }

            var mappedRooms = _mapper.Map<IEnumerable<Room>,IEnumerable<RoomDTO>>(rooms);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Rooms Retrieved Successfully";
            genericResponse.Data = mappedRooms;

            return genericResponse;

        }

        public async Task<GenericResponse<RoomDetailsDTO>> GetRoomDetailsAsync(int roomId)
        {
            var genericResponse=new GenericResponse<RoomDetailsDTO>();

            Expression<Func<Room, bool>> filter = R => R.RoomStatus == RoomStatus.Available || R.RoomStatus == RoomStatus.Reserved;

            var room = await _unitOfWork.GetRepository<Room,int>()
                .GetByIdAsync(roomId, filter, [R=>R.RoomImages]);

            if(room is null)
            {
                genericResponse.StatusCode= StatusCodes.Status404NotFound;
                genericResponse.Message = "Room Not Found";

                return genericResponse;
            }

            var mappedRoom = _mapper.Map<RoomDetailsDTO>(room);

            genericResponse.StatusCode =StatusCodes.Status200OK;
            genericResponse.Message = "Room details Retrieved Successfully.";
            genericResponse.Data = mappedRoom;

            return genericResponse;

        }

        public async Task<GenericResponse<bool>> CreateRoomAsync(RoomToCreateDTO createRoom)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                if (createRoom is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid Room data";

                    return genericResponse;
                }

                var roomToBeCreated = _mapper.Map<RoomToCreateDTO, Room>(createRoom); // De Attached


                await _unitOfWork.GetRepository<Room, int>()
                    .AddAsync(roomToBeCreated); // Added

                var result = await _unitOfWork.SaveChangesAsync() > 0; 

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = " Room Created Successfully .";
                    genericResponse.Data = true;

                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = " Faild To  Create Room .";
                    genericResponse.Data = false;
                }
                return genericResponse;
            }
            catch (Exception ex)
            {
               _logger.LogError(ex , "An error Occurred while creating a room");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = " An unexpected error Occurred . ";
                genericResponse.Data = false;

                return genericResponse;
            }

        }

        public async Task<GenericResponse<bool>> UpdateRoomAsync(int roomId, RoomToUpdateDTO updateRoom)
        {
           var genericResponse = new GenericResponse<bool>();

            try
            {
                var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;

                    genericResponse.Message = "Room Not Found To Update";

                    return genericResponse;
                }

                _mapper.Map(updateRoom, room);

                room.UpdatedAt = DateTime.Now;

                _unitOfWork.GetRepository<Room, int>().Update(room);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Success To Update Room ";
                    genericResponse.Data = true;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = " Faild To Update Room";
                    genericResponse.Data = false;

                }
                return genericResponse;
            }
            catch (Exception ex )
            {

                _logger.LogError(ex, "An Unexpected Error Occurred Updating a Room");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = " Unexpected Error Occurred";
                genericResponse.Data = false;

                return genericResponse;
            }






        }

        public async Task<GenericResponse<bool>> DeleteRoomAsync(int roomId)
        {
            var genericResponse= new GenericResponse<bool>();

            try
            {

                //  Not Complete Logic Want To Create Booking Entity 
                // To Ensure This Room Will Be Delete Not Have Next Booking 
                var room = await _unitOfWork.GetRepository<Room, int>()
                    .GetByIdAsync(roomId);

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Room Not Found To Delete ";
                    genericResponse.Data = false;

                    return genericResponse;

                }
                room.RoomStatus = RoomStatus.NotExist;
                _unitOfWork.GetRepository<Room, int>().Update(room); // Marked As Modified
                room.UpdatedAt = DateTime.Now;


                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Success To Delete Room ";
                    genericResponse.Data = true;

                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Faild To Delete Room ";
                    genericResponse.Data = false;
                }

                return genericResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An unexpected error Occurred");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "UnExpected Error Occurred While Delete Room ";
                return genericResponse;
            }
            
        }

        public async Task<GenericResponse<bool>> UploadImagesAsync(int roomId, List<IFormFile> files)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No Room Found To Upload Images For it ";
                    genericResponse.Data = false;

                    return genericResponse;

                }

                foreach (var file in files)
                {

                    var fileName = await _attachmentService.UplodaFileAsync(file, "rooms");

                    if (fileName is null)
                        continue;

                    var roomImage = new RoomImage
                    {
                        RoomId = room.Id,
                        PictureUrl = fileName,
                    };

                    await _unitOfWork.GetRepository<RoomImage, int>().AddAsync(roomImage);

                }
                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Success To Upload Room Image";
                    genericResponse.Data = true;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Faild To Upload Room Images";
                    genericResponse.Data = false;
                }
                return genericResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An Unexpected Error Occurred While Uploading Images");
                genericResponse.StatusCode= StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Faild To Upload Room Images";
                return genericResponse;
            }

        }

        public async Task<GenericResponse<bool>> DeleteRoomImageAsync(int roomId, int imageId)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var room = await _unitOfWork.GetRepository<Room, int>()
                        .GetByIdAsync(roomId, null, [RI => RI.RoomImages]);

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Not Found Room to Delete Its Images";
                    return genericResponse;
                }

                if (room.RoomImages is null || room.RoomImages.Count == 0)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;

                    genericResponse.Message = "No Images Found To This Room";
                    return genericResponse;
                }

                var roomImage = room.RoomImages.FirstOrDefault(RI => RI.Id == imageId);

                if (roomImage is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "No Image With This id Found To Delete";
                    return genericResponse;
                }

                _unitOfWork.GetRepository<RoomImage, int>().Delete(roomImage);

                var IsDeletedFromServer =  _attachmentService.DeleteFile(roomImage.PictureUrl, "rooms");

                if (!IsDeletedFromServer)
                {
                    genericResponse.StatusCode= StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Faild  to Delete This Image From Server";
                    return genericResponse;
                }

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Success To Delete This room Image";
                    genericResponse.Data = true;
                }
                else
                { 
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Faild To Delete This Image For This Room";
                    genericResponse.Data = false;
                }

                return genericResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An Unexpected Error Occurred While Deleting Room Image");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Faild To Delete This Image For This Room";
                return genericResponse;

            }


        }
    }
}
