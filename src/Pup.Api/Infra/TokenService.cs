using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Pup.Application.Features.Acesso;
using Pup.Contracts;

namespace Pup.Api.Infra;

public sealed class TokenService(IOptions<JwtOptions> options)
{
    public const string PapelAdmin = "Admin";

    public TokenResponse Gerar(AdministradorAutenticado admin)
    {
        var opt = options.Value;
        var expira = DateTime.UtcNow.AddHours(opt.HorasValidade);
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opt.Chave));

        var descritor = new SecurityTokenDescriptor
        {
            Issuer = opt.Emissor,
            Audience = opt.Audiencia,
            Expires = expira,
            SigningCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, admin.Email),
                new Claim(ClaimTypes.Name, admin.Nome),
                new Claim(ClaimTypes.Role, PapelAdmin)
            })
        };

        var token = new JsonWebTokenHandler().CreateToken(descritor);
        return new TokenResponse(token, expira, admin.Nome);
    }
}
