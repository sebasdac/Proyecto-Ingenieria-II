using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;



namespace Oracle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private ModelContext _context;
        public EmployeeController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet]// listar categorias
        public async Task<List<Employee>> Listar()
        {
            return await _context.Employees.ToListAsync();

        }

        [HttpGet("{id}")]//buscar por id

        public async Task<ActionResult<Employee>> BuscarPorId(long id)
        {
            var retorno = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

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
        public async Task<ActionResult<Employee>> Guardar([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("El objeto 'employee' no puede ser nulo.");
            }

            // Validaciones
            if (string.IsNullOrEmpty(employee.EmployeeName) || string.IsNullOrEmpty(employee.Position) ||
                string.IsNullOrEmpty(employee.LastName) || string.IsNullOrEmpty(employee.Cedula) ||
                string.IsNullOrEmpty(employee.Phone) || string.IsNullOrEmpty(employee.Email) ||
                employee.Salary <= 0 || employee.HireDate == default(DateTime))
            {
                return BadRequest("Asegúrate de que todos los campos requeridos estén completos y sean válidos.");
            }
            
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Cedula == employee.Cedula || e.Email == employee.Email);

            if (existingEmployee != null)
            {
                return BadRequest("Ya existe un empleado con la misma cédula o email.");
            }

            try
            {
                // Agregar el empleado
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Guardar), new { id = employee.Id }, employee);
            }
            catch (DbUpdateException ex)
            {
                
                if (ex.InnerException?.Message.Contains("ORA-00001") == true)
                {
                    return BadRequest("El valor de Cedula o Email ya existe en la base de datos.");
                }
                else
                {
                    return StatusCode(500, $"Error en la base de datos: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }


        [HttpPut]
        public async Task<ActionResult<Employee>> Actualizar(Employee c)
        {
            if (c == null || c.Id == 0)
            {
                return BadRequest("Faltan Datos");
            }


            // Buscar el empleado por Id
            Employee employee = await _context.Employees.FindAsync(c.Id);



            if (employee == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            // Validación de los campos antes de la actualización
            if (string.IsNullOrEmpty(c.EmployeeName) || string.IsNullOrEmpty(c.Position) ||
                string.IsNullOrEmpty(c.LastName) || string.IsNullOrEmpty(c.Cedula) ||
                string.IsNullOrEmpty(c.Phone) || string.IsNullOrEmpty(c.Email) ||
                c.Salary <= 0 || c.HireDate == default(DateTime))
            {
                return BadRequest("Datos incompletos o inválidos.");
            }

            try
            {
                // Actualizar los campos del empleado
                employee.EmployeeName = c.EmployeeName;
                employee.Position = c.Position;
                employee.LastName = c.LastName;
                employee.Cedula = c.Cedula;
                employee.Phone = c.Phone;
                employee.Email = c.Email;
                employee.Salary = c.Salary;
                employee.HireDate = c.HireDate;

                // Guardar los cambios
                await _context.SaveChangesAsync();

                return Ok(employee); // Retorna el empleado actualizado
            }
            catch (DbUpdateException ex)
            {
                // Manejo de excepción si ocurre un error al guardar los cambios
                return StatusCode(500, $"Error en la base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Manejo de otras excepciones
                return StatusCode(500, $"Se encontró un error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Eliminar(decimal id)
        {
            Employee cat = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (cat == null)
            {
                return NotFound();
            }

            try
            {
                _context.Employees.Remove(cat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Se encontro un error");
            }
        }

        //exporat empleados a pdf
        [HttpGet("ExportarEmpleadosPDF")]
        public async Task<IActionResult> ExportarEmpleadosPDF()
        {
            // Obtener datos de la base de datos
            var empleados = await _context.Employees.ToListAsync();

            // Crear un documento PDF
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Lista de Empleados";

            // Crear una página en el PDF
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont titleFont = new XFont("Arial", 14);
            XFont tableFont = new XFont("Arial", 10);

            // Título
            gfx.DrawString("Lista de Empleados", titleFont, XBrushes.Black, new XPoint(40, 40));

            // Coordenadas iniciales para la tabla
            double yPoint = 70;

            // Dibujar encabezados de la tabla
            gfx.DrawString("ID", tableFont, XBrushes.Black, new XPoint(40, yPoint));
            gfx.DrawString("Nombre", tableFont, XBrushes.Black, new XPoint(80, yPoint));
            gfx.DrawString("Puesto", tableFont, XBrushes.Black, new XPoint(200, yPoint));
            gfx.DrawString("Salario", tableFont, XBrushes.Black, new XPoint(300, yPoint));
            gfx.DrawString("Teléfono", tableFont, XBrushes.Black, new XPoint(400, yPoint));
            gfx.DrawString("Correo", tableFont, XBrushes.Black, new XPoint(500, yPoint));

            yPoint += 20; // Espaciado

            // Agregar filas de datos
            foreach (var empleado in empleados)
            {
                gfx.DrawString(empleado.Id.ToString(), tableFont, XBrushes.Black, new XPoint(40, yPoint));
                gfx.DrawString(empleado.EmployeeName, tableFont, XBrushes.Black, new XPoint(80, yPoint));
                gfx.DrawString(empleado.Position, tableFont, XBrushes.Black, new XPoint(200, yPoint));
                gfx.DrawString(empleado.Salary.ToString("C"), tableFont, XBrushes.Black, new XPoint(300, yPoint));
                gfx.DrawString(empleado.Phone, tableFont, XBrushes.Black, new XPoint(400, yPoint));
                gfx.DrawString(empleado.Email, tableFont, XBrushes.Black, new XPoint(500, yPoint));

                yPoint += 20;

                // Si llega al final de la página, agregar una nueva página
                if (yPoint > page.Height - 50)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPoint = 40;
                }
            }

            // Guardar el PDF en memoria
            using (var stream = new MemoryStream())
            {
                document.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                // Devolver el PDF como respuesta
                return File(stream.ToArray(), "application/pdf", "ListaEmpleados.pdf");
            }
        }

    }
}
