using CarRental;
using CarRental.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingCancellationsController : ControllerBase
    {
        private readonly ILogger<BookingCancellationsController> logger;
        private readonly IBookingService bookingService;

        public BookingCancellationsController(ILogger<BookingCancellationsController> logger,
            IBookingService bookingService)
        {
            this.logger = logger;
            this.bookingService = bookingService;
        }

        [HttpPost]
        public async Task<ServiceResult> Post([FromBody] Booking booking)
        {
            var cancellationResult = new ServiceResult();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var savedBooking = await bookingService.GetBooking(booking.Id);

            if (savedBooking != null) cancellationResult = await bookingService.CancelBooking(userId, booking.Id);

            return cancellationResult;
        }
    }
}
