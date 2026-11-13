using Microsoft.EntityFrameworkCore;
using Pup.Data.Comum;
using Pup.Domain.Acesso;

namespace Pup.Data.Acesso;

public sealed class AdministradorRepo(AppDbContext db) : RepoBase<Administrador>(db)
{
    public Task<Administrador?> ObterPorEmailAsync(string emailNormalizado, CancellationToken ct = default) =>
        Db.Administradores.FirstOrDefaultAsync(a => a.Email == emailNormalizado, ct);
}
