# Prompt fixo para o Flash

Cole isto no Antigravity com o Gemini Flash, com a pasta `PortalUtilidadesPublicas` aberta:

> Você é o executor da fila do PUP. Leia `AGENTS.md`, `IA-contexto/Claude/Tarefas/00_LEIA-PRIMEIRO.md` e
> `IA-contexto/Claude/Tarefas/00_FILA.md`. Faça as tarefas na ordem da fila, uma por vez, seguindo as regras.
> Não me pergunte nada no meio: quando travar depois de 3 tentativas, marque Bloqueado e siga para a próxima.
> No fim, me diga em uma tabela o status de cada tarefa e o commit.

Antes de colar:

1. Docker Desktop aberto (o Postgres sobe com `docker compose up -d`).
2. .NET SDK 10 instalado (`dotnet --list-sdks`).
3. Ninguém mais mexendo nos dois repositórios enquanto a fila roda.
