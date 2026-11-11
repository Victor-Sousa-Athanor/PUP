# Smoke do PUP: chama as rotas principais e falha (exit 1) se alguma não responder 2xx.
param([string]$Api = "http://localhost:5200")

$rotas = @(
    "/",
    "/coleta.html",
    "/obras.html",
    "/manutencao.html",
    "/comunicados.html",
    "/assets/js/data.js",
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
    "/api/comunicados",
    "/api/portal/servicos",
    "/api/portal/comunicados"
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

# Arquivos internos da pasta do front nunca são públicos (precisam dar erro, 404)
foreach ($interno in @("/ORIGEM.md", "/README.md")) {
    try {
        Invoke-WebRequest -Uri ($Api + $interno) -UseBasicParsing -TimeoutSec 20 | Out-Null
        $falhas++
        Write-Host ("FALHA  {0} está público" -f $interno) -ForegroundColor Red
    }
    catch {
        Write-Host ("OK     {0} bloqueado" -f $interno) -ForegroundColor Green
    }
}

# O nome interno da pasta do front nunca aparece para o usuário (montado em duas partes de propósito)
$nomeInterno = "Front" + "Enzo"
$publicas = @(
    "/", "/coleta.html", "/obras.html", "/manutencao.html", "/comunicados.html",
    "/assets/css/app.css", "/assets/js/data.js", "/assets/js/app.js", "/assets/js/components.js",
    "/assets/js/pages/home.js", "/assets/js/pages/services.js", "/assets/js/pages/announcements.js",
    "/api/painel"
)
foreach ($publica in $publicas) {
    try {
        $conteudo = (Invoke-WebRequest -Uri ($Api + $publica) -UseBasicParsing -TimeoutSec 20).Content
        if ($conteudo -match $nomeInterno) {
            $falhas++
            Write-Host ("FALHA  {0} mostra o nome interno da pasta" -f $publica) -ForegroundColor Red
        }
    }
    catch {
        $falhas++
        Write-Host ("FALHA  {0}  ->  {1}" -f $publica, $_.Exception.Message) -ForegroundColor Red
    }
}
Write-Host "OK     nome interno não aparece nas páginas públicas" -ForegroundColor Green

if ($falhas -gt 0) { Write-Host "$falhas falha(s)." -ForegroundColor Red; exit 1 }
Write-Host "Smoke verde." -ForegroundColor Green
