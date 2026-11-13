using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Inicializacao;

public sealed partial class DadosExemplo
{
    private async Task SemearColetaEPontosAsync(Guid centro, Guid zonaRural, Guid varadouro, DateOnly hoje, CancellationToken ct)
    {
        var inicio = new TimeOnly(7, 0);
        var fim = new TimeOnly(15, 20);
        const DiasSemana terQuiSab = DiasSemana.Terca | DiasSemana.Quinta | DiasSemana.Sabado;

        await coletas.CriarAsync(new SalvarColetaRequest(centro, ModalidadeColeta.Comum, terQuiSab, inicio, fim,
            "Confirme dias e horários atualizados nos canais oficiais da Prefeitura/Urbam.", NoticiaColeta2023), ct);
        await coletas.CriarAsync(new SalvarColetaRequest(centro, ModalidadeColeta.Seletiva, terQuiSab, inicio, fim,
            "Separe papel, plástico, metal e vidro limpos. Horário de referência; confirme nos canais oficiais.", NoticiaColeta2024), ct);
        await coletas.CriarAsync(new SalvarColetaRequest(zonaRural, ModalidadeColeta.Seletiva, DiasSemana.Quarta,
            new TimeOnly(8, 0), new TimeOnly(14, 0),
            "[Exemplo] Recolhimento nos abrigos ecológicos (lixeiras de comum e seletiva lado a lado).", NoticiaColeta2024), ct);

        var idVaradouro = await coletas.CriarAsync(new SalvarColetaRequest(varadouro, ModalidadeColeta.Comum,
            DiasSemana.Terca | DiasSemana.Sabado, new TimeOnly(8, 0), new TimeOnly(12, 0),
            "[Exemplo] Coleta nos abrigos ecológicos ao longo da estrada.", null), ct);
        await coletas.AlterarTemporariamenteAsync(idVaradouro, new AlterarColetaRequest(
            SituacaoColeta.TemporariamenteAlterada,
            "[Exemplo] Por causa das chuvas, a coleta desta semana passa para quarta-feira.", hoje.AddDays(7)), ct);

        await pontos.CriarAsync(new SalvarPontoDescarteRequest("[Exemplo] Abrigo ecológico do Varadouro",
            TipoPontoDescarte.AbrigoEcologico, varadouro, "[Exemplo] Ao lado do ponto de ônibus, km 2", null, null,
            "Lixeira verde: recicláveis limpos. Lixeira cinza: lixo comum ensacado. Não deixe entulho, móveis ou poda.",
            NoticiaColeta2024), ct);
        await pontos.CriarAsync(new SalvarPontoDescarteRequest("[Exemplo] Abrigo ecológico da capela",
            TipoPontoDescarte.AbrigoEcologico, zonaRural, "[Exemplo] Em frente à capela do bairro", null, null,
            "A Prefeitura informou abrigos ecológicos em 88 pontos da zona rural do distrito (2024).", NoticiaColeta2024), ct);
    }
}
