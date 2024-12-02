using FluentAssertions.Common;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Oracle.DataAccess.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
// Registrar el tipo MIME para .avif
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".avif"] = "image/avif"; // Configuración del tipo MIME

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider // Agregar el proveedor de tipos MIME
});



app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Store}/{action=Index}/{id?}");

app.Run();
