using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Data.Regioes;
using Pup.Domain.Comum;
using Pup.Domain.Regioes;

namespace Pup.Application.Features.Regioes;

public sealed class RegiaoService(RegiaoRepo repo, RegiaoQuery query)
{
    public Task<IReadOnlyList<RegiaoDto>> ListarAsync(bool incluirInativas, CancellationToken ct = default) =>
        query.ListarAsync(incluirInativas, ct);

    public async Task<RegiaoDto> ObterAsync(Guid id, CancellationToken ct = default) =>
        await query.ObterAsync(id, ct) ?? throw new NaoEncontradoException("Região não encontrada.");

    public async Task<IReadOnlyDictionary<Guid, RegiaoDto>> ObterMapaAsync(CancellationToken ct = default) =>
        (await query.ListarAsync(incluirInativas: true, ct)).ToDictionary(r => r.Id);

    public Task<bool> ExisteAlgumaAsync(CancellationToken ct = default) =>
        query.ExisteAlgumaAsync(ct);

    public async Task GarantirExisteAsync(Guid regiaoId, CancellationToken ct = default)
    {
        if (regiaoId == Guid.Empty || !await query.ExisteAsync(regiaoId, ct))
            throw new DomainException("Região não encontrada. Selecione uma região da lista.");
    }

    public async Task<Guid> CriarAsync(SalvarRegiaoRequest r, CancellationToken ct = default)
    {
        await GarantirNomeLivreAsync(r.Nome, null, ct);
        var regiao = Regiao.Criar(r.Nome, r.Tipo, r.Descricao);
        repo.Adicionar(regiao);
        await repo.SalvarAsync(ct);
        return regiao.Id;
    }

    /// <summary>Regiões não são apagadas (têm vínculos): desative com Ativa = false.</summary>
    public async Task AtualizarAsync(Guid id, SalvarRegiaoRequest r, CancellationToken ct = default)
    {
        var regiao = await repo.ObterPorIdAsync(id, ct) ?? throw new NaoEncontradoException("Região não encontrada.");
        await GarantirNomeLivreAsync(r.Nome, id, ct);
        regiao.Atualizar(r.Nome, r.Tipo, r.Descricao, r.Ativa);
        await repo.SalvarAsync(ct);
    }

    private async Task GarantirNomeLivreAsync(string? nome, Guid? ignorarId, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(nome) && await query.NomeEmUsoAsync(nome, ignorarId, ct))
            throw new DomainException("Já existe uma região com esse nome.");
    }
}
