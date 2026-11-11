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
