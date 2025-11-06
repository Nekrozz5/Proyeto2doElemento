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
            //  Controladores y Validadores
            // =====================================
            builder.Services.AddControllers()
                .AddFluentValidation(config =>
                {
                    config.RegisterValidatorsFromAssemblyContaining<LibroCreateValidator>();
                })
                .AddNewtonsoftJson(options =>
                {
                    // Evita bucles de referencias en JSON (importante con EF)
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Permite manejar manualmente los errores de validación
                    options.SuppressModelStateInvalidFilter = true;
                });

            // =====================================
            //  Base de datos 
            // =====================================
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("ConnectionMySql"),
                    new MySqlServerVersion(new Version(8, 0, 36)),
                    b => b.MigrationsAssembly("Libreria.Api")
                )
            );

            //  AutoMapper

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Repositorios

            builder.Services.AddScoped<ILibroRepository, LibroRepository>();
            builder.Services.AddScoped<IAutorRepository, AutorRepository>();
            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
            builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();

            // Servicios (Lógica de negocio

            builder.Services.AddScoped<LibroService>();
            builder.Services.AddScoped<AutorService>();
            builder.Services.AddScoped<ClienteService>();
            builder.Services.AddScoped<FacturaService>();

            // dapper

            builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();

            //Configuracion de Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "Backend Libreria API",
                    Version = "v1",
                    Description = "Documentacion de la API de Libreria - NET 9",
                    Contact = new()
                    {
                        Name = "Equipo de Desarrollo UCB",
                        Email = "desarrollo@ucb.edu.bo"
                    }
                });
            });


            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
            } 
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
