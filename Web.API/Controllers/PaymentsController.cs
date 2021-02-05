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
    public class PaymentsController : Controller
    {
        private readonly ILogger<PaymentsController> logger;
        private readonly IPaymentService paymentService;

        public PaymentsController(ILogger<PaymentsController> logger,
            IPaymentService paymentService)
        {
            this.logger = logger;
            this.paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IEnumerable<Payment>> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var payments = await paymentService.GetPayments(userId);
            return payments.OrderByDescending(p => p.TransactionTimeUtc).ThenBy(p => p.Booking.ReferenceNumber);
        }

        [HttpGet("{referenceNumber}")]
        public async Task<IEnumerable<Payment>> Get(string referenceNumber)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var payments = await paymentService.GetPayments(userId);
            var paymentsByBookingReference = payments.Where(p => p.Booking.ReferenceNumber == referenceNumber).ToList();
            return paymentsByBookingReference.OrderByDescending(p => p.TransactionTimeUtc);
        }
    }
}
