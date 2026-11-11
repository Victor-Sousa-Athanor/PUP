# PUP-05 — O front passa a ler da API

**Depende de: PUP-02, PUP-03, PUP-04**
Relatório: `2026-11-11_PUP-05_front-consome-a-api-RELATORIO.md`

## Objetivo

A cópia do front em `FrontEnzo/` (dentro deste repositório) troca os dados fixos do `data.js` pelas rotas
`/api/portal/servicos` e `/api/portal/comunicados`. As telas, o CSS e os componentes continuam iguais.
Se a API não responder (por exemplo, `index.html` aberto direto do disco), o site continua funcionando com os dados
de demonstração que já estão no `data.js`.

- A pasta é **nossa cópia**: o repositório da dupla não é alterado. Commit normal na `main` do PUP.
- Não mexa no `FrontEnzo/ORIGEM.md`.
- Regra do `00_LEIA-PRIMEIRO.md`: o nome `FrontEnzo` **nunca** aparece em nada que o usuário vê (HTML, JS, CSS,
  títulos, mensagens de console) nem em comentários, README ou commit.

## Arquivos

Dentro de `FrontEnzo/`:
- `assets/js/data.js`
- `assets/js/app.js`
- `assets/js/components.js`
- `assets/js/pages/home.js`
- `assets/js/pages/services.js`
- `assets/js/pages/announcements.js`
- `coleta.html`, `obras.html`, `manutencao.html`, `comunicados.html` (só as opções dos `<select>`)
- `README.md` (nova seção "Executar com a API")

Fora dela:
- `ferramentas/smoke.ps1` (só acrescentar rotas à lista `$publicas`)

## O que fazer

Mantenha o estilo da dupla: JS puro, IIFE com `window.PUP`, `textContent` para dados (nunca `innerHTML`),
sem dependências.

### 1. `data.js`

- Renomeie `const services = [` para `const demoServices = [` e `const announcements = [` para
  `const demoAnnouncements = [`. Não mexa nos itens.
- Troque as três últimas linhas (`root.PUP = ...`, `root.PUP.data = ...`, `root.PUP.utils = ...`) por:

```js
  // Onde está a API. Servido pela própria API (padrão): mesma origem, caminho relativo.
  // Aberto do disco (file://) ou num servidor estático na porta 5500: API local na porta 5200.
  function apiBase() {
    if (typeof root.PUP_API_BASE === "string") return root.PUP_API_BASE;
    const local = root.location.protocol === "file:" || root.location.port === "5500";
    return local ? "http://localhost:5200" : "";
  }
  async function getJson(path) {
    const response = await fetch(apiBase() + path, { headers: { Accept: "application/json" } });
    if (!response.ok) throw new Error(`HTTP ${response.status} em ${path}`);
    return response.json();
  }
  const data = { categories, services: demoServices, announcements: demoAnnouncements, source: "demo" };
  // Busca os dados reais; se a API não responder, ficam os dados de demonstração acima.
  async function load() {
    try {
      const [services, announcements] = await Promise.all([getJson("/api/portal/servicos"), getJson("/api/portal/comunicados")]);
      data.services = services;
      data.announcements = announcements;
      data.source = "api";
    } catch (error) {
      console.warn("PUP: API indisponível, usando os dados de demonstração.", error);
    }
  }
  root.PUP = root.PUP || {};
  root.PUP.data = data;
  root.PUP.utils = { normalize, formatDate };
  root.PUP.ready = load();
```

### 2. `app.js` — esperar os dados antes de montar a página

Troque `function start() {` por `async function start() {` e, logo antes de `const page = document.body.dataset.page;`,
acrescente:

```js
    if (root.PUP.ready) await root.PUP.ready;
```

(o menu móvel continua sendo ligado antes, sem esperar a API).

### 3. `services.js` — localidades vindas dos dados

Logo depois de `const category = document.body.dataset.category;`, acrescente:

```js
    // Localidades vêm dos dados (API ou demonstração), não fixas no HTML.
    const locations = [...new Set(root.PUP.data.services.filter((item) => item.category === category).map((item) => item.location))]
      .sort((a, b) => a.localeCompare(b, "pt-BR"));
    location.replaceChildren(new Option("Todas as localidades", ""), ...locations.map((value) => new Option(value, value)));
```

### 4. Textos de "demonstração" só quando os dados forem de demonstração

- `services.js`: `· demonstração` só se `root.PUP.data.source !== "api"`.
- `home.js`: `· exemplos demonstrativos` só se `root.PUP.data.source !== "api"`.
- `announcements.js`: `· todos demonstrativos` só se `root.PUP.data.source !== "api"`.
- `components.js` (`announcementItem`): no modo compacto, o selo `Demonstração` só aparece se `item.isDemo` for verdadeiro
  (no modo normal continua `makeSource(item)`).
- Os avisos amarelos fixos no topo das páginas ("Protótipo acadêmico...") **ficam** — o projeto continua sendo um protótipo.

### 5. Opções dos `<select>`

- `coleta.html`, `obras.html`, `manutencao.html`: no `#service-status`, acrescente `<option>Cancelado</option>` depois de `Alterado`.
- `comunicados.html`: no `#announcement-category`, acrescente `<option>Alerta</option>` antes de `Geral`.

### 6. `README.md` da pasta do front — seção "Executar com a API"

Em poucas linhas, sem citar o nome da pasta: com o back (`PortalUtilidadesPublicas`) rodando, acesse
`http://localhost:5200/` (a API serve estes arquivos); abrir o `index.html` direto do disco mostra só os dados de
demonstração; `window.PUP_API_BASE` permite apontar para outra URL de API.

### 7. Smoke

Em `ferramentas/smoke.ps1`, acrescente `"/api/portal/servicos"` e `"/api/portal/comunicados"` à lista `$publicas`
(a checagem do nome interno passa a cobrir também os dados que o front recebe).

### 8. Commit e relatório

Commit `PUP-05: front consome a API` com as alterações, o relatório e a fila.

## Como conferir (escreva no relatório o que viu)

1. API no ar (`dotnet run --project src/Pup.Api`), abra `http://localhost:5200/`.
2. F12 → Rede: `/api/portal/servicos` e `/api/portal/comunicados` com 200. Console sem erro.
3. Início: 4 cards "Serviços recentes" com os títulos do banco (sem a marca `[Exemplo]`) e 2 comunicados.
4. Coleta, Obras e Manutenção: filtros de situação e localidade funcionando; localidades `Centro`, `Estrada do Varadouro`, `Zona rural`.
5. Comunicados: filtro `Alerta` mostra a interdição do Varadouro.
6. Pare a API e abra `index.html` direto do disco: aparecem os dados de demonstração antigos, sem erro na tela.
7. Em nenhuma página, no título da aba, no código-fonte (Ctrl+U) ou no console aparece o nome interno da pasta.

## Pronto quando

- [ ] Os 7 itens de "Como conferir" ok
- [ ] Só arquivos da pasta do front e o `ferramentas/smoke.ps1` alterados (além de relatório e fila); `ORIGEM.md` intacto
- [ ] Build, testes e smoke verdes (incluindo a checagem do nome interno)
