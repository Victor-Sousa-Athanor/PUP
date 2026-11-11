# Relatório — PUP-01: Primeiro build, banco no ar e smoke das rotas

## Arquivos alterados/criados

- `Directory.Packages.props` (atualizadas as versões de `Microsoft.Extensions.DependencyInjection.Abstractions` e `Microsoft.Extensions.Options` de `10.0.12` para `10.0.13`)
- `ferramentas/smoke.ps1` (criado)
- `IA-contexto/Claude/Tarefas/00_FILA.md` (status da PUP-01 marcado como **Feito**)
- `IA-contexto/Claude/Tarefas/2026-11-11_PUP-01_primeiro-build-RELATORIO.md` (este relatório)

## Erros encontrados e como foram resolvidos

1. **Erro `NU1605` (Package downgrade) no `dotnet restore`/`dotnet build`**
   - **Onde:** `Pup.Application` e `Pup.Tests`.
   - **Causa:** `Microsoft.Extensions.Options.ConfigurationExtensions` (`10.0.13`) exige `Microsoft.Extensions.DependencyInjection.Abstractions` (`>= 10.0.13`) e `Microsoft.Extensions.Options` (`>= 10.0.13`), mas o `Directory.Packages.props` fixava ambos em `10.0.12`.
   - **Resolução:** Atualizados `Microsoft.Extensions.DependencyInjection.Abstractions` e `Microsoft.Extensions.Options` para `10.0.13` em `Directory.Packages.props`.

2. **Código C# e consultas EF**
   - Após o ajuste no `Directory.Packages.props`, toda a solução compilou sem erros (`0 Error(s)`), os 20 testes de `Pup.Tests` passaram (`20/20`) e todas as 11 rotas GET + `POST /api/auth/login` do `ferramentas/smoke.ps1` responderam `200 OK` na primeira execução com o PostgreSQL no Docker (porta `5433`).

## Warnings observados

- **`MSB3277` em `Pup.Application` e `Pup.Tests`:**
  - `Found conflicts between different versions of "Microsoft.EntityFrameworkCore.Relational" that could not be resolved.`
  - Motivo: conflito entre a versão `10.0.4.0` (trazida transitivamente por `Npgsql.EntityFrameworkCore.PostgreSQL` `10.0.3`) e `10.0.12.0` (unificada com `EfVersion = 10.0.12` / `Pup.Data`). O MSBuild escolheu `10.0.4.0` como primária.
  - Em execução, o `EnsureCreated`, o seed inicial e todas as consultas funcionaram sem exceções, mas vale alinhar `EfVersion`/`Npgsql.EntityFrameworkCore.PostgreSQL` futuramente para eliminar o aviso.

## Decisões tomadas

- Inicializado o repositório git em `main` com o commit inicial `"Código inicial do back-end (Claude, sem compilar)"`.
- `README.md` não precisou de alterações porque os comandos da seção "Como rodar" funcionaram exatamente como descritos.

## O que o João deve conferir

- Confirmar o commit inicial e o commit `PUP-01: primeiro build e smoke` no histórico da `main`.
- Avaliar oportunamente o warning `MSB3277` de versão entre `Microsoft.EntityFrameworkCore` (`10.0.12`) e `Npgsql.EntityFrameworkCore.PostgreSQL` (`10.0.3` → `Relational 10.0.4`).
