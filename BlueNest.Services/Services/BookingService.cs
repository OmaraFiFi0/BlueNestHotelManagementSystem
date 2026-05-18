using AutoMapper;
using BlueNest.Core.Contracts;
using BlueNest.Core.Entities.BookingModule;
using BlueNest.Core.Entities.RoomModule;
using BlueNest.Services.Abstraction;
using BlueNest.Shared.DTOs.BookingDTOs;
using BlueNest.Shared.Message;
using BlueNest.Shared.Reponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookingService> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public BookingService(IUnitOfWork unitOfWork, ILogger<BookingService> logger, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<GenericResponse<Guid>> CreateBooking(string userId, CreateBookingDTO createBooking)
        {
            var genericResponse = new GenericResponse<Guid>();

            try
            {
                if (createBooking is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No Booking Data Provided";

                    return genericResponse;
                }

                if (createBooking.CheckInDate < DateTime.Now || createBooking.CheckInDate >= createBooking.CheckOutDate)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid Booking Data";

                    return genericResponse;
                }

                var room = await _unitOfWork.GetRepository<Room, int>()
                    .GetByIdAsync(createBooking.RoomId, null, [R => R.RoomBookings]);

                if (room is null || room.RoomStatus == RoomStatus.NotExist ||
                    room.RoomStatus == RoomStatus.Maintenance)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "This Room Is not Available To Reserve";

                    return genericResponse;
                }

                // 10 15 
                // 11 16

                var hasConflict = room.RoomBookings.Any
                    (B => (B.Status == BookingStatus.PaymentPending || B.Status == BookingStatus.PaymentPaid)
                    && (B.CheckInDate < createBooking.CheckOutDate)
                    && (B.CheckOutDate > createBooking.CheckInDate));

                if (hasConflict)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Room is Not Available to Reserve in This Dates";

                    return genericResponse;
                }

                var nights = (createBooking.CheckOutDate - createBooking.CheckInDate).Days;
                var totalAmount = room.PricePerNight * nights;
                var booking = new Booking()
                {
                    Id = Guid.NewGuid(),
                    CheckInDate = createBooking.CheckInDate,
                    CheckOutDate = createBooking.CheckOutDate,
                    HotelUserId = userId,
                    RoomId = createBooking.RoomId,
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.Now,

                };

                await _unitOfWork.GetRepository<Booking, Guid>().AddAsync(booking);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Success To Create Booking";
                    genericResponse.Data = booking.Id;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Faild To Create Booking";

                }
                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected Error Occuree While Booking Room");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Faild To Create Booking";

                return genericResponse;
            }


        }

        public async Task<GenericResponse<IEnumerable<BookingDTO>>> GetAllBookingForAdmin()
        {
            var genericResponse = new GenericResponse<IEnumerable<BookingDTO>>();

            var booking = await _unitOfWork.GetRepository<Booking, Guid>()
                .GetAllAsync(null, null, C => C.CreatedAt, [B => B.HotelUser]);

            if (booking is null || booking.Count() == 0)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Bookings To Show";

                return genericResponse;
            }

            var mappedBookings = _mapper.Map<IEnumerable<Booking>, IEnumerable<BookingDTO>>(booking);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success To Retrive All Bookings ";
            genericResponse.Data = mappedBookings;

            return genericResponse;

        }
        public async Task<GenericResponse<bool>> CancleBookingAsync(Guid bookingId)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var booking = await _unitOfWork.GetRepository<Booking, Guid>().GetByIdAsync(bookingId, null, [B => B.HotelUser]);

                if (booking is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "No Booking To Cancle";

                    return genericResponse;
                }

                if (booking.Status == BookingStatus.PaymentPaid)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "You Can't Cancle Paid Booking";
                    return genericResponse;
                }

                booking.Status = BookingStatus.PaymentCancelled;
                booking.UpdatedAt = DateTime.Now;

                _unitOfWork.GetRepository<Booking, Guid>().Update(booking);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Success To Cancle Booking";
                    genericResponse.Data = true;

                    var email = new Email
                    {
                        To = booking.HotelUser.Email!,
                        Subject = "Your Booking Has Been Cancellation",
                        Body = $"Dear {booking.HotelUser.FullName},\n\nWe regret to inform you that your booking with ID {booking.Id} has been cancelled. If you have any questions or need further assistance, please contact our support team.\n\nBest regards,\nBlueNest Team"
                    };
                    await _emailService.SendEmail(email);
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Faild To Cancle Booking";
                }
                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed To Cancle This Booking");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Faild To Cancle Booking";
                return genericResponse;
            }

        }

        public async Task<GenericResponse<IEnumerable<MyBookingDTO>>> GetAllBookingsForGuest(string userId)
        {
            var genericResponse = new GenericResponse<IEnumerable<MyBookingDTO>>();
            var bookings = await _unitOfWork.GetRepository<Booking, Guid>()
                .GetAllAsync(X => X.HotelUserId == userId, null, C => C.CreatedAt);

            if (bookings is null || !bookings.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Bookings To Show";

                return genericResponse;
            }

            var mappedBookings = _mapper.Map<IEnumerable<MyBookingDTO>>(bookings);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success To Retrive All Guest Bookings ";

            genericResponse.Data = mappedBookings;

            return genericResponse;
        }
    }
}
