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
