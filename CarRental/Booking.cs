using System;
using System.Collections.Generic;

namespace CarRental
{
    public class Booking
    {
        public int Id { get; set; }
        public string GuestUserId { get; set; }
        public string GuestName { get; set; }
        public Car Car { get; set; }
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
        public BookingStatus Status { get; set; }
        public decimal TotalCost { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime LastUpdateTimeUtc { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime CreatedTimeUtc { get; set; }
        public DateTime? CancelledTimeUtc { get; set; }
        public DateTime? ConfirmedTimeUtc { get; set; }


        private List<Payment> _payments;
        public IReadOnlyCollection<Payment> Payments => _payments;

        public Booking()
        {
            _payments = new List<Payment>();
        }

        public void AddPayment(decimal amount)
        {
            _payments.Add(new Payment
            {
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow
            });
        }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}
