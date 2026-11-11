# PUP-03 — Rotas no formato do front (`/api/portal/servicos` e `/api/portal/comunicados`)

**Depende de: PUP-01**
Relatório: `2026-11-11_PUP-03_rotas-do-portal-RELATORIO.md`

## Objetivo

O front da dupla filtra tudo no navegador a partir de duas listas do `assets/js/data.js`. Esta tarefa cria duas
rotas GET que devolvem essas listas **no mesmo formato e com os mesmos textos** que o front já usa, a partir dos
dados reais do banco. Assim o front só troca a origem dos dados (PUP-05), sem reescrever telas.

Formato que o front espera (copiado do `data.js` deles):

```js
// services (coleta, pontos de descarte, obras e manutenções juntos)
{ id, title, category, description, location, status, updatedAt: "2026-10-18", schedule, officialSourceUrl, isDemo }
// announcements
{ id, title, message, category, publishedAt: "2026-10-21", officialSourceUrl, isDemo }
```

Textos que os `<select>` do front comparam **por igualdade** (não mude acentos nem maiúsculas):
- category de serviço: `Coleta de resíduos`, `Obras`, `Manutenção`
- status: `Programado`, `Em andamento`, `Concluído`, `Alterado` (o back também pode mandar `Cancelado`)
- category de comunicado: `Coleta`, `Obras`, `Manutenção`, `Geral` (o back também manda `Alerta`; a PUP-05 põe a opção no front)
- datas: só a data, `yyyy-MM-dd` (o `formatDate` do front quebra com data e hora)

## Arquivos

- `src/Pup.Contracts/Portal.cs` (novo)
- `src/Pup.Contracts/Intervencoes.cs` (acrescentar `Descricao` no `IntervencaoResumoDto`)
- `src/Pup.Data/Intervencoes/IntervencaoQuery.cs` (preencher o novo campo na projeção)
- `src/Pup.Application/Comum/Relogio.cs`
- `src/Pup.Application/Features/Portal/MapeadorPortal.cs` (novo)
- `src/Pup.Application/Features/Portal/PortalService.cs` (novo)
- `src/Pup.Application/Features/Portal/PortalSlices.cs` (novo)
- `src/Pup.Application/DependencyInjection.cs`
- `src/Pup.Api/Controllers/PortalController.cs` (novo)
- `src/Pup.Api/Pup.Api.http` (acrescentar as duas rotas)
- `ferramentas/smoke.ps1` (acrescentar as duas rotas)
- `tests/Pup.Tests/MapeadorPortalTests.cs` (novo, **copie exatamente** o código do fim desta tarefa)

## O que fazer

### 1. Contracts — `src/Pup.Contracts/Portal.cs` (copie assim)

```csharp
using System.Text.Json.Serialization;

namespace Pup.Contracts;

/// <summary>
/// Item de "services" do front estático (assets/js/data.js).
/// Os nomes do JSON ficam em inglês porque o JavaScript da dupla do front já usa esses nomes.
/// </summary>
public sealed record PortalServicoDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("title")] string Titulo,
    [property: JsonPropertyName("category")] string Categoria,
    [property: JsonPropertyName("description")] string Descricao,
    [property: JsonPropertyName("location")] string Localidade,
    [property: JsonPropertyName("status")] string Situacao,
    [property: JsonPropertyName("updatedAt")] DateOnly AtualizadoEm,
    [property: JsonPropertyName("schedule")] string Programacao,
    [property: JsonPropertyName("officialSourceUrl")] string? FonteOficialUrl,
    [property: JsonPropertyName("isDemo")] bool Demonstracao);

/// <summary>Item de "announcements" do front estático.</summary>
public sealed record PortalComunicadoDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("title")] string Titulo,
    [property: JsonPropertyName("message")] string Mensagem,
    [property: JsonPropertyName("category")] string Categoria,
    [property: JsonPropertyName("publishedAt")] DateOnly PublicadoEm,
    [property: JsonPropertyName("officialSourceUrl")] string? FonteOficialUrl,
    [property: JsonPropertyName("isDemo")] bool Demonstracao);

/// <summary>Textos exatos que os filtros do front comparam por igualdade. Não mude acentos nem maiúsculas.</summary>
public static class PortalTextos
{
    public const string CategoriaColeta = "Coleta de resíduos";
    public const string CategoriaObras = "Obras";
    public const string CategoriaManutencao = "Manutenção";

    public const string Programado = "Programado";
    public const string EmAndamento = "Em andamento";
    public const string Concluido = "Concluído";
    public const string Alterado = "Alterado";
    public const string Cancelado = "Cancelado";
}
```

