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
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// =============================================
// 🔹 USER SECRETS (SOLO DESARROLLO)
// =============================================
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    Console.WriteLine("✔ User Secrets cargados correctamente.");
}

// ==================================================
// 🔹 BASE DE DATOS (MYSQL AZURE)
// ==================================================
var providerDb = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "MySql";

if (providerDb.Equals("MySql", StringComparison.OrdinalIgnoreCase))
{
    var conn = builder.Configuration.GetConnectionString("ConnectionMySql");

    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException("❌ No se encontró ConnectionMySql. Verifica UserSecrets.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(conn, new MySqlServerVersion(new Version(8, 0, 36))));
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
// 🔹 DAPPER
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
builder.Services.AddScoped<SecurityService>();


// ==================================================
// 🔹 MVC + NEWTONSOFT + FLUENTVALIDATION
// ==================================================
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LibroValidator>();

// ==================================================
// 🔹 API VERSIONING
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

// ==================================================
// 🔹 VERSIONED API EXPLORER
// ==================================================
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";   // v1, v2, v3
    options.SubstituteApiVersionInUrl = true;
});

// ==================================================
// 🔹 AUTENTICACIÓN JWT
// ==================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
// 🔹 SWAGGER (GENÉRICO, LAS VERSIONES SE AGREGAN EN UI)
// ==================================================
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    // Soporte para JWT en Swagger (botón Authorize)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Bearer {token}\"",
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
            Array.Empty<string>()
        }
    });
});

// ==================================================
// 🔹 BUILD APP
// ==================================================
var app = builder.Build();

// ==================================================
// 🔹 MIDDLEWARE GLOBAL DE EXCEPCIONES
// ==================================================
app.UseMiddleware<GlobalExceptionMiddleware>();

// ==================================================
// 🔹 SWAGGER UI
// ==================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Librería API v1");
        options.RoutePrefix = string.Empty;
    });
}

// ==================================================
// 🔹 PIPELINE GENERAL
// ==================================================
app.UseHttpsRedirection();
app.UseAuthentication();   // ⬅ IMPORTANTE: antes de UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.Run();
