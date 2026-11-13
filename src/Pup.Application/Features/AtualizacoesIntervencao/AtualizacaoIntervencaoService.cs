using Pup.Contracts;
using Pup.Data.AtualizacoesIntervencao;

namespace Pup.Application.Features.AtualizacoesIntervencao;

public sealed class AtualizacaoIntervencaoService(
    AtualizacaoIntervencaoRepo repo,
    AtualizacaoIntervencaoQuery query)
{
    // Repo injetado para manter o par Repo/Query padrão da entidade e eventuais registros diretos.
    private readonly AtualizacaoIntervencaoRepo _repo = repo;

    public Task<IReadOnlyList<AtualizacaoIntervencaoDto>> ListarPorIntervencaoAsync(Guid intervencaoId,
        CancellationToken ct = default) =>
        query.ListarPorIntervencaoAsync(intervencaoId, ct);
}
