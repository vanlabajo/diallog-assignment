using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public interface ICarService
    {
        Task<ServiceResult> AddCarDetail(string creatorUserId, Car car);
        Task<ServiceResult> UpdateCarDetail(string updaterUserId, Car car);
        Task<Car> GetCar(int carId);
        Task<List<Car>> GetCars();
    }
}
