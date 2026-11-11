using Microsoft.EntityFrameworkCore;
using Pup.Data.Comum;
using Pup.Domain.Coleta;

namespace Pup.Data.Coleta;

public sealed class ColetaRepo(AppDbContext db) : RepoBase<ColetaResiduos>(db)
{
    public Task<ColetaResiduos?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        Db.Coletas.FirstOrDefaultAsync(c => c.Id == id, ct);
}
