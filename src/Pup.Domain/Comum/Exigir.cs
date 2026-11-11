namespace Pup.Domain.Comum;

/// <summary>Validações repetidas nas entidades. Mensagens em português, prontas para a tela.</summary>
public static class Exigir
{
    public static string Texto(string? valor, string campo, int maximo)
    {
        var v = valor?.Trim();
        if (string.IsNullOrEmpty(v)) throw new DomainException($"Informe {campo}.");
        if (v.Length > maximo) throw new DomainException($"{Primeira(campo)} deve ter no máximo {maximo} caracteres.");
        return v;
    }

    public static string? TextoOpcional(string? valor, string campo, int maximo)
    {
        var v = valor?.Trim();
        if (string.IsNullOrEmpty(v)) return null;
        if (v.Length > maximo) throw new DomainException($"{Primeira(campo)} deve ter no máximo {maximo} caracteres.");
        return v;
    }

    /// <summary>Fonte oficial é opcional, mas quando informada precisa ser um link http(s) válido.</summary>
    public static string? Url(string? valor)
    {
        var v = valor?.Trim();
        if (string.IsNullOrEmpty(v)) return null;
        if (v.Length > 500 || !Uri.TryCreate(v, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new DomainException("O link da fonte oficial precisa ser um endereço http(s) válido.");
        return v;
    }

    public static void Definido<TEnum>(TEnum valor, string campo) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(valor)) throw new DomainException($"{Primeira(campo)} inválido(a).");
    }

    public static void Id(Guid id, string campo)
    {
        if (id == Guid.Empty) throw new DomainException($"Informe {campo}.");
    }

    private static string Primeira(string campo) =>
        campo.Length == 0 ? campo : char.ToUpperInvariant(campo[0]) + campo[1..];
}
