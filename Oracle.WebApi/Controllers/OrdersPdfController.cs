using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Oracle.DataAccess.Models; // Ajustar el namespace según donde estén tus modelos

namespace Oracle.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersPdfController : ControllerBase
    {
        private readonly ModelContext _context;

        public OrdersPdfController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("GetOrdersPdf")]
        public async Task<IActionResult> GetOrdersPdf()
        {
            // 1. Obtener las órdenes con sus facturas desde la base de datos
            var orders = await _context.Orders
                                       .Include(o => o.Invoices)
                                       .ToListAsync();

            // 2. Crear el documento PDF
            using (var document = new PdfDocument())
            {
                document.Info.Title = "Listado de Órdenes";

                // Definir fuente
                var font = new XFont("Arial", 12 );

                foreach (var order in orders)
                {
                    var page = document.AddPage();
                    var gfx = XGraphics.FromPdfPage(page);

                    int startY = 20;
                    int lineHeight = 20;

                    // Escribir datos de la orden
                    gfx.DrawString($"Orden: {order.OrderNumber}", font, XBrushes.Black,
                                   new XRect(20, startY, page.Width - 40, page.Height - 40),
                                   XStringFormats.TopLeft);
                    startY += lineHeight;

                    gfx.DrawString($"Cliente: {order.CustomerName}", font, XBrushes.Black,
                                   new XRect(20, startY, page.Width - 40, page.Height - 40),
                                   XStringFormats.TopLeft);
                    startY += lineHeight;

                    gfx.DrawString($"Modelo de Auto: {order.CarModel}", font, XBrushes.Black,
                                   new XRect(20, startY, page.Width - 40, page.Height - 40),
                                   XStringFormats.TopLeft);
                    startY += lineHeight;

                    gfx.DrawString($"Estado: {order.OrderStatus}", font, XBrushes.Black,
                                   new XRect(20, startY, page.Width - 40, page.Height - 40),
                                   XStringFormats.TopLeft);
                    startY += lineHeight;

                    gfx.DrawString($"Fecha: {order.OrderDate:dd/MM/yyyy}", font, XBrushes.Black,
                                   new XRect(20, startY, page.Width - 40, page.Height - 40),
                                   XStringFormats.TopLeft);
                    startY += lineHeight;

                    // Imprimir facturas
                    gfx.DrawString("Facturas:", font, XBrushes.Black,
                                   new XRect(20, startY, page.Width - 40, page.Height - 40),
                                   XStringFormats.TopLeft);
                    startY += lineHeight;

                    if (order.Invoices != null && order.Invoices.Any())
                    {
                        foreach (var invoice in order.Invoices)
                        {
                            gfx.DrawString($"- Id: {invoice.Id}, Monto: {invoice.TotalAmount}, Estado: {invoice.Status}, Fecha: {invoice.InvoiceDate:dd/MM/yyyy}",
                                           font, XBrushes.Black,
                                           new XRect(40, startY, page.Width - 60, page.Height - 40),
                                           XStringFormats.TopLeft);
                            startY += lineHeight;
                        }
                    }
                    else
                    {
                        gfx.DrawString("No hay facturas", font, XBrushes.Black,
                                       new XRect(40, startY, page.Width - 60, page.Height - 40),
                                       XStringFormats.TopLeft);
                        startY += lineHeight;
                    }
                }

                // 3. Guardar en memoria
                using (var ms = new MemoryStream())
                {
                    document.Save(ms, false);
                    ms.Position = 0;

                    // 4. Devolver el PDF
                    return File(ms.ToArray(), "application/pdf", "ListadoOrdenes.pdf");
                }
            }
        }
    }
}
