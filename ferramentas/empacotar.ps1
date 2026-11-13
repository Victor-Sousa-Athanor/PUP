# Empacota o PUP para outro PC Windows (executavel autocontido, front estatico, zip e instalador Inno Setup se disponivel).
param(
    [string]$Saida = "dist",
    [int]$Porta = 5200
)

$ErrorActionPreference = "Stop"

$raiz = Split-Path -Parent $PSScriptRoot
$destinoBase = Join-Path $raiz $Saida
$pastaPacote = Join-Path $destinoBase "pup-win-x64"
$pastaFrontDestino = Join-Path $pastaPacote "front"
$caminhoZip = Join-Path $destinoBase "pup-win-x64.zip"

# Descobre a pasta do front a partir da configuracao de desenvolvimento sem expor o nome em arquivos publicos.
$cfgDev = Get-Content (Join-Path $raiz "src\Pup.Api\appsettings.Development.json") -Raw | ConvertFrom-Json
$pastaFrontRelativa = $cfgDev.Front.Pasta
if ([string]::IsNullOrWhiteSpace($pastaFrontRelativa)) {
    throw "Front:Pasta nao configurada em src/Pup.Api/appsettings.Development.json."
}
$pastaFrontOrigem = [System.IO.Path]::GetFullPath((Join-Path (Join-Path $raiz "src\Pup.Api") $pastaFrontRelativa))
if (-not (Test-Path $pastaFrontOrigem)) {
    throw "Pasta do front nao encontrada em '$pastaFrontOrigem'."
}

if (Test-Path $pastaPacote) {
    Remove-Item -Recurse -Force $pastaPacote
}
New-Item -ItemType Directory -Force -Path $pastaPacote | Out-Null

Write-Host "1/4 Publicando Pup.Api autocontido (win-x64)..." -ForegroundColor Cyan
dotnet publish (Join-Path $raiz "src\Pup.Api\Pup.Api.csproj") `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $pastaPacote
if ($LASTEXITCODE -ne 0) {
    throw "Falha no dotnet publish."
}

Write-Host "2/4 Copiando apenas arquivos web do front para 'front/'..." -ForegroundColor Cyan
New-Item -ItemType Directory -Force -Path $pastaFrontDestino | Out-Null
$extensoesPermitidas = @(".html", ".css", ".js", ".svg", ".png", ".jpg", ".jpeg", ".webp", ".ico", ".woff2")
Get-ChildItem -Path $pastaFrontOrigem -Recurse -File | Where-Object {
    $extensoesPermitidas -contains $_.Extension.ToLowerInvariant()
} | ForEach-Object {
    $relativo = $_.FullName.Substring($pastaFrontOrigem.Length).TrimStart('\', '/')
    $alvo = Join-Path $pastaFrontDestino $relativo
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $alvo) | Out-Null
    Copy-Item $_.FullName -Destination $alvo -Force
}

Write-Host "3/4 Gerando configuracao de producao e atalho de inicializacao..." -ForegroundColor Cyan
Copy-Item (Join-Path $raiz "docker-compose.yml") -Destination (Join-Path $pastaPacote "docker-compose.yml") -Force
Copy-Item (Join-Path $PSScriptRoot "appsettings.Production.exemplo.json") -Destination (Join-Path $pastaPacote "appsettings.Production.exemplo.json") -Force

# Gera chave JWT aleatoria de 48 caracteres para evitar falha na primeira execucao fora de dev.
$bytes = New-Object byte[] 36
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
$chaveAleatoria = [Convert]::ToBase64String($bytes)

$cfgProd = Get-Content (Join-Path $PSScriptRoot "appsettings.Production.exemplo.json") -Raw | ConvertFrom-Json
$cfgProd.Jwt.Chave = $chaveAleatoria
$cfgProd | ConvertTo-Json -Depth 5 | Set-Content -Path (Join-Path $pastaPacote "appsettings.Production.json") -Encoding UTF8

@"
@echo off
setlocal
cd /d "%~dp0"
set ASPNETCORE_ENVIRONMENT=Production
set ASPNETCORE_URLS=http://localhost:$Porta
echo Iniciando Portal de Utilidades Publicas em http://localhost:$Porta ...
echo Mantenha esta janela aberta enquanto o portal estiver no ar.
"%~dp0Pup.Api.exe"
"@ | Set-Content -Path (Join-Path $pastaPacote "iniciar-pup.bat") -Encoding ASCII

@"
Portal de Utilidades Publicas (PUP) - Pacote autocontido
=========================================================

Pre-requisito no PC de destino:
- PostgreSQL 17 (com Docker Desktop usando 'docker compose up -d' nesta pasta,
  ou PostgreSQL nativo no Windows ajustando a porta para 5432 em appsettings.Production.json).

Como executar:
1. Revise 'appsettings.Production.json' (troque a senha de AdminInicial antes de expor).
2. Execute 'iniciar-pup.bat'.
3. Abra http://localhost:$Porta/ no navegador.
4. Para expor pela internet: cloudflared tunnel --url http://localhost:$Porta
"@ | Set-Content -Path (Join-Path $pastaPacote "LEIA-ME.txt") -Encoding UTF8

Write-Host "4/4 Compactando pacote ZIP..." -ForegroundColor Cyan
if (Test-Path $caminhoZip) {
    Remove-Item -Force $caminhoZip
}
Compress-Archive -Path (Join-Path $pastaPacote "*") -DestinationPath $caminhoZip -CompressionLevel Optimal
Write-Host "Pacote ZIP gerado em: $caminhoZip" -ForegroundColor Green

$iscc = Get-Command "iscc" -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source
if (-not $iscc) {
    $candidatosInno = @(
        (Join-Path $env:LOCALAPPDATA "Programs\Inno Setup 6\ISCC.exe"),
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles}\Inno Setup 6\ISCC.exe"
    )
    foreach ($cand in $candidatosInno) {
        if ($cand -and (Test-Path $cand)) {
            $iscc = $cand
            break
        }
    }
}

if ($iscc) {
    Write-Host "Inno Setup detectado. Compilando PUP-Setup.exe..." -ForegroundColor Cyan
    & $iscc (Join-Path $PSScriptRoot "instalador.iss")
    if ($LASTEXITCODE -ne 0) { throw "Falha ao gerar PUP-Setup.exe no Inno Setup." }
    Write-Host "Instalador gerado em: $(Join-Path $destinoBase 'PUP-Setup.exe')" -ForegroundColor Green
} else {
    Write-Host "Inno Setup nao encontrado. Para gerar PUP-Setup.exe unico, instale via 'winget install --id JRSoftware.InnoSetup' e execute este script novamente." -ForegroundColor Yellow
}
