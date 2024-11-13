using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.DataAccess.Models;

namespace Oracle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ModelContext _context;

        public SuppliersController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet]
        // Método para listar todos los proveedores
        public async Task<ActionResult<IEnumerable<Supplier>>> Listar()
        {
            return await _context.Suppliers.ToListAsync();
        }

        // Método para buscar un proveedor por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> BuscarPorId(long id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();  // Si no existe el proveedor, devolver 404
            }

            return supplier;
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> Guardar([FromBody] Supplier supplier)
        {
            if (supplier == null)
            {
                return BadRequest("El objeto 'supplier' no puede ser nulo.");
            }

            // Validaciones
            if (string.IsNullOrEmpty(supplier.SupplierName))
            {
                return BadRequest("Asegúrate de que todos los campos requeridos estén completos y sean válidos.");
            }

            try
            {
                // Agregar el proveedor
                await _context.Suppliers.AddAsync(supplier);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Guardar), new { id = supplier.Id }, supplier);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Se encontró un error al guardar el proveedor: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult<Supplier>> Actualizar(Supplier supplier)
        {
            if (supplier == null || supplier.Id == 0)
            {
                return BadRequest("Faltan Datos");
            }

            var existingSupplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == supplier.Id);

            if (existingSupplier == null)
            {
                return NotFound();
            }

            try
            {
                existingSupplier.SupplierName = supplier.SupplierName;
                existingSupplier.ContactName = supplier.ContactName;
                existingSupplier.Email = supplier.Email;
                existingSupplier.Phone = supplier.Phone;
                existingSupplier.Address = supplier.Address;
                await _context.SaveChangesAsync();
                return existingSupplier;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Se encontró un error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Eliminar(long id)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == id);

            if (supplier == null)
            {
                return NotFound();
            }

            try
            {
                _context.Suppliers.Remove(supplier);
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
