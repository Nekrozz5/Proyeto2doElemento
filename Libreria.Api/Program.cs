using Libreria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libreria.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // ✅ Inyectar ApplicationDbContext con MySQL
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)),
        b => b.MigrationsAssembly("Libreria.Api") // 👈 le digo dónde guardar migraciones
    )
);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
