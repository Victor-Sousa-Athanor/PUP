using System.Globalization;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Portal;

/// <summary>Converte os DTOs da API para o formato do front estático. Funções puras: sem banco e sem relógio.</summary>
public static partial class MapeadorPortal
{
    private const string MarcaExemplo = "[Exemplo]";
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public static (string Texto, bool Exemplo) SepararExemplo(string? texto)
    {
        var t = texto?.Trim() ?? string.Empty;
        return t.StartsWith(MarcaExemplo, StringComparison.OrdinalIgnoreCase)
            ? (t[MarcaExemplo.Length..].Trim(), true)
            : (t, false);
    }

    public static PortalComunicadoDto DeComunicado(ComunicadoDto c, DateOnly publicadoEm)
    {
        var titulo = SepararExemplo(c.Titulo);
        var conteudo = SepararExemplo(c.Conteudo);
        return new PortalComunicadoDto(c.Id.ToString(), titulo.Texto, conteudo.Texto, Categoria(c.Categoria), publicadoEm,
            c.FonteOficialUrl, titulo.Exemplo || conteudo.Exemplo);
    }

    public static string Categoria(CategoriaComunicado c) => c switch
    {
        CategoriaComunicado.Coleta => "Coleta",
        CategoriaComunicado.Obras => "Obras",
        CategoriaComunicado.Manutencao => "Manutenção",
        CategoriaComunicado.Alerta => "Alerta",
        _ => "Geral"
    };

    private static string Data(DateOnly d) => d.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    private static string Hora(TimeOnly h) => h.ToString("HH:mm", CultureInfo.InvariantCulture);

    private static string Maiuscula(string t) => t.Length == 0 ? t : char.ToUpper(t[0], PtBr) + t[1..];
}
