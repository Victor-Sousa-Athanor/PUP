# Relatório — PUP-02: A API serve os arquivos do front

## Arquivos alterados/criados

- `src/Pup.Api/Program.cs` (adicionados `using Microsoft.AspNetCore.StaticFiles;` e `using Microsoft.Extensions.FileProviders;` e middleware de arquivos padrão/estáticos restrito aos tipos do site)
- `src/Pup.Api/appsettings.json` (seção `"Front": { "Pasta": "" }`)
- `src/Pup.Api/appsettings.Development.json` (seção `"Front:Pasta"` apontando para a pasta do front e origens `:5500` em `Cors:Origens`)
- `src/Pup.Api/Properties/launchSettings.json` (`"launchUrl": ""` nos dois perfis)
- `ferramentas/smoke.ps1` (rotas do site, verificação 404 de `/ORIGEM.md` e `/README.md` e verificação de ausência do nome interno nas rotas públicas)
- `README.md` (adicionada seção "Front-end")
- `IA-contexto/Claude/Tarefas/00_FILA.md` (status da PUP-02 marcado como **Feito**)
- `IA-contexto/Claude/Tarefas/2026-11-11_PUP-02_servir-o-front-RELATORIO.md` (este relatório)

## Erros encontrados e como foram resolvidos

- Nenhum erro de compilação ou execução.
- `dotnet build Pup.slnx`: 0 erros (apenas os 2 warnings `MSB3277` já registrados na PUP-01).
- `dotnet test tests/Pup.Tests`: 20/20 passando.
- `ferramentas/smoke.ps1`: todas as rotas retornaram `200`, `/ORIGEM.md` e `/README.md` retornaram `404` e a verificação de não exposição do nome interno da pasta passou em todas as páginas, CSS, JS e `/api/painel`.

## Decisões tomadas

- Posicionado o bloco de `UseDefaultFiles`/`UseStaticFiles` depois de `UseHttpsRedirection()` (mantendo-o depois de `UseExceptionHandler()` e antes de `UseCors()`), para que em produção o redirecionamento HTTPS ocorra antes da entrega dos arquivos estáticos.
- Mantidos comentários, logs e `README.md` sem citar o nome interno da pasta do front (apenas no valor de configuração de desenvolvimento e dividido em `"Front" + "Enzo"` no script de smoke).

## O que o João deve conferir

- Abrir `http://localhost:5200/` com a API rodando e confirmar no F12 → Rede que todas as páginas do menu e seus `.css`/`.js` carregam com 200.
- Conferir no navegador que `http://localhost:5200/ORIGEM.md` e `/README.md` retornam 404.
