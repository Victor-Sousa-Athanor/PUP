# PUP-01 — Primeiro build, banco no ar e smoke das rotas

**Depende de: nada**
Relatório: `2026-11-11_PUP-01_primeiro-build-RELATORIO.md`

## Objetivo

A solução compila pela primeira vez, os testes passam, a API sobe com o Postgres e todas as rotas GET respondem
200. Sem mudar comportamento.

## Arquivos

- Qualquer arquivo de `src/` e `tests/` **só para corrigir erro de compilação ou erro de execução** (menor mudança possível)
- `Directory.Packages.props`
- `ferramentas/smoke.ps1` (novo)
- `README.md` (só a seção "Como rodar", se algum comando mudar)

## O que fazer

1. Repositório git (ainda não existe):
   `git init -b main`, `git add -A`, `git commit -m "Código inicial do back-end (Claude, sem compilar)"`.
2. Ambiente: `dotnet --list-sdks` precisa ter um SDK 10 (o `global.json` aceita qualquer 10.0.1xx ou maior).
3. **Erro conhecido** (aconteceu igual no ServiZap, mesmas versões): `NU1605` (package downgrade) em
   `Pup.Application`. Correção: no `Directory.Packages.props`, troque `Microsoft.Extensions.DependencyInjection.Abstractions`
   e `Microsoft.Extensions.Options` de `10.0.12` para `10.0.13`.
4. `dotnet restore` e `dotnet build Pup.slnx`. Corrija os erros com a menor mudança possível: ajuste a sintaxe,
   não reescreva a lógica nem renomeie classes/métodos públicos.
5. `dotnet test tests/Pup.Tests`. Teste falhando: o erro está no código, não no teste (só mude o teste se ele
   estiver claramente contradizendo a regra escrita no comentário da classe testada, e explique no relatório).
6. `docker compose up -d` (Postgres na porta **5433**) e `dotnet run --project src/Pup.Api`.
   Na primeira subida a API cria o schema (`EnsureCreated`), o admin de desenvolvimento e os dados de exemplo.
7. Crie `ferramentas/smoke.ps1` exatamente assim e rode com a API no ar:

```powershell
# Smoke do PUP: chama as rotas principais e falha (exit 1) se alguma não responder 2xx.
param([string]$Api = "http://localhost:5200")

$rotas = @(
    "/health",
    "/api/painel",
    "/api/busca?termo=coleta",
    "/api/regioes",
    "/api/coletas",
    "/api/coletas?hoje=true",
    "/api/pontos-descarte",
    "/api/intervencoes",
    "/api/obras?abertas=true",
    "/api/manutencoes",
    "/api/comunicados"
)

$falhas = 0
foreach ($rota in $rotas) {
    try {
        $resposta = Invoke-WebRequest -Uri ($Api + $rota) -UseBasicParsing -TimeoutSec 20
        Write-Host ("OK     {0}  {1}" -f $resposta.StatusCode, $rota) -ForegroundColor Green
    }
    catch {
        $falhas++
        Write-Host ("FALHA  {0}  ->  {1}" -f $rota, $_.Exception.Message) -ForegroundColor Red
    }
}

# Login da área administrativa (credenciais do appsettings.Development.json)
try {
    $corpo = '{"email":"admin@pup.local","senha":"Admin@12345"}'
    $login = Invoke-RestMethod -Uri ($Api + "/api/auth/login") -Method Post -ContentType "application/json" -Body $corpo
    if ([string]::IsNullOrWhiteSpace($login.token)) { throw "login sem token" }
    Write-Host "OK     login do admin" -ForegroundColor Green
}
catch {
    $falhas++
    Write-Host ("FALHA  login  ->  {0}" -f $_.Exception.Message) -ForegroundColor Red
}

if ($falhas -gt 0) { Write-Host "$falhas falha(s)." -ForegroundColor Red; exit 1 }
Write-Host "Smoke verde." -ForegroundColor Green
```

8. Rota com 500: leia o log da API no console. Os pontos mais prováveis são consultas EF que não traduzem para SQL
   (`*Query.cs` em `src/Pup.Data`). Corrija a consulta mantendo o DTO devolvido igual. Se o banco já tiver sido criado
   com um schema errado, recrie: `docker compose down -v` e `docker compose up -d`.
9. Commit `PUP-01: primeiro build e smoke` (código, `ferramentas/smoke.ps1`, relatório e fila).

## Não mexa

- Regras de negócio, textos, rotas, nomes públicos de classes e métodos.
- Não crie migrations nesta tarefa.

## Pronto quando

- [ ] `dotnet build Pup.slnx` sem erros
- [ ] `dotnet test tests/Pup.Tests` todos passando
- [ ] `ferramentas\smoke.ps1` verde com a API no ar
- [ ] Relatório com cada erro encontrado e como foi resolvido, mais os warnings que parecem bug
