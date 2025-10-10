using Libreria.Infrastructure.Data;
using Libreria.Infrastructure.Mappings;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Repositories;
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

            // Controladores y FluentValidation
            builder.Services.AddControllers()
                .AddFluentValidation(config =>
                {
                    config.RegisterValidatorsFromAssemblyContaining<LibroCreateValidator>();
                });

            // ✅ Base de datos MySQL
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("MySqlConnection"),
                    new MySqlServerVersion(new Version(8, 0, 36)),
                    b => b.MigrationsAssembly("Libreria.Api")
                )
            );

            // ✅ AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // ✅ Inyección de repositorios
            builder.Services.AddScoped<ILibroRepository, LibroRepository>();
            builder.Services.AddScoped<IAutorRepository, AutorRepository>();
            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
            builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();

            // Add services to the container.
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
