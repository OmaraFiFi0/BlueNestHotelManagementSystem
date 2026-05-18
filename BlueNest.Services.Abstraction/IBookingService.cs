using BlueNest.Shared.DTOs.BookingDTOs;
using BlueNest.Shared.Reponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Abstraction
{
    public interface IBookingService
    {
        Task<GenericResponse<Guid>> CreateBooking(string userId, CreateBookingDTO createBooking);

        Task<GenericResponse<IEnumerable<BookingDTO>>> GetAllBookingForAdmin();

        Task<GenericResponse<bool>> CancleBookingAsync(Guid bookingId);

        Task<GenericResponse<IEnumerable<MyBookingDTO>>> GetAllBookingsForGuest(string userId);
    }
}
