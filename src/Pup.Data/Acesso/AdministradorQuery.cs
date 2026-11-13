using Microsoft.EntityFrameworkCore;

namespace Pup.Data.Acesso;

public sealed class AdministradorQuery(AppDbContext db)
{
    public Task<bool> ExisteAlgumAsync(CancellationToken ct = default) =>
        db.Administradores.AsNoTracking().AnyAsync(ct);
}
