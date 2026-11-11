namespace Pup.Data.Comum;

internal static class BuscaTexto
{
    /// <summary>Monta o padrão do ILIKE escapando % e _ digitados pelo usuário.</summary>
    public static string Padrao(string termo) =>
        "%" + termo.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
}
