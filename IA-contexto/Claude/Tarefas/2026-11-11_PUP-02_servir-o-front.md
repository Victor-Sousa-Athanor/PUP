# PUP-02 — A API serve os arquivos do front

**Depende de: PUP-01**
Relatório: `2026-11-11_PUP-02_servir-o-front-RELATORIO.md`

## Objetivo

Abrir `http://localhost:5200/` mostra o site do front, servido pela própria API a partir da pasta `FrontEnzo/`
(cópia do front da dupla dentro deste repositório). Assim front e API ficam na **mesma origem**: o JavaScript chama
`/api/...` sem CORS e um único túnel do Cloudflare expõe tudo.

Lembrete da regra do `00_LEIA-PRIMEIRO.md`: o nome `FrontEnzo` é interno e **nunca** aparece para o usuário
(nem em comentário, README ou commit). Só arquivos do site são servidos.

## Arquivos

- `src/Pup.Api/Program.cs`
- `src/Pup.Api/appsettings.json`
- `src/Pup.Api/appsettings.Development.json`
- `src/Pup.Api/Properties/launchSettings.json`
- `ferramentas/smoke.ps1`
- `README.md` (nova seção "Front-end")

## O que fazer

1. Confira que `FrontEnzo/index.html` existe (a pasta já vem no repositório). **Não clone nada** e não altere
   nenhum arquivo de `FrontEnzo/` nesta tarefa.
2. `appsettings.json`: acrescente a seção (vazia = a API não serve front):
   ```json
   "Front": {
     "Pasta": ""
   },
   ```
3. `appsettings.Development.json`: acrescente `"Front": { "Pasta": "../../FrontEnzo" }`
   (relativo à pasta do projeto `src/Pup.Api`, que é o ContentRoot no `dotnet run` e no Visual Studio).
   Na mesma seção `Cors:Origens`, acrescente `"http://localhost:5500"` e `"http://127.0.0.1:5500"` (para quem rodar
   o front num servidor estático separado na porta 5500).
4. `Program.cs`: no topo, `using Microsoft.AspNetCore.StaticFiles;` e `using Microsoft.Extensions.FileProviders;`.
   Depois de `app.UseExceptionHandler();` e **antes** de `app.UseCors();`, acrescente exatamente:
   ```csharp
   // Front estático (pasta configurada em Front:Pasta). Servido pela API: mesma origem, sem CORS e um único túnel.
   var pastaFront = cfg["Front:Pasta"];
   if (!string.IsNullOrWhiteSpace(pastaFront))
   {
       var caminhoFront = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, pastaFront));
       if (Directory.Exists(caminhoFront))
       {
           var arquivosFront = new PhysicalFileProvider(caminhoFront);

           // Só tipos de arquivo do site. Arquivos internos da pasta (README.md, ORIGEM.md...) nunca são servidos.
           var tiposDoSite = new FileExtensionContentTypeProvider();
           tiposDoSite.Mappings.Clear();
           tiposDoSite.Mappings[".html"] = "text/html; charset=utf-8";
           tiposDoSite.Mappings[".css"] = "text/css; charset=utf-8";
           tiposDoSite.Mappings[".js"] = "text/javascript; charset=utf-8";
           tiposDoSite.Mappings[".svg"] = "image/svg+xml";
           tiposDoSite.Mappings[".png"] = "image/png";
           tiposDoSite.Mappings[".jpg"] = "image/jpeg";
           tiposDoSite.Mappings[".jpeg"] = "image/jpeg";
           tiposDoSite.Mappings[".webp"] = "image/webp";
           tiposDoSite.Mappings[".ico"] = "image/x-icon";
           tiposDoSite.Mappings[".woff2"] = "font/woff2";

           app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = arquivosFront });
           app.UseStaticFiles(new StaticFileOptions { FileProvider = arquivosFront, ContentTypeProvider = tiposDoSite });
           app.Logger.LogInformation("Front estático servido a partir de {Caminho}", caminhoFront);
       }
       else
       {
           app.Logger.LogWarning("Front:Pasta não encontrada ({Caminho}). A API sobe sem o front.", caminhoFront);
       }
   }
   ```
   (O caminho só aparece no log do console do servidor, nunca na resposta HTTP.)
5. `launchSettings.json`: nos dois perfis, troque `"launchUrl": "openapi/v1.json"` por `"launchUrl": ""`
   (o navegador abre o site).
6. `ferramentas/smoke.ps1`:
   - acrescente ao array `$rotas` as linhas `"/"`, `"/coleta.html"`, `"/obras.html"`, `"/manutencao.html"`,
     `"/comunicados.html"` e `"/assets/js/data.js"`;
   - **antes** do bloco final (`if ($falhas -gt 0) ...`), acrescente exatamente:
   ```powershell
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
   ```
7. `README.md`: seção "Front-end" (sem citar o nome da pasta): o front é uma cópia do repositório da dupla guardada
   neste repositório; a API serve os arquivos dessa pasta, configurada em `Front:Pasta` (relativa ao projeto, ou
   caminho absoluto em produção); só arquivos do site são servidos; `http://localhost:5200/` abre o site; o túnel do
   Cloudflare expõe site e API juntos.
8. Commit `PUP-02: API serve o front`.

## Não mexa

- Arquivos de `FrontEnzo/` (a integração é a PUP-05).
- Rotas `/api/...`, autenticação e regras.

## Pronto quando

- [ ] `http://localhost:5200/` abre a página inicial com CSS e JS carregando (sem 404 no F12 → Rede)
- [ ] As 5 páginas abrem pelos links do menu
- [ ] `http://localhost:5200/ORIGEM.md` e `/README.md` dão 404
- [ ] `/api/painel` continua respondendo
- [ ] Build, testes e smoke verdes (incluindo a checagem do nome interno)
