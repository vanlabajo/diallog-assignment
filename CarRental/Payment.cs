using System;

namespace CarRental
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public Booking Booking { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionTimeUtc { get; set; }
    }
}
