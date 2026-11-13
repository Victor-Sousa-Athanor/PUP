using Microsoft.EntityFrameworkCore;
using Pup.Data.Comum;
using Pup.Domain.Paginas;

namespace Pup.Data.Paginas;

public sealed class PaginaRepo(AppDbContext db) : RepoBase<Pagina>(db)
{
    public Task<Pagina?> ObterPorIdAsync(Guid id, CancellationToken ct) =>
        db.Paginas.FirstOrDefaultAsync(x => x.Id == id, ct);
}
