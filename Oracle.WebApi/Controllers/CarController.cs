using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.DataAccess.Models;
using PdfSharp.Pdf;
using System.Reflection.Metadata;
using PdfSharp.Drawing;

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

        //metodos para detalles de carro a pdf 
        // GET: api/Cars/pdf (PDF)
        // Ahora también incluye los detalles en el PDF de todos los carros
        [HttpGet("pdf")]
        public async Task<ActionResult> GetCarsPdf()
        {
            var cars = await _context.Cars
                .Include(c => c.CarDetails)
                .ToListAsync();

            var document = new PdfDocument();
            document.Info.Title = "Lista de Cars";

            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            var fontTitle = new XFont("Verdana", 16);
            var fontSubtitle = new XFont("Verdana", 14);
            var fontBody = new XFont("Verdana", 12);

            double yPoint = 40;

            // Título principal
            gfx.DrawString("Lista de Cars", fontTitle, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 30;

            foreach (var car in cars)
            {
                // Imprimir información del carro
                gfx.DrawString($"ID: {car.Id}, Model: {car.Model}, Year: {car.Year}", fontSubtitle, XBrushes.Black, new XPoint(40, yPoint));
                yPoint += 20;

                // Imprimir detalles
                if (car.CarDetails.Any())
                {
                    gfx.DrawString("Detalles:", fontBody, XBrushes.Black, new XPoint(60, yPoint));
                    yPoint += 20;

                    foreach (var detail in car.CarDetails)
                    {
                        gfx.DrawString(
                            $"ID: {detail.Id}, Transmisión: {detail.TransmissionType}, Color: {detail.Color}, Stock: {detail.Stock}, Precio: {detail.Price}",
                            fontBody, XBrushes.Black, new XPoint(80, yPoint));
                        yPoint += 20;

                        // Si se acaba la página, crear otra
                        if (yPoint > page.Height - 40)
                        {
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            yPoint = 40;
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Sin detalles.", fontBody, XBrushes.Gray, new XPoint(60, yPoint));
                    yPoint += 20;
                }

                yPoint += 20; // Espacio entre carros

                // Controlar salto de página
                if (yPoint > page.Height - 60)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPoint = 40;
                }
            }

            using var memoryStream = new MemoryStream();
            document.Save(memoryStream, false);

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream.ToArray(), "application/pdf", "cars.pdf");
        }
        // GET: api/Cars/{id}/pdf (PDF)
        [HttpGet("{id}/pdf")]
        public async Task<ActionResult> GetCarPdf(int id)
        {
            var car = await _context.Cars
                .Include(c => c.CarDetails)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound("Car not found.");
            }

            // Crear el documento PDF
            var document = new PdfDocument();
            document.Info.Title = $"Car {car.Id}";

            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            var fontTitle = new XFont("Verdana", 14);
            var fontBody = new XFont("Verdana", 12);

            double yPoint = 40;

            // Información del Car
            gfx.DrawString($"Car ID: {car.Id}", fontTitle, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 25;
            gfx.DrawString($"Model: {car.Model}", fontBody, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 20;
            gfx.DrawString($"Year: {car.Year}", fontBody, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 30;

            gfx.DrawString("Detalles:", fontTitle, XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 25;

            // Listar los detalles
            foreach (var detail in car.CarDetails)
            {
                gfx.DrawString(
                    $"ID: {detail.Id}, " +
                    $"Transmisión: {detail.TransmissionType}, " +
                    $"Color: {detail.Color}, " +
                    $"Stock: {detail.Stock}, " +
                    $"Precio: {detail.Price}",
                    fontBody, XBrushes.Black, new XPoint(40, yPoint));

                yPoint += 20;

                // Si se llena la página, agregar nueva
                if (yPoint > page.Height - 40)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPoint = 40;
                }
            }

            using var memoryStream = new MemoryStream();
            document.Save(memoryStream, false);

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream.ToArray(), "application/pdf", $"car_{car.Id}.pdf");
        }


    }
}
