using CarRental.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public class CarService : ICarService
    {
        private readonly DiallogDbContext dbContext;

        public CarService(DiallogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ServiceResult> AddCarDetail(string creatorUserId, Car car)
        {
            var result = ValidateCarModel(car);
            if (result.ValidationErrors.Count > 0) return result;

            var newCar = new Car
            {
                Type = car.Type,
                Size = car.Size,
                GasConsumption = car.GasConsumption,
                DailyRentalCost = car.DailyRentalCost,
                NumberOfUnits = car.NumberOfUnits,
                LastUpdatedBy = creatorUserId,
                LastUpdateTimeUtc = DateTime.UtcNow,
                CreatedTimeUtc = DateTime.UtcNow,
                CreatedBy = creatorUserId
            };
            dbContext.Cars.Add(newCar);

            result.Success = await dbContext.SaveChangesAsync() > 0;

            if (result.Success) result.Id = newCar.Id;
            return result;
        }

        public async Task<Car> GetCar(int carId)
        {
            var car = await dbContext.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == carId);
            return car;
        }

        public async Task<List<Car>> GetCars()
        {
            return await dbContext.Cars
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ServiceResult> UpdateCarDetail(string updaterUserId, Car car)
        {
            var result = ValidateCarModel(car);
            if (result.ValidationErrors.Count > 0) return result;

            var model = await dbContext.Cars.FirstOrDefaultAsync(c => c.Id == car.Id);
            if (model == null)
            {
                result.ValidationErrors.Add("Car", "Car ID does not exist");
                return result;
            }

            model.LastUpdatedBy = updaterUserId;
            model.LastUpdateTimeUtc = DateTime.UtcNow;
            model.Type = car.Type;
            model.Size = car.Size;
            model.GasConsumption = car.GasConsumption;
            model.DailyRentalCost = car.DailyRentalCost;

            result.Success = await dbContext.SaveChangesAsync() > 0;
            return result;
        }

        private ServiceResult ValidateCarModel(Car car)
        {
            var result = new ServiceResult();

            if (string.IsNullOrEmpty(car.GasConsumption))
                result.ValidationErrors.Add("GasConsumption", "Gas consumption is required.");

            if (car.Size < 0)
                result.ValidationErrors.Add("Size", "Size must be greater than or equal to 0.");

            if (car.NumberOfUnits < 0)
                result.ValidationErrors.Add("NumberOfUnits", "Size must be greater than or equal to 0.");

            return result;
        }
    }
}
