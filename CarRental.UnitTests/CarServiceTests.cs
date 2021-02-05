using CarRental.Repository;
using CarRental.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace CarRental.UnitTests
{
    public class CarServiceTests
    {
        [Fact]
        public async Task ShouldAddCarDetails()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

            var carService = new CarService(dbContext);

            var car = new Car
            {
                Type = CarType.Sedan,
                Size = 4,
                GasConsumption = "1000 miles / liter",
                DailyRentalCost = 100,
                NumberOfUnits = 3
            };

            var result = await carService.AddCarDetail("tester", car);

            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            Assert.Empty(result.ValidationErrors);
        }

        [Fact]
        public async Task ShouldUpdateCarDetails()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DiallogDbContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new DiallogDbContext(options);
            dbContext.Database.EnsureCreated();

            dbContext.Cars.Add(new Car
            {
                Id = 1,
                Type = CarType.Sedan,
                Size = 4,
                GasConsumption = "1000 miles / liter",
                DailyRentalCost = 100,
                NumberOfUnits = 3
            });
            dbContext.SaveChanges();

            var carService = new CarService(dbContext);

            var car = await carService.GetCar(1);

            car.Type = CarType.SUV;

            var result = await carService.UpdateCarDetail("tester", car);

            Assert.True(result.Success);
            Assert.Empty(result.ValidationErrors);

            car = await carService.GetCar(1);

            Assert.Equal(CarType.SUV, car.Type);
        }
    }
}
