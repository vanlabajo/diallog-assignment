using CarRental.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public class BookingService : IBookingService
    {
        private readonly DiallogDbContext dbContext;

        public BookingService(DiallogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ServiceResult> AddBooking(Booking booking)
        {
            var result = ValidateBookingModel(booking);
            if (result.ValidationErrors.Count > 0) return result;

            var dbCar = await dbContext.Cars.FindAsync(booking.Car.Id);
            if (dbCar != null && !dbCar.RentOut())
            {
                result.ValidationErrors["Car"] = "All available units are rented out.";
                return result;
            }

            var random = new Random(Guid.NewGuid().GetHashCode());
            var referenceNumber = random.Next(0, 1000000);
            var totalDays = Convert.ToDecimal((booking.EndDateUtc.Date - booking.StartDateUtc.Date).TotalDays);
            var newBooking = new Booking
            {
                GuestUserId = booking.GuestUserId,
                GuestName = booking.GuestName,
                Car = dbCar ?? booking.Car,
                StartDateUtc = booking.StartDateUtc,
                EndDateUtc = booking.EndDateUtc,
                Status = BookingStatus.Pending,
                TotalCost = totalDays * (dbCar?.DailyRentalCost ?? booking.Car.DailyRentalCost),
                LastUpdatedBy = booking.GuestUserId,
                LastUpdateTimeUtc = DateTime.UtcNow,
                CreatedTimeUtc = DateTime.UtcNow,
                ReferenceNumber = referenceNumber.ToString("D6")
            };
            dbContext.Bookings.Add(newBooking);

            result.Success = await dbContext.SaveChangesAsync() > 0;

            if (result.Success) result.Id = newBooking.Id;
            return result;
        }

        public async Task<ServiceResult> CancelBooking(string userId, int bookingId)
        {
            var result = new ServiceResult();

            var booking = await dbContext.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null)
            {
                result.ValidationErrors.Add("Booking", "Booking ID does not exist");
                return result;
            }

            booking.Status = BookingStatus.Cancelled;
            booking.LastUpdatedBy = userId;
            booking.LastUpdateTimeUtc = DateTime.UtcNow;
            booking.CancelledTimeUtc = DateTime.UtcNow;
            booking.Car.Return();

            result.Success = await dbContext.SaveChangesAsync() > 0;
            return result;
        }

        public async Task<Booking> GetBooking(int bookingId)
        {
            var booking = await dbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.Payments)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            return booking;
        }

        public async Task<Booking> GetBooking(string referenceNumber)
        {
            var booking = await dbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.Payments)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ReferenceNumber == referenceNumber);
            return booking;
        }

        public async Task<List<Booking>> GetBookings(string guestUserId)
        {
            var data = dbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.Payments)
                .AsNoTracking()
                .Where(b => b.GuestUserId == guestUserId);
            return await data.ToListAsync();
        }

        public async Task<List<Booking>> GetPendingBookings()
        {
            var data = dbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.Payments)
                .AsNoTracking()
                .Where(b => b.Status == BookingStatus.Pending);
            return await data.ToListAsync();
        }

        public async Task<ServiceResult> UpdateBooking(string updaterUserId, Booking booking)
        {
            var result = ValidateBookingModel(booking);
            if (result.ValidationErrors.Count > 0) return result;

            var model = await dbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == booking.Id);
            if (model == null)
            {
                result.ValidationErrors.Add("Booking", "Booking ID does not exist");
                return result;
            }

            if (model.Status != BookingStatus.Pending)
            {
                result.ValidationErrors.Add("Booking", "Updating of confirmed or cancelled booking is not allowed");
                return result;
            }

            var rentalCost = model.Car.DailyRentalCost;

            if (model.Car.Id != booking.Car.Id) // Changed cars
            {
                model.Car.Return();

                var newCar = await dbContext.Cars.FindAsync(booking.Car.Id);
                if (newCar != null && !newCar.RentOut())
                {
                    result.ValidationErrors["Car"] = "All available units are rented out.";
                    return result;
                }

                model.Car = newCar;

                rentalCost = newCar.DailyRentalCost;
            }

            var totalDays = Convert.ToDecimal((booking.EndDateUtc.Date - booking.StartDateUtc.Date).TotalDays);
            var totalCost = totalDays * rentalCost;

            model.LastUpdatedBy = updaterUserId;
            model.LastUpdateTimeUtc = DateTime.UtcNow;
            model.StartDateUtc = booking.StartDateUtc;
            model.EndDateUtc = booking.EndDateUtc;
            model.TotalCost = totalCost;

            foreach (var payment in booking.Payments.Where(p => p.PaymentId <= 0).ToList())
            {
                model.AddPayment(payment.Amount);
            }

            if (model.Payments.Sum(p => p.Amount) >= model.TotalCost)
                model.Status = BookingStatus.Confirmed;

            result.Success = await dbContext.SaveChangesAsync() > 0;
            return result;
        }

        private ServiceResult ValidateBookingModel(Booking booking)
        {
            var result = new ServiceResult();

            if (string.IsNullOrEmpty(booking.GuestUserId))
                result.ValidationErrors.Add("GuestUserId", "Guest UserID is required.");

            if (string.IsNullOrEmpty(booking.GuestName))
                result.ValidationErrors.Add("GuestName", "Guest name is required.");

            if (booking.Car == null)
                result.ValidationErrors.Add("Car", "Car selection is required.");

            if (booking.StartDateUtc.Equals(DateTime.MinValue))
                result.ValidationErrors.Add("StartDateUtc", "Car pick up date is invalid.");

            if (booking.EndDateUtc.Equals(DateTime.MinValue))
                result.ValidationErrors.Add("EndDateUtc", "Car pick up date is invalid.");

            return result;
        }
    }
}
