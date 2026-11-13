using Microsoft.EntityFrameworkCore;
using Pup.Data.Comum;
using Pup.Domain.Regioes;

namespace Pup.Data.Regioes;

public sealed class RegiaoRepo(AppDbContext db) : RepoBase<Regiao>(db)
{
    public Task<Regiao?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        Db.Regioes.FirstOrDefaultAsync(r => r.Id == id, ct);
}