### 2. `IntervencaoResumoDto` ganha a descrição

Em `src/Pup.Contracts/Intervencoes.cs`, acrescente `string Descricao` **logo depois de** `string Titulo` no
`IntervencaoResumoDto`. Em `IntervencaoQuery.Projetar`, passe `x.I.Descricao` na posição nova.

### 3. Relógio — data local de um instante UTC

Em `src/Pup.Application/Comum/Relogio.cs`:
- na interface `IRelogio`, acrescente `DateOnly DataLocal(DateTime utc);` com o comentário
  `/// <summary>Data em Brasília de um instante gravado em UTC (o front mostra só a data).</summary>`;
- na classe `Relogio`: `public DateOnly DataLocal(DateTime utc) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Fuso));`
  e troque o corpo de `Hoje` por `DataLocal(DateTime.UtcNow)`.

### 4. `MapeadorPortal` — funções puras (copie assim)

`src/Pup.Application/Features/Portal/MapeadorPortal.cs`:

```csharp
using System.Globalization;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Portal;

/// <summary>
/// Converte os DTOs da API para o formato do front estático (assets/js/data.js).
/// Funções puras: sem banco e sem relógio (a data local chega pronta do PortalService).
/// </summary>
public static class MapeadorPortal
{
    private const string MarcaExemplo = "[Exemplo]";
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Tira a marca "[Exemplo]" do texto e diz se ela existia (o front mostra o aviso de demonstração).</summary>
    public static (string Texto, bool Exemplo) SepararExemplo(string? texto)
    {
        var t = texto?.Trim() ?? string.Empty;
        return t.StartsWith(MarcaExemplo, StringComparison.OrdinalIgnoreCase)
            ? (t[MarcaExemplo.Length..].Trim(), true)
            : (t, false);
    }

    public static string Situacao(SituacaoColeta s) =>
        s == SituacaoColeta.Regular ? PortalTextos.Programado : PortalTextos.Alterado;

    public static string Situacao(StatusIntervencao s) => s switch
    {
        StatusIntervencao.Programado => PortalTextos.Programado,
        StatusIntervencao.EmAndamento => PortalTextos.EmAndamento,
        StatusIntervencao.Concluido => PortalTextos.Concluido,
        StatusIntervencao.TemporariamenteAlterado => PortalTextos.Alterado,
        _ => PortalTextos.Cancelado
    };

    public static string Categoria(CategoriaComunicado c) => c switch
    {
        CategoriaComunicado.Coleta => "Coleta",
        CategoriaComunicado.Obras => "Obras",
        CategoriaComunicado.Manutencao => "Manutenção",
        CategoriaComunicado.Alerta => "Alerta",
        _ => "Geral"
    };

    public static PortalServicoDto DeColeta(ColetaDto c, DateOnly atualizadoEm)
    {
        var observacao = SepararExemplo(c.Observacao);
        var aviso = SepararExemplo(c.AvisoAlteracao);
        var alterada = c.Situacao != SituacaoColeta.Regular;
        var programacao = $"{Maiuscula(c.Dias.Nome())}, das {Hora(c.HorarioInicio)} às {Hora(c.HorarioFim)}";

        return new PortalServicoDto(
            c.Id.ToString(),
            c.Modalidade == ModalidadeColeta.Comum ? "Coleta comum" : "Coleta seletiva",
            PortalTextos.CategoriaColeta,
            alterada && aviso.Texto.Length > 0 ? aviso.Texto : observacao.Texto,
            c.RegiaoNome,
            Situacao(c.Situacao),
            atualizadoEm,
            programacao,
            c.FonteOficialUrl,
            observacao.Exemplo || aviso.Exemplo);
    }

    /// <summary>Ponto de descarte ativo aparece como serviço de coleta "Em andamento" (disponível).</summary>
    public static PortalServicoDto DePonto(PontoDescarteDto p, DateOnly atualizadoEm)
    {
        var nome = SepararExemplo(p.Nome);
        var referencia = SepararExemplo(p.Referencia);
        var orientacoes = SepararExemplo(p.Orientacoes);
        var descricao = orientacoes.Texto.Length == 0 ? referencia.Texto : $"{referencia.Texto} — {orientacoes.Texto}";

        return new PortalServicoDto(p.Id.ToString(), nome.Texto, PortalTextos.CategoriaColeta, descricao, p.RegiaoNome,
            PortalTextos.EmAndamento, atualizadoEm, p.Tipo.Nome(), p.FonteOficialUrl,
            nome.Exemplo || referencia.Exemplo || orientacoes.Exemplo);
    }

    public static PortalServicoDto DeIntervencao(IntervencaoResumoDto i, DateOnly atualizadoEm)
    {
        var titulo = SepararExemplo(i.Titulo);
        var descricao = SepararExemplo(i.Descricao);
        var local = SepararExemplo(i.Local);
        var texto = descricao.Texto;
        if (local.Texto.Length > 0) texto += $" Local: {local.Texto}.";
        if (!string.IsNullOrWhiteSpace(i.ImpactoNaVia)) texto += $" Impacto na via: {i.ImpactoNaVia.Trim()}.";

        return new PortalServicoDto(
            i.Id.ToString(),
            titulo.Texto,
            i.Tipo == TipoIntervencao.Obra ? PortalTextos.CategoriaObras : PortalTextos.CategoriaManutencao,
            texto.Trim(),
            i.RegiaoNome,
            Situacao(i.Status),
            atualizadoEm,
            Programacao(i),
            i.FonteOficialUrl,
            titulo.Exemplo || descricao.Exemplo || local.Exemplo);
    }

    public static PortalComunicadoDto DeComunicado(ComunicadoDto c, DateOnly publicadoEm)
    {
        var titulo = SepararExemplo(c.Titulo);
        var conteudo = SepararExemplo(c.Conteudo);
        return new PortalComunicadoDto(c.Id.ToString(), titulo.Texto, conteudo.Texto, Categoria(c.Categoria), publicadoEm,
            c.FonteOficialUrl, titulo.Exemplo || conteudo.Exemplo);
    }

    private static string Programacao(IntervencaoResumoDto i)
    {
        if (i.Status == StatusIntervencao.Concluido && i.DataConclusao is { } fim) return $"Concluído em {Data(fim)}";
        if (i.Status == StatusIntervencao.Cancelado) return PortalTextos.Cancelado;
        if (i.PrevisaoConclusao is { } previsao) return $"Previsão: {Data(previsao)}" + (i.Atrasada ? " (vencida)" : string.Empty);
        return "Previsão não informada";
    }

    private static string Data(DateOnly d) => d.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    private static string Hora(TimeOnly h) => h.ToString("HH:mm", CultureInfo.InvariantCulture);

    private static string Maiuscula(string t) => t.Length == 0 ? t : char.ToUpper(t[0], PtBr) + t[1..];
}
```

