using BlueNest.Infrastructure.Repository;
using BlueNest.Services.Abstraction;
using BlueNest.Shared.DTOs.BookingDTOs;
using BlueNest.Shared.Reponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueNest.API.Controllers
{

    public class BookingsController : BaseApiController
    {
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymentService;

        public BookingsController(IBookingService bookingService, IPaymentService paymentService)
        {
            _bookingService = bookingService;
            _paymentService = paymentService;
        }

        // POST : BaseUrl/api/Bookings
        [Authorize(Roles = "Guest")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<Guid>>> CreateBooking([FromBody] CreateBookingDTO createBooking)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.CreateBooking(userid!, createBooking);
            return HandleResult(result);
        }

        // POST : BaseUrl/api/Bookings/{bookingId}/pay
        [HttpPost("{id}/pay")]
        public async Task<ActionResult<GenericResponse<string>>> CreatePaymentUrl(Guid id)
        {
            var result = await _paymentService.CreatePaymentUrlAsync(id);
            return HandleResult(result);
        }

        // GET : BaseUrl/api/Bookings/admin
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<ActionResult<GenericResponse<IEnumerable<BookingDTO>>>> GetAllBookingsForAdmin()
        {
            var result = await _bookingService.GetAllBookingForAdmin();
            return HandleResult(result);
        }

        // POST : BaseUrl/api/Bookings/{bookingId}/cancel
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/cancel")]
        public async Task<ActionResult<GenericResponse<bool>>> CancelBooking([FromRoute] Guid id)
        {
            var result = await _bookingService.CancleBookingAsync(id);
            return HandleResult(result);
        }

        // GET : BaseUrl/api/Bookings/my
        [Authorize(Roles = "Guest")]
        [HttpGet("my")]
        public async Task<ActionResult<GenericResponse<IEnumerable<MyBookingDTO>>>> GetAllBookingsForGuest()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.GetAllBookingsForGuest(userid!);
            return HandleResult(result);
        }
    }
}
