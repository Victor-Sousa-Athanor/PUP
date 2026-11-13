using Microsoft.EntityFrameworkCore;
using Pup.Domain.Coleta;
using Pup.Domain.Comum;

namespace Pup.Data.PontosDescarte;

public sealed class PontoDescarteQuery(AppDbContext db)
{
    public async Task<IReadOnlyList<PontoDescarte>> ListarAsync(Guid? regiaoId, TipoPontoDescarte? tipo,
        bool incluirInativos, CancellationToken ct = default)
    {
        var q = db.PontosDescarte.AsNoTracking();
        if (!incluirInativos) q = q.Where(p => p.Ativo);
        if (regiaoId is { } r) q = q.Where(p => p.RegiaoId == r);
        if (tipo is { } t) q = q.Where(p => p.Tipo == t);
        return await q.OrderBy(p => p.Nome).ToListAsync(ct);
    }

    public Task<PontoDescarte?> ObterAsync(Guid id, CancellationToken ct = default) =>
        db.PontosDescarte.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
}
