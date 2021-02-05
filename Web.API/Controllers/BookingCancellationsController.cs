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
    public class BookingPaymentsController : ControllerBase
    {
        private readonly ILogger<BookingPaymentsController> logger;
        private readonly IBookingService bookingService;
        private readonly IPaymentService paymentService;

        public BookingPaymentsController(ILogger<BookingPaymentsController> logger,
            IBookingService bookingService,
            IPaymentService paymentService)
        {
            this.logger = logger;
            this.bookingService = bookingService;
            this.paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ServiceResult> Post([FromBody] Booking booking)
        {
            var paymentResult = new ServiceResult();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var savedBooking = await bookingService.GetBooking(booking.Id);
            var (Success, GatewayReferenceId) = await paymentService.MakePayment(savedBooking.TotalCost);
            if (Success)
            {
                savedBooking.AddPayment(savedBooking.TotalCost);
                paymentResult = await bookingService.UpdateBooking(userId, savedBooking);
            }

            return paymentResult;
        }
    }
}
