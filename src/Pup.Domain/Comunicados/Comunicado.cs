using Pup.Domain.Comum;

namespace Pup.Domain.Comunicados;

/// <summary>Aviso ou notícia para os moradores. Pode valer para o distrito todo ou para uma região.</summary>
public sealed class Comunicado : Entidade
{
    public string Titulo { get; private set; } = string.Empty;
    public string Resumo { get; private set; } = string.Empty;
    public string Conteudo { get; private set; } = string.Empty;
    public CategoriaComunicado Categoria { get; private set; }
    /// <summary>Nulo = vale para o distrito inteiro.</summary>
    public Guid? RegiaoId { get; private set; }
    public DateTime PublicadoEm { get; private set; } = DateTime.UtcNow;
    /// <summary>Depois desta data o comunicado sai da página inicial (continua no histórico).</summary>
    public DateOnly? ValidoAte { get; private set; }
    public bool Destaque { get; private set; }
    public string? FonteOficialUrl { get; private set; }

    private Comunicado() { }

    public static Comunicado Criar(string titulo, string resumo, string conteudo, CategoriaComunicado categoria,
        Guid? regiaoId, DateOnly? validoAte, bool destaque, string? fonteOficialUrl, DateOnly hoje)
    {
        var c = new Comunicado();
        c.Preencher(titulo, resumo, conteudo, categoria, regiaoId, validoAte, destaque, fonteOficialUrl, hoje);
        return c;
    }

    public void Atualizar(string titulo, string resumo, string conteudo, CategoriaComunicado categoria,
        Guid? regiaoId, DateOnly? validoAte, bool destaque, string? fonteOficialUrl, DateOnly hoje)
    {
        Preencher(titulo, resumo, conteudo, categoria, regiaoId, validoAte, destaque, fonteOficialUrl, hoje);
        Tocar();
    }

    public bool Vigente(DateOnly hoje) => ValidoAte is null || ValidoAte >= hoje;

    private void Preencher(string titulo, string resumo, string conteudo, CategoriaComunicado categoria,
        Guid? regiaoId, DateOnly? validoAte, bool destaque, string? fonteOficialUrl, DateOnly hoje)
    {
        Exigir.Definido(categoria, "categoria");
        if (regiaoId == Guid.Empty) regiaoId = null;
        if (validoAte is { } v && v < hoje)
            throw new DomainException("A data de validade não pode estar no passado.");

        Titulo = Exigir.Texto(titulo, "o título", 200);
        Resumo = Exigir.Texto(resumo, "o resumo", 300);
        Conteudo = Exigir.Texto(conteudo, "o texto do comunicado", 4000);
        Categoria = categoria;
        RegiaoId = regiaoId;
        ValidoAte = validoAte;
        Destaque = destaque;
        FonteOficialUrl = Exigir.Url(fonteOficialUrl);
    }
}
