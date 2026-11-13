using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Inicializacao;

public sealed partial class DadosExemplo
{
    private async Task SemearComunicadosAsync(Guid varadouro, CancellationToken ct)
    {
        var hoje = relogio.Hoje;

        await comunicados.CriarAsync(new SalvarComunicadoRequest("Coleta seletiva também na zona rural",
            "Abrigos ecológicos com lixeiras para lixo comum e recicláveis foram instalados na zona rural.",
            "Segundo a Prefeitura, a Urbam instalou lixeiras comunitárias (abrigos ecológicos) em pontos da zona rural de " +
            "São Francisco Xavier, com lixeiras de coleta comum e seletiva lado a lado. Separe os recicláveis limpos e " +
            "deposite o lixo comum ensacado.",
            CategoriaComunicado.Coleta, null, null, true, NoticiaColeta2024), ct);

        await comunicados.CriarAsync(new SalvarComunicadoRequest("Solicitações pelo 156",
            "Pedidos de manutenção e limpeza podem ser feitos pelo canal oficial 156 da Prefeitura.",
            "Para registrar solicitações de manutenção, limpeza e outras demandas urbanas, use os canais oficiais 156 " +
            "da Prefeitura de São José dos Campos. Este portal apenas organiza informações e não substitui o atendimento oficial.",
            CategoriaComunicado.Geral, null, null, true, null), ct);

        await comunicados.CriarAsync(new SalvarComunicadoRequest("[Exemplo] Interdição parcial na Estrada do Varadouro",
            "Meia pista em trechos da estrada durante a obra de adequação.",
            "[Exemplo] Durante a obra de adequação de via, a estrada terá meia pista em alguns trechos, das 8h às 16h. Programe-se.",
            CategoriaComunicado.Alerta, varadouro, hoje.AddDays(15), false, null), ct);

        await comunicados.CriarAsync(new SalvarComunicadoRequest("Sobre este portal",
            "Protótipo acadêmico do Projeto Integrador II (UNIVESP).",
            "Protótipo acadêmico do Projeto Integrador II (UNIVESP). Organiza informações públicas de São Francisco Xavier " +
            "e não substitui os canais oficiais da Prefeitura de São José dos Campos.",
            CategoriaComunicado.Geral, null, null, false, null), ct);
    }
}
