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

        [HttpPut]//actualizar
        public async Task<IActionResult> ActualizarFactura([FromBody] Invoice invoice)
        {
            if (!_context.Invoices.Any(i => i.Id == invoice.Id))
            {
                return BadRequest("El ID de la factura no existe.");
            }

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            return Ok();
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
