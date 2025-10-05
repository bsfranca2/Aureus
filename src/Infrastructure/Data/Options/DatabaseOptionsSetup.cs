using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Aureus.Infrastructure.Data.Options;

public class DatabaseOptionsSetup(IConfiguration configuration) : IConfigureOptions<DatabaseOptions>
{
    private const string ConfigurationSchemaName = "DatabaseOptions";
    
    public void Configure(DatabaseOptions options)
    {
        string? connectionString = configuration.GetConnectionString("Database");

        options.ConnectionString = connectionString ??
                                   throw new ArgumentException("Connection string cannot be null.", nameof(options));
        
        configuration.GetSection(ConfigurationSchemaName).Bind(options);
    }
}