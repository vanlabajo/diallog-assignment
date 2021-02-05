using CarRental;
using CarRental.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ILogger<BookingsController> logger;
        private readonly IBookingService bookingService;
        private readonly IPaymentService paymentService;

        public BookingsController(ILogger<BookingsController> logger,
            IBookingService bookingService,
            IPaymentService paymentService)
        {
            this.logger = logger;
            this.bookingService = bookingService;
            this.paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IEnumerable<Booking>> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await bookingService.GetBookings(userId);
            return bookings.OrderByDescending(p => p.StartDateUtc);
        }

        [HttpGet("{referenceNumber}")]
        public async Task<IEnumerable<Booking>> Get(string referenceNumber)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await bookingService.GetBookings(userId);
            var carsByReference = bookings.Where(c => c.ReferenceNumber == referenceNumber).OrderByDescending(p => p.StartDateUtc).ToList();
            return carsByReference;
        }

        [HttpPost]
        public async Task<ServiceResult> Post([FromBody] Booking booking)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            booking.GuestUserId = userId;

            var bookingResult = await bookingService.AddBooking(booking);
            if (bookingResult.Success)
            {
                var savedBooking = await bookingService.GetBooking(bookingResult.Id.Value);
                var (Success, GatewayReferenceId) = await paymentService.MakePayment(savedBooking.TotalCost);
                if (Success)
                {
                    savedBooking.AddPayment(savedBooking.TotalCost);
                    await bookingService.UpdateBooking(userId, savedBooking);
                }
            }

            return bookingResult;
        }

        [HttpPut("{id}")]
        public async Task<ServiceResult> Put(int id, [FromBody] Booking booking)
        {
            var exists = await bookingService.GetBooking(id);
            if (exists != null)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var bookingResult = await bookingService.UpdateBooking(userId, booking);
                if (bookingResult.Success)
                {
                    var savedBooking = await bookingService.GetBooking(id);
                    var (Success, GatewayReferenceId) = await paymentService.MakePayment(savedBooking.TotalCost);
                    if (Success)
                    {
                        savedBooking.AddPayment(savedBooking.TotalCost);
                        await bookingService.UpdateBooking(userId, savedBooking);
                    }
                }

                return bookingResult;
            }

            return new ServiceResult();
        }
    }
}
