using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public interface IBookingService
    {
        Task<ServiceResult> AddBooking(Booking booking);
        Task<ServiceResult> UpdateBooking(string updaterUserId, Booking booking);
        Task<ServiceResult> CancelBooking(string userId, int bookingId);
        Task<Booking> GetBooking(int bookingId);
        Task<Booking> GetBooking(string referenceNumber);
        Task<List<Booking>> GetBookings(string guestUserId);
        Task<List<Booking>> GetPendingBookings();
    }
}
