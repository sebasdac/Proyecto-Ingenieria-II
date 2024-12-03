using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.DataAccess.Models;

namespace Oracle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private ModelContext _context;
        public CarController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Cars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            return await _context.Cars
                
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars
                .Include(c => c.CarDetails) // Incluir los detalles relacionados
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                car.Id,
                car.Model,
                car.Year,
                carDetails = car.CarDetails.Select(detail => new
                {
                    detail.Id,
                    detail.TransmissionType,
                    detail.Color,
                    detail.Stock,
                    detail.Price
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            // Validar que CarDetails no sea nulo
            if (car.CarDetails != null)
            {
                foreach (var detail in car.CarDetails)
                {
                    // Asociar el detalle al carro
                    detail.CarId = car.Id;
                    _context.CarDetails.Add(detail);
                }
            }

            // Agregar el carro a la base de datos
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest("El ID del carro no coincide.");
            }

            // Validar existencia del carro
            var existingCar = await _context.Cars
                .Include(c => c.CarDetails)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCar == null)
            {
                return NotFound("El carro no existe.");
            }

            // Actualizar datos del carro principal
            existingCar.Model = car.Model;
            existingCar.Year = car.Year;

            // Actualizar detalles (CarDetails)
            foreach (var detail in car.CarDetails)
            {
                var existingDetail = existingCar.CarDetails.FirstOrDefault(d => d.Id == detail.Id);

                if (existingDetail != null)
                {
                    // Actualizar detalle existente
                    existingDetail.TransmissionType = detail.TransmissionType;
                    existingDetail.Color = detail.Color;
                    existingDetail.Stock = detail.Stock;
                    existingDetail.Price = detail.Price;
                }
                else
                {
                    // Agregar nuevo detalle
                    detail.CarId = id;
                    existingCar.CarDetails.Add(detail);
                }
            }

            // Eliminar detalles que ya no están en la solicitud
            var detailsToRemove = existingCar.CarDetails
                .Where(d => !car.CarDetails.Any(cd => cd.Id == d.Id))
                .ToList();

            foreach (var detail in detailsToRemove)
            {
                _context.CarDetails.Remove(detail);
            }

            // Guardar cambios
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cars.Any(c => c.Id == id))
                {
                    return NotFound("El carro no existe.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            // Buscar el carro incluyendo sus detalles
            var car = await _context.Cars
                .Include(c => c.CarDetails)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound("El carro no existe.");
            }

            // Eliminar el carro y sus detalles asociados
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
