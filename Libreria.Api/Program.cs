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

// Database
var provider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "MySql";
if (provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
{
    var conn = builder.Configuration.GetConnectionString("ConnectionMySql");
    builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseMySql(conn, ServerVersion.AutoDetect(conn)));
}
else
{
    throw new InvalidOperationException("Only MySql is configured in this template.");
}

// Automapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, Assembly.GetExecutingAssembly());

// Services & Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<LibroService>();
builder.Services.AddScoped<AutorService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<FacturaService>();
builder.Services.AddScoped<DetalleFacturaService>();

// MVC + FluentValidation
builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LibroValidator>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
