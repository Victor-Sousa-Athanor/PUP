using System.Globalization;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Painel;

public sealed partial class PainelService
{
    private async Task<List<ItemAtualizacaoDto>> MontarItensAsync(bool incluirVencidos, CancellationToken ct)
    {
        var itens = new List<ItemAtualizacaoDto>();

        var listaColetas = await coletas.ListarAsync(null, null, null, false, ct);
        itens.AddRange(listaColetas.Select(c => new ItemAtualizacaoDto(
            CategoriaServico.Coleta, c.Id, $"{c.Modalidade.Nome()} — {c.RegiaoNome}",
            c.Situacao != SituacaoColeta.Regular ? c.AvisoAlteracao : $"Dias: {c.Dias.Nome()}.",
            c.Situacao.Nome(), c.AtualizadoEm)));

        var listaPontos = await pontos.ListarAsync(null, null, false, ct);
        itens.AddRange(listaPontos.Select(p => new ItemAtualizacaoDto(
            CategoriaServico.PontoDescarte, p.Id, $"{p.Tipo.Nome()} — {p.Nome}",
            $"{p.Referencia} ({p.RegiaoNome})", null, p.AtualizadoEm)));

        var listaIntervencoes = await intervencoes.ListarAsync(new FiltroIntervencoes(), ct);
        itens.AddRange(listaIntervencoes.Select(i => new ItemAtualizacaoDto(
            i.Tipo == TipoIntervencao.Obra ? CategoriaServico.Obra : CategoriaServico.Manutencao,
            i.Id, i.Titulo, ResumoIntervencao(i), i.Status.Nome(), i.AtualizadoEm)));

        var listaComunicados = await comunicados.ListarAsync(null, null, !incluirVencidos, false, ct);
        itens.AddRange(listaComunicados.Select(c => new ItemAtualizacaoDto(
            CategoriaServico.Comunicado, c.Id, c.Titulo, c.Resumo, c.Categoria.Nome(), c.AtualizadoEm)));

        return itens;
    }

    private static string ResumoIntervencao(IntervencaoResumoDto i)
    {
        var local = i.Local ?? i.RegiaoNome;
        return i.PrevisaoConclusao is { } p
            ? $"{local} · previsão {p.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}"
            : local;
    }

    private static bool Contem(ItemAtualizacaoDto item, string termo) =>
        item.Titulo.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
        (item.Descricao?.Contains(termo, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (item.Situacao?.Contains(termo, StringComparison.OrdinalIgnoreCase) ?? false);
}
