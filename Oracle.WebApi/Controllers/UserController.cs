using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

namespace Oracle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class UserController : ControllerBase
    {
        private ModelContext _context;
        public UserController(ModelContext context)
        {
            _context = context;
        }

        [HttpOptions]
        [Route("")]
        public IActionResult Options()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:7214");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> ListarUser()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                if (users == null || !users.Any())
                {
                    return NotFound("No se encontraron usuarios.");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los usuarios: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> Guardar([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("El objeto 'user' no puede ser nulo.");
            }

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("El nombre de usuario y la contraseña son obligatorios.");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null)
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Guardar), new { id = user.Id }, user);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error en la base de datos: {ex.Message}");
            }
        }







        [HttpGet("{id}")]//buscar por id

        public async Task<ActionResult<User>> BuscarPorId(long id)
        {
            var retorno = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (retorno != null)
            {
                return retorno;
            }
            else
            {
                return NotFound();
            }

        }
        



        [HttpPut]
        public async Task<ActionResult<Employee>> Actualizar(User c)
        {
            if (c == null || c.Id == 0)
            {
                return BadRequest("Faltan Datos");
            }


            // Buscar el usuario por Id
            User user = await _context.Users.FindAsync(c.Id);



            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Validación de los campos antes de la actualización
            if (string.IsNullOrEmpty(c.Username) || string.IsNullOrEmpty(c.Password) ||
                string.IsNullOrEmpty(c.Role) || (c.Role != "Administrador" && c.Role != "Cliente") ||
                (c.CustomerId.HasValue && c.CustomerId <= 0))
            {
                return BadRequest("Datos incompletos o inválidos.");
            }



            try
            {
               
                user.Username = c.Username;
                user.Password = c.Password;
                user.Role = c.Role;
                user.CustomerId = c.CustomerId; 

                // Guardar los cambios
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Usuario actualizado correctamente",
                    User = user 
                });
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
            User cat = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (cat == null)
            {
                return NotFound();
            }

            try
            {
                _context.Users.Remove(cat);
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
