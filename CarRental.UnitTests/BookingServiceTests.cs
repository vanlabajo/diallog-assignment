using CarRental.Repository;
using CarRental.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CarRental.UnitTests
{
    public class BookingServiceTests
    {
        [Fact]
        public async Task ShouldAddBookingUnderNormalCircumstances()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

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

            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            Assert.Empty(result.ValidationErrors);
        }

        [Fact]
        public async Task ShouldCalculateTheCorrectTotalCost()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

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

            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            Assert.Empty(result.ValidationErrors);

            var bookingId = result.Id.Value;

            booking = await bookingService.GetBooking(bookingId);

            Assert.Equal(300, booking.TotalCost);

            booking.StartDateUtc = booking.StartDateUtc.AddDays(1);

            result = await bookingService.UpdateBooking("tester", booking);

            booking = await bookingService.GetBooking(bookingId);

            Assert.Equal(200, booking.TotalCost);
        }

        [Fact]
        public async Task ShouldConfirmBookingIfPayedInFull()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

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

            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            Assert.Empty(result.ValidationErrors);

            booking = await bookingService.GetBooking(result.Id.Value);

            Assert.False(string.IsNullOrEmpty(booking.ReferenceNumber));

            booking.AddPayment(100);
            booking.AddPayment(200);

            result = await bookingService.UpdateBooking("tester", booking);

            Assert.True(result.Success);
            Assert.Empty(result.ValidationErrors);

            booking = await bookingService.GetBooking(booking.ReferenceNumber);

            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact]
        public async Task ShouldCancelBooking()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

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

            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            Assert.Empty(result.ValidationErrors);

            var bookingId = result.Id.Value;

            result = await bookingService.CancelBooking("tester", bookingId);

            booking = await bookingService.GetBooking(bookingId);

            Assert.Equal(BookingStatus.Cancelled, booking.Status);
        }

        [Fact]
        public async Task ShouldNotBeAllowedToUpdateConfirmedBooking()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

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

            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            Assert.Empty(result.ValidationErrors);

            booking.Id = result.Id.Value;
            booking.AddPayment(300);

            result = await bookingService.UpdateBooking("tester", booking);

            Assert.True(result.Success);
            Assert.Empty(result.ValidationErrors);

            booking.StartDateUtc = DateTime.UtcNow.AddDays(1);

            result = await bookingService.UpdateBooking("tester", booking);

            Assert.False(result.Success);
            Assert.NotEmpty(result.ValidationErrors);
            Assert.Equal("Updating of confirmed or cancelled booking is not allowed", result.ValidationErrors["Booking"]);
        }

        [Fact]
        public async Task ShouldReturnBookingsByRenter()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

            var bookingService = new BookingService(dbContext);

            var car = new Car { DailyRentalCost = 100, GasConsumption = "XXXXX", NumberOfUnits = 10 };

            for (int i = 0; i < 5; i++)
            {
                var booking = new Booking
                {
                    GuestUserId = "XXXXXX",
                    GuestName = "XXXXXX",
                    Car = car,
                    StartDateUtc = DateTime.UtcNow,
                    EndDateUtc = DateTime.UtcNow.AddDays(3)
                };
                await bookingService.AddBooking(booking);
            }

            for (int i = 0; i < 5; i++)
            {
                var booking = new Booking
                {
                    GuestUserId = "YYYYYY",
                    GuestName = "YYYYYY",
                    Car = car,
                    StartDateUtc = DateTime.UtcNow,
                    EndDateUtc = DateTime.UtcNow.AddDays(3)
                };
                await bookingService.AddBooking(booking);
            }

            var pendingBookings = await bookingService.GetPendingBookings();

            Assert.Equal(10, pendingBookings.Count);

            var bookingsByRenter = await bookingService.GetBookings("XXXXXX");

            Assert.Equal(5, bookingsByRenter.Count);
            Assert.Equal("XXXXXX", bookingsByRenter[3].GuestUserId);
        }

        [Fact]
        public async Task ShouldNotAllowBookingIfCarIsNotAvailable()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

            var carService = new CarService(dbContext);
            var bookingService = new BookingService(dbContext);

            var car = new Car { DailyRentalCost = 100, GasConsumption = "XXXXX", NumberOfUnits = 1 };

            var carResult = await carService.AddCarDetail("tester", car);

            car = await carService.GetCar(carResult.Id.Value);

            var booking1 = new Booking
            {
                GuestUserId = "XXXXXX",
                GuestName = "XXXXXX",
                Car = car,
                StartDateUtc = DateTime.UtcNow,
                EndDateUtc = DateTime.UtcNow.AddDays(3)
            };
            await bookingService.AddBooking(booking1);

            var booking2 = new Booking
            {
                GuestUserId = "XXXXXX",
                GuestName = "XXXXXX",
                Car = car,
                StartDateUtc = DateTime.UtcNow,
                EndDateUtc = DateTime.UtcNow.AddDays(3)
            };

            var result = await bookingService.AddBooking(booking2);

            Assert.False(result.Success);
            Assert.NotNull(result.ValidationErrors);
            Assert.Equal("All available units are rented out.", result.ValidationErrors["Car"]);
        }
    }
}
