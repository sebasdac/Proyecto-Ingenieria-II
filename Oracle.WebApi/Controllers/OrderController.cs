using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.DataAccess.Models;

namespace Oracle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ModelContext _context;

        public OrderController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet]
        // Método para listar todas las órdenes
        public async Task<ActionResult<IEnumerable<Order>>> Listar()
        {
            return await _context.Orders.ToListAsync();
        }

        // Método para buscar una orden por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> BuscarPorId(long id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();  // Si no existe la orden, devolver 404
            }

            return order;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Guardar([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("El objeto 'order' no puede ser nulo.");
            }

            // Validaciones
            if (string.IsNullOrEmpty(order.OrderNumber) || string.IsNullOrEmpty(order.CustomerName) ||
                string.IsNullOrEmpty(order.CarModel) || string.IsNullOrEmpty(order.OrderStatus))
            {
                return BadRequest("Asegúrate de que todos los campos requeridos estén completos y sean válidos.");
            }

            try
            {
                // Agregar la orden
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Guardar), new { id = order.Id }, order);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Se encontró un error al guardar la orden: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult<Order>> Actualizar(Order order)
        {
            if (order == null || order.Id == 0)
            {
                return BadRequest("Faltan Datos");
            }

            var existingOrder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == order.Id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            try
            {
                existingOrder.OrderNumber = order.OrderNumber;
                existingOrder.CustomerName = order.CustomerName;
                existingOrder.CarModel = order.CarModel;
                existingOrder.OrderStatus = order.OrderStatus;
                existingOrder.OrderDate = order.OrderDate;
                await _context.SaveChangesAsync();
                return existingOrder;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Se encontró un error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Eliminar(long id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Se encontró un error");
            }
        }
    }
}
