using CarRental.Repository;
using CarRental.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CarRental.UnitTests
{
    public class GetPaymentsTests
    {
        [Fact]
        public async Task ShouldReturnPayments()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

            var paymentService = new StripePaymentService(dbContext);
            var bookingService = new BookingService(dbContext);

            var booking = new Booking
            {
                GuestUserId = "XXXXXX",
                GuestName = "XXXXXX",
                Car = new Car { DailyRentalCost = 100, GasConsumption = "XXXXX", NumberOfUnits = 3 },
                StartDateUtc = DateTime.UtcNow,
                EndDateUtc = DateTime.UtcNow.AddDays(3)
            };

            var result = await bookingService.AddBooking(booking);

            booking.Id = result.Id.Value;
            booking.AddPayment(100);
            booking.AddPayment(200);

            await bookingService.UpdateBooking("tester", booking);

            var payments = await paymentService.GetPayments();

            Assert.NotEmpty(payments);
            Assert.Equal(2, payments.Count);
        }

        [Fact]
        public async Task ShouldReturnPaymentsByRenterId()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

            var paymentService = new StripePaymentService(dbContext);
            var bookingService = new BookingService(dbContext);

            var booking = new Booking
            {
                GuestUserId = "XXXXXX",
                GuestName = "XXXXXX",
                Car = new Car { DailyRentalCost = 100, GasConsumption = "XXXXX", NumberOfUnits = 3 },
                StartDateUtc = DateTime.UtcNow,
                EndDateUtc = DateTime.UtcNow.AddDays(3)
            };

            var result = await bookingService.AddBooking(booking);

            booking.Id = result.Id.Value;
            booking.AddPayment(100);
            booking.AddPayment(200);

            await bookingService.UpdateBooking("tester", booking);

            booking = new Booking
            {
                GuestUserId = "YYYYYY",
                GuestName = "YYYYYY",
                Car = new Car { DailyRentalCost = 100, GasConsumption = "XXXXX", NumberOfUnits = 3 },
                StartDateUtc = DateTime.UtcNow,
                EndDateUtc = DateTime.UtcNow.AddDays(3)
            };

            result = await bookingService.AddBooking(booking);

            booking.Id = result.Id.Value;
            booking.AddPayment(50);
            booking.AddPayment(50);
            booking.AddPayment(50);
            booking.AddPayment(50);
            booking.AddPayment(100);

            await bookingService.UpdateBooking("tester", booking);

            var payments = await paymentService.GetPayments("YYYYYY");

            Assert.NotEmpty(payments);
            Assert.Equal(5, payments.Count);
        }
    }
}
