# PUP — tarefas para o Gemini Flash (leia primeiro)

Escritas pelo Claude em 11/11/2026. Objetivo da leva: o back-end (`PortalUtilidadesPublicas`) compilar, rodar,
servir o front (cópia do front da dupla, na pasta `FrontEnzo/` deste repositório) e entregar os dados no formato
que esse front já usa.

## Contexto em 30 segundos

- O back foi escrito pelo Claude **sem compilar** (não havia SDK no ambiente dele). Espere erros pontuais.
- O front fica em `FrontEnzo/` (cópia de https://github.com/GabrielVilela23/ProjetoPI2-Front; veja `FrontEnzo/ORIGEM.md`).
  É HTML + CSS + JS puro, sem build e sem servidor. Hoje ele lê dados fixos de `assets/js/data.js`
  (`window.PUP.data.services` e `window.PUP.data.announcements`).
- Estratégia: a API passa a **servir os arquivos do front** (mesma origem, sem CORS) e ganha duas rotas
  (`/api/portal/servicos` e `/api/portal/comunicados`) que devolvem exatamente o formato do `data.js`.
  No front, só o `data.js`/`app.js` (e detalhes de texto) mudam para buscar da API, com os dados de demonstração como reserva.

## O nome `FrontEnzo` é interno (regra que vale para todas as tarefas)

`FrontEnzo` é só o nome da pasta no nosso repositório. **Ele nunca aparece para quem usa o site.**

- Nunca escreva `FrontEnzo` (em qualquer caixa) em: HTML, CSS, JS, títulos, textos de tela, mensagens da API,
  respostas JSON, comentários de código, README, mensagens de commit ou nomes de classes/rotas.
- Nos textos, chame de "o front", "pasta do front" ou "`Front:Pasta`".
- O nome só pode aparecer onde é inevitável: o valor de `Front:Pasta` no `appsettings.Development.json`, os caminhos
  de arquivo destas tarefas e o `ferramentas/smoke.ps1` (que verifica que o nome **não** vaza; lá ele é montado
  como `"Front" + "Enzo"`).
- A API serve **só** arquivos do site (`.html`, `.css`, `.js`, imagens e fontes). Arquivos internos da pasta
  (`README.md`, `ORIGEM.md` etc.) devolvem 404.

## Regras (valem para todas)

1. Siga a ordem da `00_FILA.md`. Uma tarefa por vez. Leia o `AGENTS.md` da raiz e o arquivo da tarefa antes de começar.
2. **Só crie ou altere os arquivos listados em "Arquivos" da tarefa.** Precisa mexer em outro? Explique no relatório.
3. Não apague arquivos. Não mexa em `bin`, `obj`, `.vs`. Não altere testes que já existem (só os que a tarefa mandar criar).
4. Nomes, comentários e textos em português. C# / .NET 10. Scripts em PowerShell. **Sem Python.**
5. Antes de entregar, rode:
   - `dotnet build Pup.slnx`
   - `dotnet test tests/Pup.Tests`
   - com a API no ar, `powershell -ExecutionPolicy Bypass -File ferramentas\smoke.ps1` (criado na PUP-01)
   Só entregue com build, testes e smoke passando. Se algo não passar depois de 3 tentativas de correção,
   marque **Bloqueado** na fila com o erro principal e siga para a próxima tarefa que não dependa dela.
6. Grave o relatório na mesma pasta: nome do arquivo da tarefa + `-RELATORIO.md`. Conteúdo: arquivos
   alterados, cada erro encontrado e como resolveu, decisões tomadas, o que o João deve conferir.
7. Commit na `main` com a mensagem `PUP-0x: <título>` (código + relatório + fila atualizada). Sem push.
8. Não sabe o nome exato de um arquivo, classe ou pasta? Procure no `Pup.slnx` e em `src/`. Não invente.
9. Nunca use `Skip`, `catch` vazio ou `#pragma warning disable` para fazer algo passar.
10. O repositório da dupla do front nunca é clonado, alterado ou recebe push. A integração acontece só na nossa cópia.

## Padrão do código (resumo do AGENTS.md)

Controller → `IDispatcher` → slice (`static class X { record Query/Command; class Handler }`) → `XService`
→ `XRepo` (escrita) / `XQuery` (leitura) → `AppDbContext`. Só `*Repo` e `*Query` usam o `AppDbContext`.
Regra de negócio no domínio com `DomainException`. Nas `*Query`, filtre e ordene sobre as entidades e projete
para DTO **por último** (o EF não traduz filtro feito depois de `new Record(...)`).
