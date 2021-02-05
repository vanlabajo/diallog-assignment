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
    public class PendingBookingsController : Controller
    {
        private readonly ILogger<PendingBookingsController> logger;
        private readonly IBookingService bookingService;

        public PendingBookingsController(ILogger<PendingBookingsController> logger,
            IBookingService bookingService)
        {
            this.logger = logger;
            this.bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IEnumerable<Booking>> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await bookingService.GetBookings(userId);
            var pendings = bookings.Where(b => b.Status == BookingStatus.Pending).OrderByDescending(p => p.StartDateUtc).ToList();
            return pendings;
        }

        [HttpGet("{referenceNumber}")]
        public async Task<IEnumerable<Booking>> Get(string referenceNumber)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await bookingService.GetBookings(userId);
            var pendings = bookings.Where(b => b.Status == BookingStatus.Pending
                    && b.ReferenceNumber == referenceNumber)
                .OrderByDescending(p => p.StartDateUtc).ToList();
            return pendings;
        }
    }
}
