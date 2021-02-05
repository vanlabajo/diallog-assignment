using CarRental;
using CarRental.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ILogger<CarsController> logger;
        private readonly ICarService carService;

        public CarsController(ILogger<CarsController> logger,
            ICarService carService)
        {
            this.logger = logger;
            this.carService = carService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IEnumerable<Car>> Get()
        {
            return await carService.GetCars();
        }

        // GET api/<ValuesController>/sedan
        [HttpGet("{type}")]
        public async Task<IEnumerable<Car>> Get(CarType type)
        {
            var cars = await carService.GetCars();
            var carsByType = cars.Where(c => c.Type == type).ToList();
            return carsByType;
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<ServiceResult> Post([FromBody] Car car)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return await carService.AddCarDetail(userId, car);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public async Task<ServiceResult> Put(int id, [FromBody] Car car)
        {
            var exists = await carService.GetCar(id);
            if (exists != null)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                return await carService.UpdateCarDetail(userId, car);
            }

            return new ServiceResult();
        }
    }
}
