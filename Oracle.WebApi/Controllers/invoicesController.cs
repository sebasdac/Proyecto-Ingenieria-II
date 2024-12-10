using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.DataAccess.Models;

namespace Oracle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class invoicesController : ControllerBase
    {
        private ModelContext _context;

        public invoicesController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet]// listar facturas
        public async Task<List<Invoice>> ListarFacturas()
        {

            return await _context.Invoices.ToListAsync();
        }

        [HttpPost]//generar factura
        public async Task<IActionResult> CrearFactura([FromBody] Invoice invoice)
        {
            if (!_context.Orders.Any(o => o.Id == invoice.OrderId))
            {
                return BadRequest("El ID de la orden no existe.");
            }

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CrearFactura), new { id = invoice.Id }, invoice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarFactura(long id, [FromBody] Invoice invoice)
        {
            try
            {
          
                var existingInvoice = await _context.Invoices.FindAsync(id);
                if (existingInvoice == null)
                {
                    return NotFound(new { message = "Factura no encontrada." });
                }


                existingInvoice.Status = invoice.Status;

              
                _context.Invoices.Update(existingInvoice);
                await _context.SaveChangesAsync();

                return Ok(existingInvoice);  
            }
            catch (Exception ex)
            {
 
                Console.WriteLine($"Error al actualizar la factura: {ex.Message}");
                return StatusCode(500, new { message = "Ocurrió un error al actualizar la factura." });
            }
        }

        //buscar por id de factura

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarFactura(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarFactura(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return Ok();
        }


    }
}
