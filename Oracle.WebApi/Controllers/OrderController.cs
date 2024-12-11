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
        //metodo para traer ordenes y facturas
        [HttpGet("ListOrdersWithInvoices")]
        public async Task<ActionResult<IEnumerable<Order>>> ListOrdersWithInvoices()
        {
            var ordersWithInvoices = await _context.Orders
                                                    .Include(o => o.Invoices) // Carga las facturas relacionadas
                                                    .ToListAsync();
            return ordersWithInvoices;
        }

        [HttpGet("GetOrderPdf/{id}")]
        public async Task<IActionResult> GetOrderPdf(int id)
        {
            // 1. Obtener la orden con sus facturas
            var order = await _context.Orders
                                      .Include(o => o.Invoices)
                                      .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound($"No se encontró la orden con el ID {id}");
            }

            // 2. Crear el documento PDF
            using (var document = new PdfSharp.Pdf.PdfDocument())
            {
                document.Info.Title = $"Orden {order.OrderNumber}";

                var font = new PdfSharp.Drawing.XFont("Arial", 12);
                var page = document.AddPage();
                var gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page);

                int startY = 20;
                int lineHeight = 20;

                // Escribir datos de la orden
                gfx.DrawString($"Orden: {order.OrderNumber}", font, PdfSharp.Drawing.XBrushes.Black,
                               new PdfSharp.Drawing.XRect(20, startY, page.Width - 40, page.Height - 40),
                               PdfSharp.Drawing.XStringFormats.TopLeft);
                startY += lineHeight;

                gfx.DrawString($"Cliente: {order.CustomerName}", font, PdfSharp.Drawing.XBrushes.Black,
                               new PdfSharp.Drawing.XRect(20, startY, page.Width - 40, page.Height - 40),
                               PdfSharp.Drawing.XStringFormats.TopLeft);
                startY += lineHeight;

                gfx.DrawString($"Modelo de Auto: {order.CarModel}", font, PdfSharp.Drawing.XBrushes.Black,
                               new PdfSharp.Drawing.XRect(20, startY, page.Width - 40, page.Height - 40),
                               PdfSharp.Drawing.XStringFormats.TopLeft);
                startY += lineHeight;

                

                gfx.DrawString($"Fecha: {order.OrderDate:dd/MM/yyyy}", font, PdfSharp.Drawing.XBrushes.Black,
                               new PdfSharp.Drawing.XRect(20, startY, page.Width - 40, page.Height - 40),
                               PdfSharp.Drawing.XStringFormats.TopLeft);
                startY += lineHeight;

                // Imprimir facturas
                gfx.DrawString("Facturas:", font, PdfSharp.Drawing.XBrushes.Black,
                               new PdfSharp.Drawing.XRect(20, startY, page.Width - 40, page.Height - 40),
                               PdfSharp.Drawing.XStringFormats.TopLeft);
                startY += lineHeight;

                if (order.Invoices != null && order.Invoices.Any())
                {
                    foreach (var invoice in order.Invoices)
                    {
                        gfx.DrawString($"- Id: {invoice.Id}, Monto: {invoice.TotalAmount}, Estado: {invoice.Status}, Fecha: {invoice.InvoiceDate:dd/MM/yyyy}",
                                       font, PdfSharp.Drawing.XBrushes.Black,
                                       new PdfSharp.Drawing.XRect(40, startY, page.Width - 60, page.Height - 40),
                                       PdfSharp.Drawing.XStringFormats.TopLeft);
                        startY += lineHeight;
                    }
                }
                else
                {
                    gfx.DrawString("No hay facturas", font, PdfSharp.Drawing.XBrushes.Black,
                                   new PdfSharp.Drawing.XRect(40, startY, page.Width - 60, page.Height - 40),
                                   PdfSharp.Drawing.XStringFormats.TopLeft);
                    startY += lineHeight;
                }

                // 3. Guardar en memoria
                using (var ms = new System.IO.MemoryStream())
                {
                    document.Save(ms, false);
                    ms.Position = 0;

                    // 4. Devolver el PDF
                    return File(ms.ToArray(), "application/pdf", $"Orden_{order.OrderNumber}.pdf");
                }
            }
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> BuscarPorId(long id)
        {
            // Incluimos la carga de las facturas relacionadas usando Include
            var order = await _context.Orders
                                      .Include(o => o.Invoices)
                                      .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();  // Devuelve 404 si no existe la orden con ese ID
            }

            return order; // Retorna la orden junto a sus facturas
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

        //obtener orden por su Order ID
        [HttpGet("GetOrderJson/{orderNumber}")]
        public async Task<IActionResult> GetOrderJson(string orderNumber)
        {
            // Validar que el número de orden no sea nulo o vacío
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                return BadRequest("El número de orden no puede estar vacío.");
            }

            // Obtener la orden con sus facturas usando OrderNumber
            var order = await _context.Orders
                                      .Include(o => o.Invoices)
                                      .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                return NotFound(new { Message = $"No se encontró la orden con el número {orderNumber}" });
            }

            // Preparar la respuesta JSON
            var result = new
            {
                order.OrderNumber,
                order.CustomerName,
                order.CarModel,
                order.OrderStatus,
                OrderDate = order.OrderDate.ToString("yyyy-MM-dd"),
                Invoices = order.Invoices.Select(invoice => new
                {
                    invoice.Id,
                    invoice.TotalAmount,
                    invoice.Status,
                    InvoiceDate = invoice.InvoiceDate.ToString("yyyy-MM-dd")
                })
            };

            return Ok(result);
        }

        //traer todas las ordenes con sus facturas
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            // Obtener todas las órdenes con sus facturas
            var orders = await _context.Orders
                                       .Include(o => o.Invoices)
                                       .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound(new { Message = "No se encontraron órdenes en la base de datos." });
            }

            // Preparar la respuesta JSON
            var result = orders.Select(order => new
            {
                order.OrderNumber,
                order.CustomerName,
                order.CarModel,
                order.OrderStatus,
                OrderDate = order.OrderDate.ToString("yyyy-MM-dd"),
                Invoices = order.Invoices.Select(invoice => new
                {
                    invoice.Id,
                    invoice.TotalAmount,
                    invoice.Status,
                    InvoiceDate = invoice.InvoiceDate.ToString("yyyy-MM-dd")
                })
            });

            return Ok(result);
        }




    }
}
