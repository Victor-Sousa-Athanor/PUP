using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pup.Data;

/// <summary>Permite rodar `dotnet ef migrations add X -p src/Pup.Data` sem subir a API.</summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("PUP_CONNECTION")
                 ?? "Host=localhost;Port=5433;Database=pup;Username=pup;Password=pup";
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(cs).Options;
        return new AppDbContext(options);
    }
}
