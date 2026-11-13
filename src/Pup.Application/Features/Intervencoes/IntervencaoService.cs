using Pup.Application.Comum;
using Pup.Application.Features.AtualizacoesIntervencao;
using Pup.Application.Features.Regioes;
using Pup.Contracts;
using Pup.Data.Intervencoes;
using Pup.Domain.Comum;
using Pup.Domain.Intervencoes;

namespace Pup.Application.Features.Intervencoes;

/// <summary>Obras e serviços de manutenção (mesma entidade, separadas pelo campo Tipo).</summary>
public sealed partial class IntervencaoService(
    IntervencaoRepo repo,
    IntervencaoQuery query,
    RegiaoService regioes,
    AtualizacaoIntervencaoService atualizacoes,
    IRelogio relogio)
{
    public async Task<IReadOnlyList<IntervencaoResumoDto>> ListarAsync(FiltroIntervencoes f, CancellationToken ct = default)
    {
        var itens = await query.ListarAsync(f.Tipo, f.Status, f.RegiaoId, ct);
        var mapaRegioes = await regioes.ObterMapaAsync(ct);
        var hoje = relogio.Hoje;

        IEnumerable<IntervencaoResumoDto> resultado = itens
            .Where(i => !f.SomenteAbertas || EstaAberta(i.Status))
            .Select(i => MapearResumo(i, mapaRegioes, hoje));

        if (!string.IsNullOrWhiteSpace(f.Termo))
        {
            var termo = f.Termo.Trim();
            resultado = resultado.Where(x => ContemTermo(x, termo));
        }

        return resultado
            .OrderBy(x => Prioridade(x.Status))
            .ThenByDescending(x => x.AtualizadoEm)
            .ToList();
    }

    public async Task<IntervencaoDetalheDto> ObterAsync(Guid id, CancellationToken ct = default)
    {
        var i = await query.ObterAsync(id, ct) ?? throw NaoEncontrada();
        var regiao = await regioes.ObterAsync(i.RegiaoId, ct);
        var historico = await atualizacoes.ListarPorIntervencaoAsync(id, ct);

        return new IntervencaoDetalheDto(
            i.Id, i.Tipo, i.Titulo, i.Descricao, i.RegiaoId, regiao.Nome, i.Local, i.Status,
            i.DataInicio, i.PrevisaoConclusao, i.DataConclusao, i.ImpactoNaVia, i.Atrasada(relogio.Hoje),
            i.FonteOficialUrl, i.CriadoEm, i.AtualizadoEm, historico);
    }

    public async Task<int> ContarAsync(TipoIntervencao tipo, bool somenteEmAndamento, CancellationToken ct = default)
    {
        var status = somenteEmAndamento ? StatusIntervencao.EmAndamento : (StatusIntervencao?)null;
        var itens = await query.ListarAsync(tipo, status, null, ct);
        return somenteEmAndamento ? itens.Count : itens.Count(i => EstaAberta(i.Status));
    }

    public async Task<Guid> CriarAsync(CriarIntervencaoRequest r, CancellationToken ct = default)
    {
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        var i = Intervencao.Criar(r.Tipo, r.Titulo, r.Descricao, r.RegiaoId, r.Local, r.Status, r.DataInicio,
            r.PrevisaoConclusao, r.ImpactoNaVia, r.FonteOficialUrl);
        repo.Adicionar(i);
        await repo.SalvarAsync(ct);
        return i.Id;
    }

    public async Task AtualizarAsync(Guid id, AtualizarIntervencaoRequest r, CancellationToken ct = default)
    {
        var i = await CarregarAsync(id, ct);
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        i.AtualizarDados(r.Titulo, r.Descricao, r.RegiaoId, r.Local, r.DataInicio, r.PrevisaoConclusao, r.ImpactoNaVia, r.FonteOficialUrl);
        await repo.SalvarAsync(ct);
    }

    public async Task<Guid> MudarStatusAsync(Guid id, MudarStatusRequest r, CancellationToken ct = default)
    {
        var i = await CarregarAsync(id, ct);
        var atualizacao = i.MudarStatus(r.Status, r.Nota, relogio.Hoje);
        await repo.SalvarAsync(ct);
        return atualizacao.Id;
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        repo.Remover(await CarregarAsync(id, ct));
        await repo.SalvarAsync(ct);
    }

    private async Task<Intervencao> CarregarAsync(Guid id, CancellationToken ct) =>
        await repo.ObterPorIdAsync(id, ct) ?? throw NaoEncontrada();

    private static NaoEncontradoException NaoEncontrada() => new("Obra ou manutenção não encontrada.");
}