### 5. `PortalService` (só leitura; segue o padrão dos outros services)

`src/Pup.Application/Features/Portal/PortalService.cs`, classe `public sealed class PortalService(ColetaQuery coletas,
PontoDescarteQuery pontos, IntervencaoQuery intervencoes, ComunicadoQuery comunicados, IRelogio relogio)`:

- `Task<IReadOnlyList<PortalServicoDto>> ServicosAsync(CancellationToken ct)`:
  - `hoje = relogio.Hoje`;
  - todas as coletas (`coletas.ListarAsync(null, null, null, hoje, ct)`) → `MapeadorPortal.DeColeta(c, relogio.DataLocal(c.AtualizadoEm))`;
  - pontos **ativos** (`pontos.ListarAsync(null, null, false, ct)`) → `DePonto`;
  - todas as obras e manutenções (`intervencoes.ListarAsync(new FiltroIntervencoes(), hoje, 500, ct)`) → `DeIntervencao`;
  - junta tudo e ordena por `AtualizadoEm` decrescente e depois por `Titulo` (o front mostra os 4 primeiros como "Serviços recentes").
  - As consultas são **sequenciais** (mesmo DbContext não aceita consultas em paralelo).
- `Task<IReadOnlyList<PortalComunicadoDto>> ComunicadosAsync(CancellationToken ct)`:
  só os vigentes, na ordem que a query já devolve (destaques primeiro):
  `comunicados.ListarAsync(null, null, true, false, relogio.Hoje, 100, ct)` → `DeComunicado(c, relogio.DataLocal(c.PublicadoEm))`.

### 6. Slices, DI e controller

- `PortalSlices.cs`: `ListarServicosDoPortal` (`record Query : IQuery<IReadOnlyList<PortalServicoDto>>`) e
  `ListarComunicadosDoPortal` (`record Query : IQuery<IReadOnlyList<PortalComunicadoDto>>`), handlers `internal sealed`
  chamando o `PortalService` (copie o formato de `PainelSlices.cs`).
