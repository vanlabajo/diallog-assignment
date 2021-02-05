using CarRental;
using CarRental.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PendingBookingsAdminController : Controller
    {
        private readonly ILogger<PendingBookingsAdminController> logger;
        private readonly IBookingService bookingService;

        public PendingBookingsAdminController(ILogger<PendingBookingsAdminController> logger,
            IBookingService bookingService)
        {
            this.logger = logger;
            this.bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IEnumerable<Booking>> Get()
        {
            return await bookingService.GetPendingBookings();
        }

        [HttpGet("{referenceNumber}")]
        public async Task<IEnumerable<Booking>> Get(string referenceNumber)
        {
            var pendings = await bookingService.GetPendingBookings();
            var pendingsByReference = pendings.Where(p => p.ReferenceNumber == referenceNumber).ToList();
            return pendingsByReference;
        }
    }
}
