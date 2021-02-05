using System;

namespace CarRental
{
    public class Car
    {
        public int Id { get; set; }
        public CarType Type { get; set; }
        public int Size { get; set; }
        public string GasConsumption { get; set; }
        public decimal DailyRentalCost { get; set; }
        public int NumberOfUnits { get; set; }
        public DateTime LastUpdateTimeUtc { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime CreatedTimeUtc { get; set; }
        public string CreatedBy { get; set; }

        private int _rentedOut;
        public int RentedOut => _rentedOut;

        public bool RentOut()
        {
            if (NumberOfUnits > RentedOut)
            {
                _rentedOut += 1;
                return true;
            }

            return false;
        }

        public void Return()
        {
            _rentedOut -= 1;
        }
    }

    public enum CarType
    {
        Sedan,
        SUV,
        Pickup
    }
}
