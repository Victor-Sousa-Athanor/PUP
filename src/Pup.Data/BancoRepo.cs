using Microsoft.EntityFrameworkCore;

namespace Pup.Data;

/// <summary>Criação/atualização do schema na subida da API (fica aqui para só o projeto Data tocar no DbContext).</summary>
public sealed class BancoRepo(AppDbContext db)
{
    public async Task PrepararAsync(CancellationToken ct = default)
    {
        // Com migrations criadas (dotnet ef migrations add Inicial), aplica-as; sem migrations, cria o schema direto (dev).
        if (db.Database.GetMigrations().Any())
            await db.Database.MigrateAsync(ct);
        else
            await db.Database.EnsureCreatedAsync(ct);
    }

    public Task<bool> TemRegioesAsync(CancellationToken ct = default) => db.Regioes.AnyAsync(ct);
}
