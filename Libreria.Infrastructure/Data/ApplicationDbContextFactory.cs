using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Libreria.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var conn =
                "Server=libreria-server.mysql.database.azure.com;Port=3306;Database=LibreriaDb;Uid=jairo;Pwd=8794236Jm;SslMode=Required;";

            optionsBuilder.UseMySql(conn, new MySqlServerVersion(new Version(8, 0, 36)));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
