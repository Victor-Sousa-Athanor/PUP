using System.Text.RegularExpressions;
using Pup.Domain.Comum;

namespace Pup.Domain.Paginas;

/// <summary>
/// Página e rota configurável pelo painel administrativo para o menu e conteúdo dinâmico do portal.
/// </summary>
public sealed partial class Pagina : Entidade
{
    private static readonly string[] PrefixosReservados =
        ["api", "swagger", "openapi", "health", "admin.html", "pagina.html"];

    public string Titulo { get; private set; } = string.Empty;
    public string Rota { get; private set; } = string.Empty;
    public string? Resumo { get; private set; }
    public string? Conteudo { get; private set; }
    public int Ordem { get; private set; }
    public bool ExibirNoMenu { get; private set; } = true;
    public bool Publicada { get; private set; } = true;
    public bool EhSistema { get; private set; }
    public string? FonteOficialUrl { get; private set; }

    private Pagina() { }

    public static Pagina Criar(
        string titulo,
        string rota,
        string? resumo,
        string? conteudo,
        int ordem,
        bool exibirNoMenu,
        bool publicada,
        bool ehSistema,
        string? fonteOficialUrl)
    {
        var pagina = new Pagina { EhSistema = ehSistema };
        pagina.Preencher(titulo, rota, resumo, conteudo, ordem, exibirNoMenu, publicada, fonteOficialUrl);
        return pagina;
    }

    public void Atualizar(
        string titulo,
        string rota,
        string? resumo,
        string? conteudo,
        int ordem,
        bool exibirNoMenu,
        bool publicada,
        string? fonteOficialUrl)
    {
        var rotaNormalizada = NormalizarRota(rota);
        if (EhSistema && !string.Equals(Rota, rotaNormalizada, StringComparison.Ordinal))
            throw new DomainException("A rota de uma página fixa do sistema não pode ser alterada.");

        Preencher(titulo, rotaNormalizada, resumo, conteudo, ordem, exibirNoMenu, publicada, fonteOficialUrl);
        Tocar();
    }

    public static string NormalizarRota(string? valor)
    {
        var rota = (valor ?? string.Empty).Trim().Trim('/').ToLowerInvariant();
        if (string.IsNullOrEmpty(rota))
            throw new DomainException("Informe a rota da página (ex.: telefones-uteis).");
        if (rota.Length > 120)
            throw new DomainException("A rota deve ter no máximo 120 caracteres.");
        if (!PadraoRotaValida().IsMatch(rota) || rota.Contains("..", StringComparison.Ordinal))
            throw new DomainException("Use apenas letras minúsculas, números, hífen, sublinhado ou ponto na rota.");

        var primeiroSegmento = rota.Split('/')[0];
        if (PrefixosReservados.Contains(primeiroSegmento, StringComparer.Ordinal))
            throw new DomainException($"A rota '{primeiroSegmento}' é reservada pelo sistema.");

        return rota;
    }

    private void Preencher(
        string titulo,
        string rota,
        string? resumo,
        string? conteudo,
        int ordem,
        bool exibirNoMenu,
        bool publicada,
        string? fonteOficialUrl)
    {
        if (ordem < 0)
            throw new DomainException("A ordem da página não pode ser negativa.");

        Titulo = Exigir.Texto(titulo, "o título da página", 80);
        Rota = NormalizarRota(rota);
        Resumo = Exigir.TextoOpcional(resumo, "o resumo", 300);
        Conteudo = Exigir.TextoOpcional(conteudo, "o conteúdo", 8000);
        Ordem = ordem;
        ExibirNoMenu = exibirNoMenu;
        Publicada = publicada;
        FonteOficialUrl = Exigir.Url(fonteOficialUrl);
    }

    [GeneratedRegex("^[a-z0-9][a-z0-9\\-_./]*$", RegexOptions.CultureInvariant)]
    private static partial Regex PadraoRotaValida();
}
