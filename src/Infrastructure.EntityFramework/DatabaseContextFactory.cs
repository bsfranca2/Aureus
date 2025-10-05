using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aureus.Infrastructure.EntityFramework;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<DatabaseContext> optionsBuilder = new();

        optionsBuilder
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseNpgsql("Server=localhost;Port=5432;Database=AurumPayDev;User Id=postgres;Password=postgres");

        return new DatabaseContext(optionsBuilder.Options);
    }
}