using CarRental.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly DiallogDbContext dbContext;

        public StripePaymentService(DiallogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Payment>> GetPayments()
        {
            return await dbContext.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Car)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPayments(string guestUserId)
        {
            var data = dbContext.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Car)
                .AsNoTracking()
                .Where(p => p.Booking.GuestUserId == guestUserId);
            return await data.ToListAsync();
        }

        public Task<(bool Success, string GatewayReferenceId)> MakePayment(decimal amount)
        {
            // Plan is to make a payment request to Stripe's gateway
            // however, I don't have enough time. So the implementation here
            // is mocking that this will either be successful or not

            var result = false;

            if (amount > 0)
            {
                var random = new Random();
                result = random.NextDouble() >= 0.5;
            }

            return Task.FromResult((result, "StripeResultId"));
        }
    }
}