- `DependencyInjection.cs` (Application): `services.AddScoped<PortalService>();`.
- `PortalController.cs`: `[ApiController] [Route("api/portal")]`, `GET servicos` e `GET comunicados`, públicos,
  só chamando o `IDispatcher` (copie o formato do `PainelController`). Nomes dos métodos: `Servicos` e `Comunicados`.
- `Pup.Api.http` e `ferramentas/smoke.ps1`: acrescente `/api/portal/servicos` e `/api/portal/comunicados`.

### 7. Teste — `tests/Pup.Tests/MapeadorPortalTests.cs` (copie exatamente)

```csharp
using Pup.Application.Features.Portal;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Tests;

public class MapeadorPortalTests
{
    private static readonly DateOnly Dia = new(2026, 11, 10);

    [Fact]
    public void SepararExemplo_ComMarca_RemoveMarcaEMarcaComoExemplo()
    {
        var (texto, exemplo) = MapeadorPortal.SepararExemplo("[Exemplo] Estrada do Varadouro");
        Assert.Equal("Estrada do Varadouro", texto);
        Assert.True(exemplo);
    }

    [Fact]
    public void SepararExemplo_SemMarca_MantemTexto()
    {
        Assert.Equal(("Centro", false), MapeadorPortal.SepararExemplo(" Centro "));
        Assert.Equal((string.Empty, false), MapeadorPortal.SepararExemplo(null));
    }

    [Theory]
    [InlineData(StatusIntervencao.Programado, "Programado")]
    [InlineData(StatusIntervencao.EmAndamento, "Em andamento")]
    [InlineData(StatusIntervencao.Concluido, "Concluído")]
    [InlineData(StatusIntervencao.TemporariamenteAlterado, "Alterado")]
    [InlineData(StatusIntervencao.Cancelado, "Cancelado")]
    public void Situacao_Intervencao_UsaTextoDoFront(StatusIntervencao status, string esperado) =>
        Assert.Equal(esperado, MapeadorPortal.Situacao(status));

    [Theory]
    [InlineData(SituacaoColeta.Regular, "Programado")]
    [InlineData(SituacaoColeta.TemporariamenteAlterada, "Alterado")]
    [InlineData(SituacaoColeta.Suspensa, "Alterado")]
    public void Situacao_Coleta_UsaTextoDoFront(SituacaoColeta situacao, string esperado) =>
        Assert.Equal(esperado, MapeadorPortal.Situacao(situacao));

    [Fact]
    public void DeColeta_Regular_MontaProgramacaoComDiasEHorario()
    {
        var c = new ColetaDto(Guid.NewGuid(), Guid.NewGuid(), "Centro", TipoRegiao.Urbana, ModalidadeColeta.Seletiva,
            DiasSemana.Terca | DiasSemana.Quinta | DiasSemana.Sabado, new TimeOnly(7, 0), new TimeOnly(15, 20),
            "Separe os recicláveis.", SituacaoColeta.Regular, null, null, "https://www.sjc.sp.gov.br/", DateTime.UtcNow);

        var s = MapeadorPortal.DeColeta(c, Dia);

        Assert.Equal(c.Id.ToString(), s.Id);
        Assert.Equal("Coleta seletiva", s.Titulo);
        Assert.Equal("Coleta de resíduos", s.Categoria);
        Assert.Equal("Centro", s.Localidade);
        Assert.Equal("Programado", s.Situacao);
        Assert.Equal("Terça, quinta e sábado, das 07:00 às 15:20", s.Programacao);
        Assert.Equal("Separe os recicláveis.", s.Descricao);
        Assert.Equal(Dia, s.AtualizadoEm);
        Assert.False(s.Demonstracao);
    }

    [Fact]
    public void DeColeta_Alterada_MostraOAvisoNaDescricao()
    {
        var c = new ColetaDto(Guid.NewGuid(), Guid.NewGuid(), "Zona rural", TipoRegiao.Rural, ModalidadeColeta.Comum,
            DiasSemana.Terca, new TimeOnly(8, 0), new TimeOnly(12, 0), "[Exemplo] Coleta nos abrigos.",
            SituacaoColeta.TemporariamenteAlterada, "[Exemplo] Coleta passa para quarta.", Dia.AddDays(3), null, DateTime.UtcNow);

        var s = MapeadorPortal.DeColeta(c, Dia);

        Assert.Equal("Coleta comum", s.Titulo);
        Assert.Equal("Alterado", s.Situacao);
        Assert.Equal("Coleta passa para quarta.", s.Descricao);
        Assert.True(s.Demonstracao);
    }

    [Fact]
    public void DePonto_JuntaReferenciaEOrientacoes()
    {
        var p = new PontoDescarteDto(Guid.NewGuid(), "[Exemplo] Abrigo da capela", TipoPontoDescarte.AbrigoEcologico,
            Guid.NewGuid(), "Zona rural", "Em frente à capela", null, null, "Recicláveis limpos.", true, null, DateTime.UtcNow);

        var s = MapeadorPortal.DePonto(p, Dia);

        Assert.Equal("Abrigo da capela", s.Titulo);
        Assert.Equal("Coleta de resíduos", s.Categoria);
        Assert.Equal("Em andamento", s.Situacao);
        Assert.Equal("Abrigo ecológico", s.Programacao);
        Assert.Equal("Em frente à capela — Recicláveis limpos.", s.Descricao);
        Assert.True(s.Demonstracao);
    }

    [Fact]
    public void DeIntervencao_ComPrevisaoVencida_AvisaNaProgramacao()
    {
        var i = new IntervencaoResumoDto(Guid.NewGuid(), TipoIntervencao.Obra, "[Exemplo] Adequação de via", "Cascalhamento.",
            Guid.NewGuid(), "Estrada do Varadouro", "km 2", StatusIntervencao.EmAndamento, new DateOnly(2026, 10, 1),
            new DateOnly(2026, 11, 5), null, "Meia pista", true, DateTime.UtcNow);

        var s = MapeadorPortal.DeIntervencao(i, Dia);

        Assert.Equal("Adequação de via", s.Titulo);
        Assert.Equal("Obras", s.Categoria);
        Assert.Equal("Em andamento", s.Situacao);
        Assert.Equal("Estrada do Varadouro", s.Localidade);
        Assert.Equal("Previsão: 05/11/2026 (vencida)", s.Programacao);
        Assert.Equal("Cascalhamento. Local: km 2. Impacto na via: Meia pista.", s.Descricao);
        Assert.True(s.Demonstracao);
    }

    [Fact]
    public void DeIntervencao_Concluida_MostraDataDeConclusao()
    {
        var i = new IntervencaoResumoDto(Guid.NewGuid(), TipoIntervencao.Manutencao, "Nivelamento", "Nivelamento da estrada.",
            Guid.NewGuid(), "Zona rural", null, StatusIntervencao.Concluido, null, null, new DateOnly(2026, 11, 3), null, false,
            DateTime.UtcNow);

        var s = MapeadorPortal.DeIntervencao(i, Dia);

        Assert.Equal("Manutenção", s.Categoria);
        Assert.Equal("Concluído", s.Situacao);
        Assert.Equal("Concluído em 03/11/2026", s.Programacao);
        Assert.Equal("Nivelamento da estrada.", s.Descricao);
        Assert.False(s.Demonstracao);
    }

    [Fact]
    public void DeComunicado_Alerta_UsaCategoriaETextoCompleto()
    {
        var c = new ComunicadoDto(Guid.NewGuid(), "[Exemplo] Interdição", "Resumo", "[Exemplo] Meia pista das 8h às 16h.",
            CategoriaComunicado.Alerta, null, null, DateTime.UtcNow, null, false, true, null, DateTime.UtcNow);

        var p = MapeadorPortal.DeComunicado(c, Dia);

        Assert.Equal("Interdição", p.Titulo);
        Assert.Equal("Meia pista das 8h às 16h.", p.Mensagem);
        Assert.Equal("Alerta", p.Categoria);
        Assert.Equal(Dia, p.PublicadoEm);
        Assert.True(p.Demonstracao);
    }

    [Fact]
    public void Json_UsaOsNomesDoFront()
    {
        var p = new PortalComunicadoDto("1", "T", "M", "Geral", Dia, null, false);

        var json = System.Text.Json.JsonSerializer.Serialize(p);

        Assert.Contains("\"publishedAt\":\"2026-11-10\"", json);
        Assert.Contains("\"message\":\"M\"", json);
        Assert.Contains("\"isDemo\":false", json);
    }
}
```

### 8. Commit `PUP-03: rotas no formato do front`.

## Não mexa

- As rotas que já existem (o painel administrativo e o futuro front em Blazor usam elas).
- Os arquivos da pasta do front (a integração é a PUP-05).

## Pronto quando

- [ ] `GET /api/portal/servicos` devolve coletas, pontos, obras e manutenções com os campos acima
- [ ] `GET /api/portal/comunicados` devolve os comunicados vigentes com os campos acima
- [ ] `MapeadorPortalTests` passando junto com os testes antigos
- [ ] Smoke verde (com as duas rotas novas)
