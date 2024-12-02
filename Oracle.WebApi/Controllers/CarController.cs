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
                .Include(c => c.CarColors)
                .Include(c => c.CarTransmissions)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars
                .Include(c => c.CarColors)
                .Include(c => c.CarTransmissions)
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
                CarColors = car.CarColors.Select(c => new { c.Id, c.Color }),
                CarTransmissions = car.CarTransmissions.Select(t => new { t.Id, t.Transmission })
            });
        }


        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            // Validar que los colores están relacionados con el coche
            if (car.CarColors != null)
            {
                foreach (var color in car.CarColors)
                {
                    color.CarId = car.Id; // Configura la relación
                }
            }

            // Validar que las transmisiones están relacionadas con el coche
            if (car.CarTransmissions != null)
            {
                foreach (var transmission in car.CarTransmissions)
                {
                    transmission.CarId = car.Id; // Configura la relación
                }
            }

            // Agregar el coche con sus relaciones
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }



        // PUT: api/Cars/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cars.Any(c => c.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Cars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars
                .Include(c => c.CarColors)
                .Include(c => c.CarTransmissions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
