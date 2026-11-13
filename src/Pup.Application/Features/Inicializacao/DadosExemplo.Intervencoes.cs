using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Inicializacao;

public sealed partial class DadosExemplo
{
    private async Task SemearIntervencoesAsync(Guid centro, Guid zonaRural, Guid varadouro, DateOnly hoje, CancellationToken ct)
    {
        await intervencoes.CriarAsync(new CriarIntervencaoRequest(TipoIntervencao.Obra, "[Exemplo] Adequação de via",
            "Cascalhamento, nivelamento e melhoria da drenagem em trecho de terra.", varadouro, "Trecho de terra perto do km 2",
            StatusIntervencao.EmAndamento, hoje.AddDays(-20), hoje.AddDays(40),
            "Meia pista em alguns trechos durante o dia.", null), ct);

        await intervencoes.CriarAsync(new CriarIntervencaoRequest(TipoIntervencao.Obra, "[Exemplo] Melhoria de drenagem",
            "Implantação e limpeza de galerias e bueiros em pontos críticos.", zonaRural, null,
            StatusIntervencao.Programado, hoje.AddDays(10), hoje.AddDays(60), null, null), ct);

        await intervencoes.CriarAsync(new CriarIntervencaoRequest(TipoIntervencao.Obra, "[Exemplo] Revitalização da praça central",
            "Troca de piso, bancos e paisagismo.", centro, "Praça central",
            StatusIntervencao.Programado, hoje.AddDays(15), hoje.AddDays(90), null, null), ct);

        await intervencoes.CriarAsync(new CriarIntervencaoRequest(TipoIntervencao.Manutencao, "[Exemplo] Manutenção de iluminação",
            "Troca de lâmpadas queimadas nas ruas do centro.", centro, null,
            StatusIntervencao.Programado, hoje.AddDays(5), hoje.AddDays(7), null, null), ct);

        await intervencoes.CriarAsync(new CriarIntervencaoRequest(TipoIntervencao.Manutencao, "[Exemplo] Desobstrução de drenagem",
            "Limpeza de bueiros e canaletas para escoamento de águas pluviais.", varadouro, null,
            StatusIntervencao.EmAndamento, hoje.AddDays(-3), hoje.AddDays(4), "Possíveis bloqueios rápidos.", null), ct);

        var nivId = await intervencoes.CriarAsync(new CriarIntervencaoRequest(TipoIntervencao.Manutencao,
            "[Exemplo] Nivelamento de estrada", "Patrolamento e recomposição de cascalho após chuvas.", zonaRural, "Estradas rurais",
            StatusIntervencao.EmAndamento, hoje.AddDays(-10), hoje.AddDays(-2), null, null), ct);
        await intervencoes.MudarStatusAsync(nivId,
            new MudarStatusRequest(StatusIntervencao.Concluido, "[Exemplo] Serviço concluído e via liberada."), ct);
    }
}
