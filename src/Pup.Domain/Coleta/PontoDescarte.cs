using Pup.Domain.Comum;

namespace Pup.Domain.Coleta;

/// <summary>Abrigo ecológico, lixeira comunitária ou ecoponto onde o morador pode descartar resíduos.</summary>
public sealed class PontoDescarte : Entidade
{
    public string Nome { get; private set; } = string.Empty;
    public TipoPontoDescarte Tipo { get; private set; }
    public Guid RegiaoId { get; private set; }
    /// <summary>Endereço ou ponto de referência ("em frente à capela", "km 3 da estrada...").</summary>
    public string Referencia { get; private set; } = string.Empty;
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    /// <summary>O que pode (e o que não pode) ser descartado ali.</summary>
    public string? Orientacoes { get; private set; }
    public bool Ativo { get; private set; } = true;
    public string? FonteOficialUrl { get; private set; }

    private PontoDescarte() { }

    public static PontoDescarte Criar(string nome, TipoPontoDescarte tipo, Guid regiaoId, string referencia,
        double? latitude, double? longitude, string? orientacoes, string? fonteOficialUrl)
    {
        var p = new PontoDescarte();
        p.Preencher(nome, tipo, regiaoId, referencia, latitude, longitude, orientacoes, fonteOficialUrl);
        return p;
    }

    public void Atualizar(string nome, TipoPontoDescarte tipo, Guid regiaoId, string referencia,
        double? latitude, double? longitude, string? orientacoes, string? fonteOficialUrl, bool ativo)
    {
        Preencher(nome, tipo, regiaoId, referencia, latitude, longitude, orientacoes, fonteOficialUrl);
        Ativo = ativo;
        Tocar();
    }

    private void Preencher(string nome, TipoPontoDescarte tipo, Guid regiaoId, string referencia,
        double? latitude, double? longitude, string? orientacoes, string? fonteOficialUrl)
    {
        Exigir.Definido(tipo, "tipo de ponto");
        Exigir.Id(regiaoId, "a região");
        if ((latitude is null) != (longitude is null))
            throw new DomainException("Informe latitude e longitude juntas (ou deixe as duas em branco).");
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            throw new DomainException("Coordenadas inválidas.");

        Nome = Exigir.Texto(nome, "o nome do ponto", 150);
        Tipo = tipo;
        RegiaoId = regiaoId;
        Referencia = Exigir.Texto(referencia, "o endereço ou ponto de referência", 300);
        Latitude = latitude;
        Longitude = longitude;
        Orientacoes = Exigir.TextoOpcional(orientacoes, "as orientações", 1000);
        FonteOficialUrl = Exigir.Url(fonteOficialUrl);
    }
}
