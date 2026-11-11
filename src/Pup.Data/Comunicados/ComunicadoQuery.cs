using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pup.Contracts;
using Pup.Domain.Comum;
using Pup.Domain.Comunicados;
using Pup.Domain.Regioes;

namespace Pup.Data.Comunicados;

public sealed class ComunicadoQuery(AppDbContext db)
{
    /// <param name="regiaoId">Filtra pela região, mas sempre inclui os comunicados do distrito inteiro.</param>
    public async Task<IReadOnlyList<ComunicadoDto>> ListarAsync(CategoriaComunicado? categoria, Guid? regiaoId, bool somenteVigentes,
        bool somenteDestaques, DateOnly hoje, int limite = 100, CancellationToken ct = default)
    {
        var q = Juntar();
        if (categoria is { } c) q = q.Where(x => x.C.Categoria == c);
        if (regiaoId is { } r) q = q.Where(x => x.C.RegiaoId == null || x.C.RegiaoId == r);
        if (somenteVigentes) q = q.Where(x => x.C.ValidoAte == null || x.C.ValidoAte >= hoje);
        if (somenteDestaques) q = q.Where(x => x.C.Destaque);

        return await q
            .OrderByDescending(x => x.C.Destaque && (x.C.ValidoAte == null || x.C.ValidoAte >= hoje))
            .ThenByDescending(x => x.C.PublicadoEm)
            .Take(limite)
            .Select(Projetar(hoje))
            .ToListAsync(ct);
    }

    public Task<ComunicadoDto?> ObterAsync(Guid id, DateOnly hoje, CancellationToken ct = default) =>
        Juntar().Where(x => x.C.Id == id).Select(Projetar(hoje)).FirstOrDefaultAsync(ct);

    public Task<int> ContarVigentesAsync(DateOnly hoje, CancellationToken ct = default) =>
        db.Comunicados.CountAsync(c => c.ValidoAte == null || c.ValidoAte >= hoje, ct);

    private IQueryable<Linha> Juntar() =>
        from c in db.Comunicados.AsNoTracking()
        join r in db.Regioes.AsNoTracking() on c.RegiaoId equals (Guid?)r.Id into rs
        from r in rs.DefaultIfEmpty()
        select new Linha { C = c, R = r };

    private static Expression<Func<Linha, ComunicadoDto>> Projetar(DateOnly hoje) => x => new ComunicadoDto(
        x.C.Id, x.C.Titulo, x.C.Resumo, x.C.Conteudo, x.C.Categoria, x.C.RegiaoId,
        x.R == null ? null : x.R.Nome,
        x.C.PublicadoEm, x.C.ValidoAte, x.C.Destaque,
        x.C.ValidoAte == null || x.C.ValidoAte >= hoje,
        x.C.FonteOficialUrl, x.C.AtualizadoEm);

    private sealed class Linha
    {
        public Comunicado C { get; init; } = null!;
        public Regiao? R { get; init; }
    }
}
