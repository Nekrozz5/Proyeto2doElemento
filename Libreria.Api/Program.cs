using System.Reflection;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Libreria.Core.Interfaces;
using Libreria.Core.Services;
using Libreria.Infrastructure.Data;
using Libreria.Infrastructure.Mappings;
using Libreria.Infrastructure.Repositories;
using Libreria.Infrastructure.Validators;
using Libreria.Api.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// 🔹 CONFIGURAR BASE DE DATOS
// ==================================================
var provider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "MySql";
if (provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
{
    var conn = builder.Configuration.GetConnectionString("ConnectionMySql");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(conn, ServerVersion.AutoDetect(conn)));
}
else
{
    throw new InvalidOperationException("Only MySql is configured in this template.");
}

// ==================================================
// 🔹 AUTOMAPPER
// ==================================================
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, Assembly.GetExecutingAssembly());

// ==================================================
// 🔹 UNIT OF WORK + REPOSITORIO GENÉRICO
// ==================================================
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ==================================================
// 🔹 REGISTRO DE DAPPER
// ==================================================
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDapperContext, DapperContext>();

// ==================================================
// 🔹 SERVICIOS CORE
// ==================================================
builder.Services.AddScoped<LibroService>();
builder.Services.AddScoped<AutorService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<FacturaService>();
builder.Services.AddScoped<DetalleFacturaService>();

// ==================================================
// 🔹 FLUENT VALIDATION + MVC
// ==================================================
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LibroValidator>();

// ==================================================
// 🔹 CONFIGURAR SWAGGER
// ==================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Librería API",
        Version = "v1",
        Description = "Documentación de la API del sistema de Librería (UCB) - .NET 9",
        Contact = new OpenApiContact
        {
            Name = "Equipo de Desarrollo UCB",
            Email = "desarrollo@ucb.edu.bo"
        }
    });

    // 🔹 Incluir comentarios XML de controladores y entidades
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }


    // 🔹 Mostrar anotaciones de tipos complejos (SwaggerSchema)
    options.EnableAnnotations();
});

// ==================================================
// 🔹 CONSTRUIR APLICACIÓN
// ==================================================
var app = builder.Build();

// ==================================================
// 🔹 MIDDLEWARE GLOBAL DE EXCEPCIONES
// ==================================================
app.UseMiddleware<GlobalExceptionMiddleware>();

// ==================================================
// 🔹 SWAGGER (solo en desarrollo)
// ==================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Librería API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

// ==================================================
// 🔹 PIPELINE GENERAL
// ==================================================
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
