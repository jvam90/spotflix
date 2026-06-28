using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Spotflix.Api.Data;

/// <summary>
/// Usado pelas ferramentas do EF Core (dotnet ef) em tempo de design, evitando
/// inicializar a aplicação inteira (e o seeder) ao gerar/aplicar migrations.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connStr = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' não configurada. " +
                "Copie appsettings.Development.json.example para appsettings.Development.json e preencha os valores.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connStr)
            .Options;

        return new AppDbContext(options);
    }
}
