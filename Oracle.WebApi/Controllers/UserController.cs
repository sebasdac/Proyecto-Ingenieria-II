using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;  
namespace Oracle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ModelContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ModelContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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



        [HttpPost("Login")]
        public IActionResult Login([FromBody] User loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("El nombre de usuario y la contraseña son obligatorios.");
            }

            // Buscar el usuario en la base de datos
            var user = _context.Users.FirstOrDefault(u => u.Username == loginRequest.Username);

            if (user == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // Verificar si la contraseña ingresada coincide con el hash de la contraseña almacenada
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // Obtener datos del cliente relacionados al usuario
            var customer = _context.Customers.FirstOrDefault(c => c.Id == user.CustomerId);
            if (customer == null)
            {
                return Unauthorized("No se encontraron los datos del cliente.");
            }

            // Generar el token JWT incluyendo los datos adicionales del cliente
            var token = GenerateJwtToken(user, customer);

            return Ok(new { Token = token });
        }



        // Método para generar el token JWT
        private string GenerateJwtToken(User user, Customer customer)
        {
                // Definir los claims (información adicional en el token)
                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("role", user.Role),
            new Claim("userId", user.Id.ToString()),
            new Claim("name", customer.Name), // Nombre del cliente
            new Claim("cedula", customer.Cedula), // Cédula
            new Claim("phone", customer.Phone), // Teléfono
            new Claim("address", customer.Address), // Dirección
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // Obtener la llave secreta del archivo appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Configurar el token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1), // Expiración del token
                signingCredentials: creds
            );

            // Serializar el token a string
            return new JwtSecurityTokenHandler().WriteToken(token);
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

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) ||
                string.IsNullOrEmpty(user.Role) || (user.Role != "Administrador" && user.Role != "Cliente"))
            {
                return BadRequest("Datos incompletos o inválidos.");
            }

            if (user.Customer == null)
            {
                return BadRequest("Los datos del cliente son requeridos.");
            }

            // Validar si el username ya existe
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null)
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Crear el cliente
                var newCustomer = new Customer
                {
                    Name = user.Customer.Name,
                    Cedula = user.Customer.Cedula,
                    BirthDate = user.Customer.BirthDate,
                    Phone = user.Customer.Phone,
                    Address = user.Customer.Address
                };

                await _context.Customers.AddAsync(newCustomer);
                await _context.SaveChangesAsync();

                // Asociar el cliente al usuario
                user.CustomerId = newCustomer.Id;
                user.Customer = null; // Evitar problemas de referencia circular

                // Crear el usuario
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return CreatedAtAction(nameof(Guardar), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al guardar los datos: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
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
        public async Task<ActionResult<User>> Actualizar(User c)
        {
            if (c == null || c.Id == 0)
            {
                return BadRequest("Faltan Datos");
            }

            User user = await _context.Users.FindAsync(c.Id);

            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

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

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Usuario actualizado correctamente",
                    User = user
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error en la base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
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
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error en la base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se encontró un error: {ex.Message}");
            }
        }
    }
}

