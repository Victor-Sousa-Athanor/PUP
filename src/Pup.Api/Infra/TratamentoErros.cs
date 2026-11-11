using Microsoft.AspNetCore.Diagnostics;
using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Api.Infra;

/// <summary>Converte exceções de negócio em respostas HTTP com mensagem amigável ({ "mensagem": "..." }).</summary>
public sealed class TratamentoErros(ILogger<TratamentoErros> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception ex, CancellationToken ct)
    {
        var (status, mensagem) = ex switch
        {
            DomainException d => (StatusCodes.Status400BadRequest, d.Message),
            NaoEncontradoException n => (StatusCodes.Status404NotFound, n.Message),
            NaoAutenticadoException a => (StatusCodes.Status401Unauthorized, a.Message),
            AcessoNegadoException a => (StatusCodes.Status403Forbidden, a.Message),
            _ => (StatusCodes.Status500InternalServerError, "Erro inesperado. Tente novamente.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(ex, "Erro não tratado em {Path}", http.Request.Path);

        http.Response.StatusCode = status;
        await http.Response.WriteAsJsonAsync(new ErroResponse(mensagem), ct);
        return true;
    }
}
