using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.DataAccess.Models;
using System.Text.RegularExpressions;

namespace Oracle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private ModelContext _context;
        public CustomersController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet]// listar customers
        public async Task<List<Customer>> Listar()
        {
            return await _context.Customers.ToListAsync();

        }
        [HttpPost]//guardar
        public async Task<ActionResult<Customer>> Guardar([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("El objeto 'customer' no puede ser nulo.");
            }

            // Validaciones
            if (string.IsNullOrEmpty(customer.Name) ||
                string.IsNullOrEmpty(customer.Cedula) ||
                string.IsNullOrEmpty(customer.Phone) ||
                string.IsNullOrEmpty(customer.Address))
            {
                return BadRequest("Asegúrate de que todos los campos requeridos estén completos y sean válidos.");
            }

            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Name == customer.Name && x.Cedula == customer.Cedula);

            if (existingCustomer != null)
            {
                return BadRequest("Ya existe un cliente con el mismo nombre y cédula.");
            }

            // Guardar el nuevo cliente en la base de datos
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }
        [HttpPut]
        public async Task<ActionResult<Customer>> Actualizar([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("El objeto 'customer' no puede ser nulo.");
            }

            // Validaciones de campos requeridos
            if (string.IsNullOrEmpty(customer.Name) ||
                string.IsNullOrEmpty(customer.Cedula) ||
                string.IsNullOrEmpty(customer.Phone) ||
                string.IsNullOrEmpty(customer.Address))
            {
                return BadRequest("Asegúrate de que todos los campos requeridos estén completos y sean válidos.");
            }

            // Verificar si el cliente existe
            var existingCustomer = await _context.Customers.FindAsync(customer.Id);

            if (existingCustomer == null)
            {
                return NotFound("No se encontró el cliente especificado.");
            }

            // Actualizar campos específicos
            _context.Entry(existingCustomer).CurrentValues.SetValues(customer);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict("Hubo un conflicto de concurrencia al actualizar el cliente: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }

            return Ok(existingCustomer);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(long id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound("No se encontró el cliente especificado.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok();
        }
        //buscar por id
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> BuscarPorId(long id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound("No se encontró el cliente especificado.");
            }

            return Ok(customer);
        }





    }

}
