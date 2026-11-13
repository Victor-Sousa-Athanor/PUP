# AGENTS.md — Portal de Utilidades Públicas (PUP)

Back-end do Projeto Integrador II (UNIVESP): API que centraliza informações de coleta de lixo, obras,
manutenção e comunicados de São Francisco Xavier (São José dos Campos - SP).

## Regras (sempre)
- C# / .NET 10. **Sem Python.** Datas e números gravados/lidos com `CultureInfo.InvariantCulture`.
- **Arquivos C# com no máximo 110 linhas.** Classes que crescerem devem ser divididas em `partial class` (`Classe.Assunto.cs`) — nunca crie classes gigantes com múltiplas responsabilidades.
- **Fluxo obrigatório:** Controller → `<Entidade>Service` → `<Entidade>Repo` (escrita/exclusão) / `<Entidade>Query` (leitura simples) → `AppDbContext`.
- **Nenhum Controller acessa Repo, Query ou `AppDbContext`** — Controllers chamam apenas Services.
- **Somente `*Repo` e `*Query` (`Pup.Data`) acessam o `AppDbContext`.**
- **Um trio `Service` + `Repo` + `Query` por entidade do banco.**
- **Services nunca consomem Repos ou Queries de outras entidades** — comunicação entre entidades acontece apenas entre Services.
- **Regras de negócio ficam nos Services (e invariantes de domínio chamadas por eles)** — nunca em `*Query` ou `*Repo` (`Query` apenas consulta sua tabela; `Repo` apenas salva/remove).
- Todo registro publicado tem `AtualizadoEm` e, quando houver, `FonteOficialUrl` (o portal complementa os canais oficiais, não os substitui).
- Dado inventado (seed) começa com "[Exemplo]".
- Nomes, comentários e textos em **português**. Comentário explica o *porquê*.

## Mapa rápido
| Onde | O quê |
|---|---|
| `src\Pup.Domain` | Entidades e invariantes (sem dependências) |
| `src\Pup.Contracts` | DTOs/requests da API e `Rotulos` (parcial por assunto) |
| `src\Pup.Data\<Entidade>\` | `<Entidade>Repo`, `<Entidade>Query` (1 entidade por pasta) |
| `src\Pup.Application\Features\<Entidade>\` | `<Entidade>Service` (`partial class` por assunto quando > 110 linhas) |
| `src\Pup.Api\Controllers` | Rotas HTTP (chamam apenas Services) |

## Glossário
Intervenção = obra **ou** manutenção (campo `Tipo`) · Histórico = linhas gravadas a cada mudança de situação ·
Coleta alterada = mudança temporária no cronograma (some sozinha depois de `AlteracaoAte`) ·
Comunicado vigente = sem `ValidoAte` ou com `ValidoAte` >= hoje (horário de Brasília).
