using Libreria.Infrastructure.Data;
using Libreria.Infrastructure.Mappings;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Repositories;
using Libreria.Core.Services;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentValidation.AspNetCore;
using Libreria.Api.Validators;

namespace Libreria.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =====================================
            // Controladores y Validadores
            // =====================================
            builder.Services.AddControllers()
                .AddFluentValidation(config =>
                {
                    config.RegisterValidatorsFromAssemblyContaining<LibroCreateValidator>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            // =====================================
            // Base de datos MySQL
            // =====================================
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("MySqlConnection"),
                    new MySqlServerVersion(new Version(8, 0, 36)),
                    b => b.MigrationsAssembly("Libreria.Api")
                )
            );

            // =====================================
            // AutoMapper
            // =====================================
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // =====================================
            // Repositorios
            // =====================================
            builder.Services.AddScoped<ILibroRepository, LibroRepository>();
            builder.Services.AddScoped<IAutorRepository, AutorRepository>();
            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
            builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();

            // =====================================
            // Servicios (Lógica de negocio)
            // =====================================
            builder.Services.AddScoped<LibroService>();
            builder.Services.AddScoped<AutorService>();
            builder.Services.AddScoped<ClienteService>();
            builder.Services.AddScoped<FacturaService>();

            // =====================================
            // Construcción de la aplicación
            // =====================================
            var app = builder.Build();

            // =====================================
            // Middleware
            // =====================================
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // =====================================
            //  Ejecutar la aplicación
            // =====================================
            app.Run();
        }
    }
}
