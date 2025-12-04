using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Libreria.Api.Middlewares;
using Libreria.Core.Interfaces;
using Libreria.Core.Services;
using Libreria.Infrastructure.Data;
using Libreria.Infrastructure.Mappings;
using Libreria.Infrastructure.Repositories;
using Libreria.Infrastructure.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// 🔥 CONFIGURACIÓN BASE (OBLIGATORIA PARA AZURE)
// ==================================================
builder.Configuration.Sources.Clear();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); // Azure lee todo desde aquí

// ==================================================
// 🔥 USER SECRETS SOLO EN DESARROLLO
// ==================================================
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    Console.WriteLine("✔ User Secrets habilitados");
}

// ==================================================
// 🔥 BASE DE DATOS MYSQL AZURE
// ==================================================
var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "❌ No se encontró ConnectionMySql.\nConfigúralo en UserSecrets o en Variables de Entorno de Azure.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)))
);

// ==================================================
// 🔥 AUTOMAPPER
// ==================================================
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, Assembly.GetExecutingAssembly());

// ==================================================
// 🔥 UNIT OF WORK + REPO GENÉRICO
// ==================================================
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ==================================================
// 🔥 DAPPER
// ==================================================
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDapperContext, DapperContext>();

// ==================================================
// 🔥 SERVICIOS
// ==================================================
builder.Services.AddScoped<LibroService>();
builder.Services.AddScoped<AutorService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<FacturaService>();
builder.Services.AddScoped<DetalleFacturaService>();
builder.Services.AddScoped<SecurityService>();

// ==================================================
// 🔥 MVC + JSON + VALIDACIÓN
// ==================================================
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LibroValidator>();

// ==================================================
// 🔥 VERSIONADO API
// ==================================================
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version")
    );
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ==================================================
// 🔥 AUTENTICACIÓN JWT
// ==================================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]!)
        )
    };
});

// ==================================================
// 🔥 SWAGGER SIEMPRE HABILITADO (REQUISITO DE AZURE)
// ==================================================
builder.Services.AddEndpointsApiExplorer(); // <-- Obligatorio para publicar
builder.Services.AddSwaggerGen(options =>
{
    // XML (para documentar con ///)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    // JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer. Ej: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ==================================================
// 🔥 BUILD PIPELINE
// ==================================================
var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger(); // Swagger siempre activo
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Librería API v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
