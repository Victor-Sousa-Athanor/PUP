using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pup.Contracts;
using Pup.Data.Comum;
using Pup.Domain.Comum;
using Pup.Domain.Intervencoes;
using Pup.Domain.Regioes;

namespace Pup.Data.Intervencoes;

public sealed class IntervencaoQuery(AppDbContext db)
{
    public async Task<IReadOnlyList<IntervencaoResumoDto>> ListarAsync(FiltroIntervencoes f, DateOnly hoje, int limite = 200,
        CancellationToken ct = default)
    {
        var q = Juntar();
        if (f.Tipo is { } tipo) q = q.Where(x => x.I.Tipo == tipo);
        if (f.Status is { } status) q = q.Where(x => x.I.Status == status);
        if (f.RegiaoId is { } regiao) q = q.Where(x => x.I.RegiaoId == regiao);
        if (f.SomenteAbertas)
            q = q.Where(x => x.I.Status != StatusIntervencao.Concluido && x.I.Status != StatusIntervencao.Cancelado);
        if (!string.IsNullOrWhiteSpace(f.Termo))
        {
            var padrao = BuscaTexto.Padrao(f.Termo);
            q = q.Where(x => EF.Functions.ILike(x.I.Titulo, padrao) ||
                             (x.I.Local != null && EF.Functions.ILike(x.I.Local, padrao)) ||
                             EF.Functions.ILike(x.R.Nome, padrao));
        }

        // Abertas primeiro (andamento > alterado > programado), depois as mais recentes.
        return await q
            .OrderBy(x => x.I.Status == StatusIntervencao.EmAndamento ? 0
                : x.I.Status == StatusIntervencao.TemporariamenteAlterado ? 1
                : x.I.Status == StatusIntervencao.Programado ? 2 : 3)
            .ThenByDescending(x => x.I.AtualizadoEm)
            .Take(limite)
            .Select(Projetar(hoje))
            .ToListAsync(ct);
    }

    public async Task<IntervencaoDetalheDto?> ObterAsync(Guid id, DateOnly hoje, CancellationToken ct = default)
    {
        var linha = await Juntar().Where(x => x.I.Id == id).FirstOrDefaultAsync(ct);
        if (linha is null) return null;

        var historico = await db.AtualizacoesIntervencao.AsNoTracking()
            .Where(a => a.IntervencaoId == id)
            .OrderByDescending(a => a.Data)
            .Select(a => new AtualizacaoIntervencaoDto(a.Id, a.Status, a.Nota, a.Data))
            .ToListAsync(ct);

        var i = linha.I;
        return new IntervencaoDetalheDto(i.Id, i.Tipo, i.Titulo, i.Descricao, i.RegiaoId, linha.R.Nome, i.Local, i.Status,
            i.DataInicio, i.PrevisaoConclusao, i.DataConclusao, i.ImpactoNaVia, i.Atrasada(hoje), i.FonteOficialUrl,
            i.CriadoEm, i.AtualizadoEm, historico);
    }

    public Task<int> ContarAsync(TipoIntervencao tipo, bool somenteEmAndamento, CancellationToken ct = default) =>
        somenteEmAndamento
            ? db.Intervencoes.CountAsync(i => i.Tipo == tipo && i.Status == StatusIntervencao.EmAndamento, ct)
            : db.Intervencoes.CountAsync(i => i.Tipo == tipo &&
                                              i.Status != StatusIntervencao.Concluido &&
                                              i.Status != StatusIntervencao.Cancelado, ct);

    private IQueryable<Linha> Juntar() =>
        from i in db.Intervencoes.AsNoTracking()
        join r in db.Regioes.AsNoTracking() on i.RegiaoId equals r.Id
        select new Linha { I = i, R = r };

    // Mesma regra de Intervencao.Atrasada, escrita aqui para o EF traduzir em SQL.
    private static Expression<Func<Linha, IntervencaoResumoDto>> Projetar(DateOnly hoje) => x => new IntervencaoResumoDto(
        x.I.Id, x.I.Tipo, x.I.Titulo, x.I.Descricao, x.I.RegiaoId, x.R.Nome, x.I.Local, x.I.Status, x.I.DataInicio,
        x.I.PrevisaoConclusao, x.I.DataConclusao, x.I.ImpactoNaVia,
        x.I.PrevisaoConclusao != null && x.I.PrevisaoConclusao < hoje &&
        (x.I.Status == StatusIntervencao.Programado || x.I.Status == StatusIntervencao.EmAndamento ||
         x.I.Status == StatusIntervencao.TemporariamenteAlterado),
        x.I.AtualizadoEm);

    private sealed class Linha
    {
        public Intervencao I { get; init; } = null!;
        public Regiao R { get; init; } = null!;
    }
}
