using Microsoft.EntityFrameworkCore;

namespace Pup.Data;

/// <summary>Criação e migração de schema na subida da API (sem acesso a entidades específicas).</summary>
public sealed class BancoRepo(AppDbContext db)
{
    public async Task PrepararAsync(CancellationToken ct = default)
    {
        if (db.Database.GetMigrations().Any())
            await db.Database.MigrateAsync(ct);
        else
            await db.Database.EnsureCreatedAsync(ct);
    }
}
