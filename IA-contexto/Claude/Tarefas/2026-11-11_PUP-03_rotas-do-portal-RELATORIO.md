# Relatório — PUP-03: Rotas no formato do front (`/api/portal/servicos` e `/api/portal/comunicados`)

## Arquivos alterados/criados

- `src/Pup.Contracts/Portal.cs` (criado)
- `src/Pup.Contracts/Intervencoes.cs` (acrescentado `string Descricao` após `string Titulo` em `IntervencaoResumoDto`)
- `src/Pup.Data/Intervencoes/IntervencaoQuery.cs` (preenchido `x.I.Descricao` em `Projetar`)
- `src/Pup.Application/Comum/Relogio.cs` (adicionado `DataLocal(DateTime utc)` em `IRelogio` e `Relogio`; `Hoje` passou a delegar para `DataLocal(DateTime.UtcNow)`)
- `src/Pup.Application/Features/Portal/MapeadorPortal.cs` (criado)
- `src/Pup.Application/Features/Portal/PortalService.cs` (criado)
- `src/Pup.Application/Features/Portal/PortalSlices.cs` (criado)
- `src/Pup.Application/DependencyInjection.cs` (registrado `PortalService`)
- `src/Pup.Api/Controllers/PortalController.cs` (criado)
- `src/Pup.Api/Pup.Api.http` (adicionadas as duas rotas do portal)
- `ferramentas/smoke.ps1` (adicionadas `/api/portal/servicos` e `/api/portal/comunicados`)
- `tests/Pup.Tests/MapeadorPortalTests.cs` (criado com os testes da tarefa)
- `IA-contexto/Claude/Tarefas/00_FILA.md` (status da PUP-03 marcado como **Feito**)
- `IA-contexto/Claude/Tarefas/2026-11-11_PUP-03_rotas-do-portal-RELATORIO.md` (este relatório)

## Erros encontrados e como foram resolvidos

1. **Erro `CS1061` em `MapeadorPortal.DeIntervencao` (`i.FonteOficialUrl`)**
   - **Onde:** `src/Pup.Application/Features/Portal/MapeadorPortal.cs` linha 97.
   - **Causa:** O snippet da tarefa referenciava `i.FonteOficialUrl` em `IntervencaoResumoDto`, mas `IntervencaoResumoDto` não possui `FonteOficialUrl` (e o teste `MapeadorPortalTests` instruído para cópia exata também instancia `IntervencaoResumoDto` com os 14 parâmetros existentes). Além disso, alterar o contrato de `IntervencaoResumoDto` mudaria o JSON das rotas já existentes (`/api/intervencoes`, `/api/obras`, `/api/manutencoes`), o que a seção "Não mexa" proíbe.
   - **Resolução:** Passado `null` como `FonteOficialUrl` em `MapeadorPortal.DeIntervencao`, mantendo o `IntervencaoResumoDto` intacto (apenas com o novo campo `Descricao`) e o `MapeadorPortalTests.cs` exatamente como especificado.

## Decisões tomadas

- Em `PortalService.ServicosAsync`, as três consultas (`coletas`, `pontos`, `intervencoes`) são executadas em sequência por compartilharem o mesmo `AppDbContext`, e a lista consolidada é ordenada por `AtualizadoEm` decrescente e depois por `Titulo`.
- Todos os comentários, nomes e textos novos foram mantidos neutros, sem citar o nome interno da pasta do front.

## Verificações realizadas

- `dotnet build Pup.slnx`: 0 erros (apenas os 2 warnings `MSB3277` pré-existentes).
- `dotnet test tests/Pup.Tests`: 37/37 passando (20 existentes + 17 novos em `MapeadorPortalTests`).
- `ferramentas/smoke.ps1`: verde (`200 OK` em `/api/portal/servicos` e `/api/portal/comunicados`), confirmando retorno JSON com chaves `id`, `title`, `category`, `description`/`message`, `location`, `status`, `updatedAt`/`publishedAt` (`yyyy-MM-dd`), `schedule`, `officialSourceUrl` e `isDemo`.

## O que o João deve conferir

- Confirmar se está de acordo em manter `officialSourceUrl: null` nas intervenções do `/api/portal/servicos` para não alterar o contrato existente do `IntervencaoResumoDto` (caso futuramente deseje expor a fonte oficial das obras/manutenções também no resumo do portal, basta adicionar `FonteOficialUrl` ao `IntervencaoResumoDto` e à projeção da `IntervencaoQuery`).
