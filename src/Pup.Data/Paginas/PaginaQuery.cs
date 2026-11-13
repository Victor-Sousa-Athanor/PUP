using Microsoft.EntityFrameworkCore;
using Pup.Domain.Paginas;

namespace Pup.Data.Paginas;

public sealed class PaginaQuery(AppDbContext db)
{
    public async Task<IReadOnlyList<Pagina>> ListarAsync(bool apenasPublicadas, CancellationToken ct)
    {
        var q = db.Paginas.AsNoTracking();
        if (apenasPublicadas) q = q.Where(x => x.Publicada);
        return await q.OrderBy(x => x.Ordem).ThenBy(x => x.Titulo).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Pagina>> ListarMenuAsync(CancellationToken ct) =>
        await db.Paginas.AsNoTracking()
            .Where(x => x.Publicada && x.ExibirNoMenu)
            .OrderBy(x => x.Ordem)
            .ThenBy(x => x.Titulo)
            .ToListAsync(ct);

    public Task<Pagina?> ObterPorIdAsync(Guid id, CancellationToken ct) =>
        db.Paginas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Pagina?> ObterPorRotaAsync(string rota, CancellationToken ct) =>
        db.Paginas.AsNoTracking().FirstOrDefaultAsync(x => x.Rota == rota, ct);

    public Task<bool> ExisteAlgumaAsync(CancellationToken ct) =>
        db.Paginas.AnyAsync(ct);

    public Task<bool> RotaEmUsoAsync(string rota, Guid? ignorarId, CancellationToken ct) =>
        db.Paginas.AnyAsync(x => x.Rota == rota && (!ignorarId.HasValue || x.Id != ignorarId.Value), ct);
}
