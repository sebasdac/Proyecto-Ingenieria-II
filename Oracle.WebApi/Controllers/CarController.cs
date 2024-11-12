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

        // Método HTTP POST para añadir un nuevo carro
        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Car car)
        {
            if (car == null)
            {
                return BadRequest("Car object is null");
            }

            // Validaciones adicionales de los datos
            if (string.IsNullOrEmpty(car.Model) || car.Year <= 0)
            {
                return BadRequest("Model and Year are required fields.");
            }

            try
            {
                // Agregar el nuevo carro a la base de datos
                await _context.Cars.AddAsync(car);
                await _context.SaveChangesAsync();

                return StatusCode(201, car); // Devuelve el carro creado con código 201
            }
            catch (Exception ex)
            {
                // Maneja excepciones durante el proceso
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut]
        public async Task<ActionResult<Car>> Actualizar([FromBody] Car c)
        {
            if (c == null || c.Id == 0)
            {
                return BadRequest("Faltan Datos");
            }

            // Buscar el carro que corresponde al Id proporcionado
            Car carExistente = await _context.Cars.FirstOrDefaultAsync(x => x.Id == c.Id);

            // Si no se encuentra el carro, devolver NotFound
            if (carExistente == null)
            {
                return NotFound();
            }

            // Asignar los nuevos valores a las propiedades del carro
            carExistente.Model = c.Model;
            carExistente.Transmission = c.Transmission;
            carExistente.Color = c.Color;
            carExistente.Year = c.Year;

            // Guardar los cambios en la base de datos
            try
            {
                await _context.SaveChangesAsync();
                return Ok(carExistente);  // Retorna el carro actualizado
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Se encontró un error al actualizar el carro: {ex.Message}");
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
