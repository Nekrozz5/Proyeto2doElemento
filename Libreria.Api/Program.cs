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
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 CONFIGURAR BASE DE DATOS
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

// 🔹 AUTOMAPPER
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, Assembly.GetExecutingAssembly());

// 🔹 UNIT OF WORK + REPOSITORIO GENÉRICO
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ✅ 🔹 REGISTRO DE DAPPER (complemento)
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDapperContext, DapperContext>();

// 🔹 SERVICIOS DE LA CAPA CORE
builder.Services.AddScoped<LibroService>();
builder.Services.AddScoped<AutorService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<FacturaService>();
builder.Services.AddScoped<DetalleFacturaService>();

// 🔹 FLUENT VALIDATION + MVC
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LibroValidator>();

// 🔹 SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 PIPELINE DE APLICACIÓN
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<Libreria.Api.Middlewares.GlobalExceptionMiddleware>();

app.MapControllers();
app.Run();
