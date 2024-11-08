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

        [HttpGet]// listar categorias
        public async Task<List<Car>> Listar()
        {
            return await _context.Cars.ToListAsync();

        }

        [HttpGet("{id}")]//buscar por id

        public async Task<ActionResult<Car>> BuscarPorId(decimal id)
        {
            var retorno = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id);

            if (retorno != null)
            {
                return retorno;
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost]
        public async Task<ActionResult<Car>> Guardar([FromBody] Car car)
        {
            if (car == null)
            {
                return BadRequest("El objeto 'car' no puede ser nulo.");
            }

            // Validaciones
            if (string.IsNullOrEmpty(car.Model) || string.IsNullOrEmpty(car.Transmission) ||
                string.IsNullOrEmpty(car.Color) || car.Year <= 0)
            {
                return BadRequest("Asegúrate de que todos los campos requeridos estén completos y sean válidos.");
            }

            try
            {
                // Agregar el carro y sus inventarios
                await _context.Cars.AddAsync(car);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Guardar), new { id = car.Id }, car);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Se encontró un error al guardar el carro: {ex.Message}");
            }
        }




        [HttpPut]
        public async Task<ActionResult<Car>> Actualizar(Car c)
        {
            if (c == null || c.Id == 0)
            {
                return BadRequest("Faltan Datos");
            }

            Car cat = await _context.Cars.FirstOrDefaultAsync(x => x.Id == c.Id);

            if (cat == null)
            {
                return NotFound();
            }

            try
            {
                cat.Model = c.Model;
                await _context.SaveChangesAsync();
                return cat;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Se encontro un error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Eliminar(decimal id)
        {
            Car cat = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id);

            if (cat == null)
            {
                return NotFound();
            }

            try
            {
                _context.Cars.Remove(cat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Se encontro un error");
            }
        }

    }
}
