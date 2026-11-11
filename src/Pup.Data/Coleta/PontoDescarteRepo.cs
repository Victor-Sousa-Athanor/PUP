using Microsoft.EntityFrameworkCore;
using Pup.Data.Comum;
using Pup.Domain.Coleta;

namespace Pup.Data.Coleta;

public sealed class PontoDescarteRepo(AppDbContext db) : RepoBase<PontoDescarte>(db)
{
    public Task<PontoDescarte?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        Db.PontosDescarte.FirstOrDefaultAsync(p => p.Id == id, ct);
}
