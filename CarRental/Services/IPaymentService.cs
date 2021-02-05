using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public interface IPaymentService
    {
        Task<(bool Success, string GatewayReferenceId)> MakePayment(decimal amount);
        Task<List<Payment>> GetPayments();
        Task<List<Payment>> GetPayments(string guestUserId);
    }
}
