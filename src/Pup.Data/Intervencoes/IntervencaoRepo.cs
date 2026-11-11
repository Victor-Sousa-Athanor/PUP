using Microsoft.EntityFrameworkCore;
using Pup.Data.Comum;
using Pup.Domain.Intervencoes;

namespace Pup.Data.Intervencoes;

public sealed class IntervencaoRepo(AppDbContext db) : RepoBase<Intervencao>(db)
{
    public Task<Intervencao?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        Db.Intervencoes.Include(i => i.Historico).FirstOrDefaultAsync(i => i.Id == id, ct);
}
