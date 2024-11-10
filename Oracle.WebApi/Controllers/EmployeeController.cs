using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.DataAccess.Models;
using Microsoft.EntityFrameworkCore;


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

    }
}
