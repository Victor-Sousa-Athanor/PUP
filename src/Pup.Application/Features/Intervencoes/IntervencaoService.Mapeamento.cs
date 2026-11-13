using Pup.Application.Features.Regioes;
using Pup.Contracts;
using Pup.Domain.Comum;
using Pup.Domain.Intervencoes;

namespace Pup.Application.Features.Intervencoes;

public sealed partial class IntervencaoService
{
    private static bool EstaAberta(StatusIntervencao s) =>
        s != StatusIntervencao.Concluido && s != StatusIntervencao.Cancelado;

    private static int Prioridade(StatusIntervencao s) => s switch
    {
        StatusIntervencao.EmAndamento => 0,
        StatusIntervencao.TemporariamenteAlterado => 1,
        StatusIntervencao.Programado => 2,
        _ => 3
    };

    private static bool ContemTermo(IntervencaoResumoDto i, string termo) =>
        i.Titulo.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
        i.Descricao.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
        (i.Local?.Contains(termo, StringComparison.OrdinalIgnoreCase) ?? false) ||
        i.RegiaoNome.Contains(termo, StringComparison.OrdinalIgnoreCase);

    private static IntervencaoResumoDto MapearResumo(Intervencao i, IReadOnlyDictionary<Guid, RegiaoDto> mapa, DateOnly hoje)
    {
        var regiaoNome = mapa.TryGetValue(i.RegiaoId, out var r) ? r.Nome : "—";
        return new IntervencaoResumoDto(
            i.Id, i.Tipo, i.Titulo, i.Descricao, i.RegiaoId, regiaoNome, i.Local, i.Status,
            i.DataInicio, i.PrevisaoConclusao, i.DataConclusao, i.ImpactoNaVia, i.Atrasada(hoje), i.AtualizadoEm);
    }
}
