using Microsoft.EntityFrameworkCore;
using Oracle.DataAccess.Models;


var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de conexi�n a la base de datos
var cadenaConexion = builder.Configuration.GetConnectionString("defaultConnection");
builder.Services.AddDbContext<ModelContext>(x => x.UseOracle(
    cadenaConexion,
    options => options.UseOracleSQLCompatibility("11")
));

// Agregar controladores
builder.Services.AddControllers();

// Configurar Swagger para documentaci�n
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuraci�n de CORS: permitir cualquier origen (solo para desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Permitir cualquier origen
              .AllowAnyMethod() // Permitir cualquier m�todo (GET, POST, etc.)
              .AllowAnyHeader(); // Permitir cualquier cabecera
    });
});

var app = builder.Build();

// Configuraci�n del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplicar CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();