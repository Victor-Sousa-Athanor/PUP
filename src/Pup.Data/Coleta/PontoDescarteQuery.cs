using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pup.Contracts;
using Pup.Domain.Coleta;
using Pup.Domain.Comum;
using Pup.Domain.Regioes;

namespace Pup.Data.Coleta;

public sealed class PontoDescarteQuery(AppDbContext db)
{
    public async Task<IReadOnlyList<PontoDescarteDto>> ListarAsync(Guid? regiaoId, TipoPontoDescarte? tipo, bool incluirInativos,
        CancellationToken ct = default)
    {
        var q = Juntar();
        if (!incluirInativos) q = q.Where(x => x.P.Ativo);
        if (regiaoId is { } r) q = q.Where(x => x.P.RegiaoId == r);
        if (tipo is { } t) q = q.Where(x => x.P.Tipo == t);
        return await q.OrderBy(x => x.R.Nome).ThenBy(x => x.P.Nome).Select(Projecao).ToListAsync(ct);
    }

    public Task<PontoDescarteDto?> ObterAsync(Guid id, CancellationToken ct = default) =>
        Juntar().Where(x => x.P.Id == id).Select(Projecao).FirstOrDefaultAsync(ct);

    private IQueryable<Linha> Juntar() =>
        from p in db.PontosDescarte.AsNoTracking()
        join r in db.Regioes.AsNoTracking() on p.RegiaoId equals r.Id
        select new Linha { P = p, R = r };

    private static readonly Expression<Func<Linha, PontoDescarteDto>> Projecao = x => new PontoDescarteDto(
        x.P.Id, x.P.Nome, x.P.Tipo, x.P.RegiaoId, x.R.Nome, x.P.Referencia, x.P.Latitude, x.P.Longitude,
        x.P.Orientacoes, x.P.Ativo, x.P.FonteOficialUrl, x.P.AtualizadoEm);

    private sealed class Linha
    {
        public PontoDescarte P { get; init; } = null!;
        public Regiao R { get; init; } = null!;
    }
}
