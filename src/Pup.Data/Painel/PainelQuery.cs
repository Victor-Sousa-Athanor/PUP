using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Pup.Contracts;
using Pup.Data.Comum;
using Pup.Domain.Comum;

namespace Pup.Data.Painel;

/// <summary>
/// Consultas que atravessam várias categorias: "Últimas atualizações" da página inicial e a busca do topo.
/// Cada categoria é consultada separadamente (consultas pequenas) e o resultado é unido em memória.
/// </summary>
public sealed class PainelQuery(AppDbContext db)
{
    public Task<IReadOnlyList<ItemAtualizacaoDto>> UltimasAtualizacoesAsync(int limite, DateOnly hoje, CancellationToken ct = default) =>
        MontarAsync(termo: null, limite, hoje, ct);

    public Task<IReadOnlyList<ItemAtualizacaoDto>> BuscarAsync(string termo, int limite, DateOnly hoje, CancellationToken ct = default) =>
        MontarAsync(termo, limite, hoje, ct);

    private async Task<IReadOnlyList<ItemAtualizacaoDto>> MontarAsync(string? termo, int limite, DateOnly hoje, CancellationToken ct)
    {
        var padrao = string.IsNullOrWhiteSpace(termo) ? null : BuscaTexto.Padrao(termo);
        var itens = new List<ItemAtualizacaoDto>();

        // Coletas: o "título" é a região + modalidade (o que o morador procura: "coleta no Centro").
        var coletas = await (
                from c in db.Coletas.AsNoTracking()
                join r in db.Regioes.AsNoTracking() on c.RegiaoId equals r.Id
                where padrao == null || EF.Functions.ILike(r.Nome, padrao) ||
                      (c.Observacao != null && EF.Functions.ILike(c.Observacao, padrao)) ||
                      (c.AvisoAlteracao != null && EF.Functions.ILike(c.AvisoAlteracao, padrao))
                orderby c.AtualizadoEm descending
                select new { c.Id, Regiao = r.Nome, c.Modalidade, c.Dias, c.Situacao, c.AlteracaoAte, c.AvisoAlteracao, c.AtualizadoEm })
            .Take(limite).ToListAsync(ct);
        foreach (var c in coletas)
        {
            var alterada = c.Situacao != SituacaoColeta.Regular && (c.AlteracaoAte is null || c.AlteracaoAte >= hoje);
            itens.Add(new ItemAtualizacaoDto(CategoriaServico.Coleta, c.Id, $"{c.Modalidade.Nome()} — {c.Regiao}",
                alterada ? c.AvisoAlteracao : $"Dias: {c.Dias.Nome()}.",
                (alterada ? c.Situacao : SituacaoColeta.Regular).Nome(), c.AtualizadoEm));
        }

        var pontos = await (
                from p in db.PontosDescarte.AsNoTracking()
                join r in db.Regioes.AsNoTracking() on p.RegiaoId equals r.Id
                where p.Ativo && (padrao == null || EF.Functions.ILike(p.Nome, padrao) ||
                                  EF.Functions.ILike(p.Referencia, padrao) || EF.Functions.ILike(r.Nome, padrao))
                orderby p.AtualizadoEm descending
                select new { p.Id, p.Nome, p.Tipo, p.Referencia, Regiao = r.Nome, p.AtualizadoEm })
            .Take(limite).ToListAsync(ct);
        itens.AddRange(pontos.Select(p => new ItemAtualizacaoDto(CategoriaServico.PontoDescarte, p.Id,
            $"{p.Tipo.Nome()} — {p.Nome}", $"{p.Referencia} ({p.Regiao})", null, p.AtualizadoEm)));

        var intervencoes = await (
                from i in db.Intervencoes.AsNoTracking()
                join r in db.Regioes.AsNoTracking() on i.RegiaoId equals r.Id
                where padrao == null || EF.Functions.ILike(i.Titulo, padrao) || EF.Functions.ILike(i.Descricao, padrao) ||
                      (i.Local != null && EF.Functions.ILike(i.Local, padrao)) || EF.Functions.ILike(r.Nome, padrao)
                orderby i.AtualizadoEm descending
                select new { i.Id, i.Tipo, i.Titulo, i.Status, i.Local, Regiao = r.Nome, i.PrevisaoConclusao, i.AtualizadoEm })
            .Take(limite).ToListAsync(ct);
        itens.AddRange(intervencoes.Select(i => new ItemAtualizacaoDto(
            i.Tipo == TipoIntervencao.Obra ? CategoriaServico.Obra : CategoriaServico.Manutencao,
            i.Id, i.Titulo,
            (i.Local ?? i.Regiao) + (i.PrevisaoConclusao is { } p ? " · previsão " + p.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : string.Empty),
            i.Status.Nome(), i.AtualizadoEm)));

        var comunicados = await db.Comunicados.AsNoTracking()
            .Where(c => padrao == null || EF.Functions.ILike(c.Titulo, padrao) || EF.Functions.ILike(c.Resumo, padrao) ||
                        EF.Functions.ILike(c.Conteudo, padrao))
            // Na página inicial só entram comunicados vigentes; na busca, todos.
            .Where(c => padrao != null || c.ValidoAte == null || c.ValidoAte >= hoje)
            .OrderByDescending(c => c.AtualizadoEm)
            .Select(c => new { c.Id, c.Titulo, c.Resumo, c.Categoria, c.AtualizadoEm })
            .Take(limite).ToListAsync(ct);
        itens.AddRange(comunicados.Select(c => new ItemAtualizacaoDto(CategoriaServico.Comunicado, c.Id, c.Titulo, c.Resumo,
            c.Categoria.Nome(), c.AtualizadoEm)));

        return itens.OrderByDescending(i => i.AtualizadoEm).Take(limite).ToList();
    }
}
