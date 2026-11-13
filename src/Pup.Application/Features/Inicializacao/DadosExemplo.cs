using Pup.Application.Comum;
using Pup.Application.Features.Coleta;
using Pup.Application.Features.Comunicados;
using Pup.Application.Features.Intervencoes;
using Pup.Application.Features.PontosDescarte;
using Pup.Application.Features.Regioes;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Inicializacao;

/// <summary>Dados iniciais para demonstração. Orquestra os Services das entidades.</summary>
public sealed partial class DadosExemplo(
    RegiaoService regioes,
    ColetaService coletas,
    PontoDescarteService pontos,
    IntervencaoService intervencoes,
    ComunicadoService comunicados,
    IRelogio relogio)
{
    private const string NoticiaColeta2023 =
        "https://www.sjc.sp.gov.br/noticias/2023/fevereiro/03/coleta-de-lixo-e-ampliada-em-sao-francisco-xavier/";
    private const string NoticiaColeta2024 =
        "https://www.sjc.sp.gov.br/noticias/2024/junho/27/coleta-seletiva-chega-na-zona-rural-de-sao-francisco-xavier/";

    public async Task SemearAsync(CancellationToken ct)
    {
        var hoje = relogio.Hoje;
        var centro = await regioes.CriarAsync(
            new SalvarRegiaoRequest("Centro", TipoRegiao.Urbana, "Área urbana central do distrito de São Francisco Xavier."), ct);
        var zonaRural = await regioes.CriarAsync(
            new SalvarRegiaoRequest("Zona rural", TipoRegiao.Rural, "Bairros e estradas rurais atendidos por abrigos ecológicos."), ct);
        var varadouro = await regioes.CriarAsync(
            new SalvarRegiaoRequest("Estrada do Varadouro", TipoRegiao.Rural, "[Exemplo] Estrada rural usada nos exemplos do protótipo."), ct);

        await SemearColetaEPontosAsync(centro, zonaRural, varadouro, hoje, ct);
        await SemearIntervencoesAsync(centro, zonaRural, varadouro, hoje, ct);
        await SemearComunicadosAsync(varadouro, ct);
    }
}
