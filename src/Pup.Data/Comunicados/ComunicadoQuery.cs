using Microsoft.EntityFrameworkCore;
using Pup.Domain.Comum;
using Pup.Domain.Comunicados;

namespace Pup.Data.Comunicados;

public sealed class ComunicadoQuery(AppDbContext db)
{
    public async Task<IReadOnlyList<Comunicado>> ListarAsync(CategoriaComunicado? categoria,
        bool somenteDestaques, CancellationToken ct = default)
    {
        var q = db.Comunicados.AsNoTracking();
        if (categoria is { } c) q = q.Where(x => x.Categoria == c);
        if (somenteDestaques) q = q.Where(x => x.Destaque);
        return await q.OrderByDescending(x => x.PublicadoEm).ToListAsync(ct);
    }

    public Task<Comunicado?> ObterAsync(Guid id, CancellationToken ct = default) =>
        db.Comunicados.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
}
