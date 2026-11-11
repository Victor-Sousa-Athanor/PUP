using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pup.Api.Infra;
using Pup.Application;
using Pup.Application.Features.Inicializacao;
using Pup.Data;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

// Camadas: Controllers -> IDispatcher -> slices -> Services -> Repo/Query -> PostgreSQL
builder.Services.AddPupData(cfg.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("ConnectionStrings:Postgres não configurada."));
builder.Services.AddPupApplication(cfg);

// Autenticação só para a área administrativa; as consultas do portal são públicas.
builder.Services.Configure<JwtOptions>(cfg.GetSection(JwtOptions.Secao));
builder.Services.AddSingleton<TokenService>();
var jwt = cfg.GetSection(JwtOptions.Secao).Get<JwtOptions>() ?? new JwtOptions();
if (jwt.Chave.Length < 32)
    throw new InvalidOperationException("Jwt:Chave precisa ter pelo menos 32 caracteres (configure em appsettings ou user-secrets).");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Emissor,
            ValidateAudience = true,
            ValidAudience = jwt.Audiencia,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Chave)),
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });
builder.Services.AddAuthorization();

// O front-end (Blazor) roda em outra origem: libere as URLs dele em Cors:Origens.
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(cfg.GetSection("Cors:Origens").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.Services.AddExceptionHandler<TratamentoErros>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers()
    // Enums como texto no JSON ("EmAndamento" em vez de 2); números continuam sendo aceitos na entrada.
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

var app = builder.Build();

await InicializarAsync(app);

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
    app.MapOpenApi(); // /openapi/v1.json

// Atrás do Cloudflare Tunnel o HTTPS termina no túnel; em dev usamos http direto.
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

static async Task InicializarAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<InicializacaoService>().ExecutarAsync();
}

public partial class Program { }
