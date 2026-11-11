using Microsoft.EntityFrameworkCore;
using Pup.Data.Comum;
using Pup.Domain.Comunicados;

namespace Pup.Data.Comunicados;

public sealed class ComunicadoRepo(AppDbContext db) : RepoBase<Comunicado>(db)
{
    public Task<Comunicado?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        Db.Comunicados.FirstOrDefaultAsync(c => c.Id == id, ct);
}
