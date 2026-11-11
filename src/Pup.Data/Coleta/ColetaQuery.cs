using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pup.Contracts;
using Pup.Domain.Coleta;
using Pup.Domain.Comum;
using Pup.Domain.Regioes;

namespace Pup.Data.Coleta;

/// <remarks>
/// Filtros e ordenação são aplicados sobre as entidades e a projeção para DTO fica por último:
/// o EF não consegue traduzir filtros feitos depois de um "new Record(...)".
/// </remarks>
public sealed class ColetaQuery(AppDbContext db)
{
    /// <param name="dia">Se informado, só coletas que passam neste dia.</param>
    public async Task<IReadOnlyList<ColetaDto>> ListarAsync(Guid? regiaoId, ModalidadeColeta? modalidade, DiasSemana? dia,
        DateOnly hoje, CancellationToken ct = default)
    {
        var q = Juntar();
        if (regiaoId is { } r) q = q.Where(x => x.C.RegiaoId == r);
        if (modalidade is { } m) q = q.Where(x => x.C.Modalidade == m);
        if (dia is { } d && d != DiasSemana.Nenhum)
        {
            var bits = (int)d;
            q = q.Where(x => ((int)x.C.Dias & bits) != 0);
        }

        return await q.OrderBy(x => x.R.Tipo).ThenBy(x => x.R.Nome).ThenBy(x => x.C.Modalidade)
            .Select(Projetar(hoje))
            .ToListAsync(ct);
    }

    public Task<ColetaDto?> ObterAsync(Guid id, DateOnly hoje, CancellationToken ct = default) =>
        Juntar().Where(x => x.C.Id == id).Select(Projetar(hoje)).FirstOrDefaultAsync(ct);

    /// <summary>Coletas com alteração ainda valendo hoje (contador da página inicial).</summary>
    public Task<int> ContarAlteradasAsync(DateOnly hoje, CancellationToken ct = default) =>
        db.Coletas.CountAsync(c => c.Situacao != SituacaoColeta.Regular &&
                                   (c.AlteracaoAte == null || c.AlteracaoAte >= hoje), ct);

    private IQueryable<Linha> Juntar() =>
        from c in db.Coletas.AsNoTracking()
        join r in db.Regioes.AsNoTracking() on c.RegiaoId equals r.Id
        select new Linha { C = c, R = r };

    // Mesma regra de ColetaResiduos.SituacaoEm, escrita aqui para o EF traduzir em SQL.
    private static Expression<Func<Linha, ColetaDto>> Projetar(DateOnly hoje) => x => new ColetaDto(
        x.C.Id, x.C.RegiaoId, x.R.Nome, x.R.Tipo, x.C.Modalidade, x.C.Dias, x.C.HorarioInicio, x.C.HorarioFim, x.C.Observacao,
        x.C.Situacao != SituacaoColeta.Regular && x.C.AlteracaoAte != null && x.C.AlteracaoAte < hoje
            ? SituacaoColeta.Regular
            : x.C.Situacao,
        x.C.Situacao != SituacaoColeta.Regular && x.C.AlteracaoAte != null && x.C.AlteracaoAte < hoje
            ? null
            : x.C.AvisoAlteracao,
        x.C.AlteracaoAte,
        x.C.FonteOficialUrl,
        x.C.AtualizadoEm);

    private sealed class Linha
    {
        public ColetaResiduos C { get; init; } = null!;
        public Regiao R { get; init; } = null!;
    }
}
