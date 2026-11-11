using Pup.Application.Comum;
using Pup.Data.Coleta;
using Pup.Data.Comunicados;
using Pup.Data.Intervencoes;
using Pup.Data.Regioes;
using Pup.Domain.Coleta;
using Pup.Domain.Comum;
using Pup.Domain.Comunicados;
using Pup.Domain.Intervencoes;
using Pup.Domain.Regioes;

namespace Pup.Application.Features.Inicializacao;

/// <summary>
/// Dados para demonstrar o protótipo. Dias/horários da coleta no Centro vêm das notícias oficiais citadas
/// no relatório (com link); tudo que é inventado começa com "[Exemplo]" para não parecer informação oficial.
/// </summary>
public sealed class DadosExemplo(
    RegiaoRepo regioes,
    ColetaRepo coletas,
    PontoDescarteRepo pontos,
    IntervencaoRepo intervencoes,
    ComunicadoRepo comunicados,
    IRelogio relogio)
{
    private const string NoticiaColeta2023 =
        "https://www.sjc.sp.gov.br/noticias/2023/fevereiro/03/coleta-de-lixo-e-ampliada-em-sao-francisco-xavier/";
    private const string NoticiaColeta2024 =
        "https://www.sjc.sp.gov.br/noticias/2024/junho/27/coleta-seletiva-chega-na-zona-rural-de-sao-francisco-xavier/";

    public async Task SemearAsync(CancellationToken ct)
    {
        var hoje = relogio.Hoje;
        var inicio = new TimeOnly(7, 0);
        var fim = new TimeOnly(15, 20);
        const DiasSemana terQuiSab = DiasSemana.Terca | DiasSemana.Quinta | DiasSemana.Sabado;

        // Regiões (nomes alinhados às localidades exibidas no front)
        var centro = Regiao.Criar("Centro", TipoRegiao.Urbana, "Área urbana central do distrito de São Francisco Xavier.");
        var zonaRural = Regiao.Criar("Zona rural", TipoRegiao.Rural, "Bairros e estradas rurais atendidos por abrigos ecológicos.");
        var varadouro = Regiao.Criar("Estrada do Varadouro", TipoRegiao.Rural, "[Exemplo] Estrada rural usada nos exemplos do protótipo.");
        foreach (var r in new[] { centro, zonaRural, varadouro }) regioes.Adicionar(r);

        // Coleta (Centro e zona rural: notícias oficiais de 2023 e 2024)
        coletas.Adicionar(ColetaResiduos.Criar(centro.Id, ModalidadeColeta.Comum, terQuiSab, inicio, fim,
            "Confirme dias e horários atualizados nos canais oficiais da Prefeitura/Urbam.", NoticiaColeta2023));
        coletas.Adicionar(ColetaResiduos.Criar(centro.Id, ModalidadeColeta.Seletiva, terQuiSab, inicio, fim,
            "Separe papel, plástico, metal e vidro limpos. Horário de referência; confirme nos canais oficiais.", NoticiaColeta2024));
        coletas.Adicionar(ColetaResiduos.Criar(zonaRural.Id, ModalidadeColeta.Seletiva, DiasSemana.Quarta,
            new TimeOnly(8, 0), new TimeOnly(14, 0),
            "[Exemplo] Recolhimento nos abrigos ecológicos (lixeiras de comum e seletiva lado a lado).", NoticiaColeta2024));

        var coletaVaradouro = ColetaResiduos.Criar(varadouro.Id, ModalidadeColeta.Comum, DiasSemana.Terca | DiasSemana.Sabado,
            new TimeOnly(8, 0), new TimeOnly(12, 0), "[Exemplo] Coleta nos abrigos ecológicos ao longo da estrada.", null);
        coletaVaradouro.AlterarTemporariamente(SituacaoColeta.TemporariamenteAlterada,
            "[Exemplo] Por causa das chuvas, a coleta desta semana passa para quarta-feira.", hoje.AddDays(7), hoje);
        coletas.Adicionar(coletaVaradouro);

        // Pontos de descarte
        pontos.Adicionar(PontoDescarte.Criar("[Exemplo] Abrigo ecológico do Varadouro", TipoPontoDescarte.AbrigoEcologico,
            varadouro.Id, "[Exemplo] Ao lado do ponto de ônibus, km 2", null, null,
            "Lixeira verde: recicláveis limpos. Lixeira cinza: lixo comum ensacado. Não deixe entulho, móveis ou poda.",
            NoticiaColeta2024));
        pontos.Adicionar(PontoDescarte.Criar("[Exemplo] Abrigo ecológico da capela", TipoPontoDescarte.AbrigoEcologico,
            zonaRural.Id, "[Exemplo] Em frente à capela do bairro", null, null,
            "A Prefeitura informou abrigos ecológicos em 88 pontos da zona rural do distrito (2024).", NoticiaColeta2024));

        // Obras e manutenções (todas fictícias, datas relativas a hoje para a demonstração fazer sentido)
        intervencoes.Adicionar(Intervencao.Criar(TipoIntervencao.Obra, "[Exemplo] Adequação de via",
            "Cascalhamento, nivelamento e melhoria da drenagem em trecho de terra.", varadouro.Id, "Trecho de terra perto do km 2",
            StatusIntervencao.EmAndamento, hoje.AddDays(-20), hoje.AddDays(40), "Meia pista em alguns trechos durante o dia.", null));

        intervencoes.Adicionar(Intervencao.Criar(TipoIntervencao.Obra, "[Exemplo] Melhoria de drenagem",
            "Implantação e limpeza de galerias e bueiros em pontos críticos.", zonaRural.Id, null,
            StatusIntervencao.Programado, hoje.AddDays(10), hoje.AddDays(60), null, null));

        intervencoes.Adicionar(Intervencao.Criar(TipoIntervencao.Obra, "[Exemplo] Revitalização da praça central",
            "Troca de piso, bancos e paisagismo.", centro.Id, "Praça central",
            StatusIntervencao.Programado, hoje.AddDays(15), hoje.AddDays(90), null, null));

        intervencoes.Adicionar(Intervencao.Criar(TipoIntervencao.Manutencao, "[Exemplo] Manutenção de iluminação",
            "Troca de lâmpadas queimadas nas ruas do centro.", centro.Id, null,
            StatusIntervencao.Programado, hoje.AddDays(5), hoje.AddDays(7), null, null));

        intervencoes.Adicionar(Intervencao.Criar(TipoIntervencao.Manutencao, "[Exemplo] Desobstrução de drenagem",
            "Limpeza de bueiros e canaletas para escoamento de águas pluviais.", varadouro.Id, null,
            StatusIntervencao.EmAndamento, hoje.AddDays(-3), hoje.AddDays(4), "Possíveis bloqueios rápidos.", null));

        var nivelamento = Intervencao.Criar(TipoIntervencao.Manutencao, "[Exemplo] Nivelamento de estrada",
            "Patrolamento e recomposição de cascalho após chuvas.", zonaRural.Id, "Estradas rurais",
            StatusIntervencao.EmAndamento, hoje.AddDays(-10), hoje.AddDays(-2), null, null);
        nivelamento.MudarStatus(StatusIntervencao.Concluido, "[Exemplo] Serviço concluído e via liberada.", hoje);
        intervencoes.Adicionar(nivelamento);

        // Comunicados
        comunicados.Adicionar(Comunicado.Criar("Coleta seletiva também na zona rural",
            "Abrigos ecológicos com lixeiras para lixo comum e recicláveis foram instalados na zona rural.",
            "Segundo a Prefeitura, a Urbam instalou lixeiras comunitárias (abrigos ecológicos) em pontos da zona rural de " +
            "São Francisco Xavier, com lixeiras de coleta comum e seletiva lado a lado. Separe os recicláveis limpos e " +
            "deposite o lixo comum ensacado.",
            CategoriaComunicado.Coleta, null, null, destaque: true, NoticiaColeta2024, hoje));

        comunicados.Adicionar(Comunicado.Criar("Solicitações pelo 156",
            "Pedidos de manutenção e limpeza podem ser feitos pelo canal oficial 156 da Prefeitura.",
            "Para registrar solicitações de manutenção, limpeza e outras demandas urbanas, use os canais oficiais 156 " +
            "da Prefeitura de São José dos Campos. Este portal apenas organiza informações e não substitui o atendimento oficial.",
            CategoriaComunicado.Geral, null, null, destaque: true, null, hoje));

        comunicados.Adicionar(Comunicado.Criar("[Exemplo] Interdição parcial na Estrada do Varadouro",
            "Meia pista em trechos da estrada durante a obra de adequação.",
            "[Exemplo] Durante a obra de adequação de via, a estrada terá meia pista em alguns trechos, das 8h às 16h. Programe-se.",
            CategoriaComunicado.Alerta, varadouro.Id, hoje.AddDays(15), destaque: false, null, hoje));

        comunicados.Adicionar(Comunicado.Criar("Sobre este portal",
            "Protótipo acadêmico do Projeto Integrador II (UNIVESP).",
            "Protótipo acadêmico do Projeto Integrador II (UNIVESP). Organiza informações públicas de São Francisco Xavier " +
            "e não substitui os canais oficiais da Prefeitura de São José dos Campos.",
            CategoriaComunicado.Geral, null, null, destaque: false, null, hoje));

        // Um único SaveChanges: tudo é gravado junto (ou nada).
        await regioes.SalvarAsync(ct);
    }
}
