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
    public class PaymentsAdminController : Controller
    {
        private readonly ILogger<PaymentsAdminController> logger;
        private readonly IPaymentService paymentService;

        public PaymentsAdminController(ILogger<PaymentsAdminController> logger,
            IPaymentService paymentService)
        {
            this.logger = logger;
            this.paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IEnumerable<Payment>> Get()
        {
            var payments = await paymentService.GetPayments();
            return payments.OrderByDescending(p => p.TransactionTimeUtc).ThenBy(p => p.Booking.ReferenceNumber);
        }

        [HttpGet("{referenceNumber}")]
        public async Task<IEnumerable<Payment>> Get(string referenceNumber)
        {
            var payments = await paymentService.GetPayments();
            var paymentsByBookingReference = payments.Where(p => p.Booking.ReferenceNumber == referenceNumber).ToList();
            return paymentsByBookingReference.OrderByDescending(p => p.TransactionTimeUtc);
        }
    }
}
