using Microsoft.AspNetCore.Mvc;
using Pup.Api.Infra;
using Pup.Application.Abstracoes;
using Pup.Application.Features.Acesso;
using Pup.Contracts;

namespace Pup.Api.Controllers;

/// <summary>Login da área administrativa. O portal público não exige login.</summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController(IDispatcher dispatcher, TokenService tokens) : ControllerBase
{
    [HttpPost("login")]
    public async Task<TokenResponse> Login(LoginRequest r, CancellationToken ct) =>
        tokens.Gerar(await dispatcher.SendAsync(new Entrar.Command(r.Email, r.Senha), ct));
}
