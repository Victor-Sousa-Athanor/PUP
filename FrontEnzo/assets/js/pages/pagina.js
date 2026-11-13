(function (root) {
  function obterRotaAtual() {
    const params = new URLSearchParams(window.location.search);
    const daQuery = params.get("rota");
    if (daQuery) return daQuery.replace(/^\/+|\/+$/g, "").toLowerCase();
    return window.location.pathname.replace(/^\/+|\/+$/g, "").toLowerCase();
  }

  function renderizarConteudo(container, texto) {
    container.replaceChildren();
    if (!texto) return;

    const blocos = texto.split(/\r?\n\r?\n+/);
    blocos.forEach((bloco) => {
      const linhas = bloco.split(/\r?\n/).map((l) => l.trim()).filter(Boolean);
      if (!linhas.length) return;

      const ehLista = linhas.every((l) => l.startsWith("- ") || l.startsWith("* "));
      if (ehLista) {
        const ul = document.createElement("ul");
        ul.style.margin = "0.75rem 0 1rem 1.25rem";
        ul.style.listStyle = "disc";
        linhas.forEach((l) => {
          const li = document.createElement("li");
          li.textContent = l.slice(2).trim();
          li.style.marginBottom = "0.35rem";
          ul.append(li);
        });
        container.append(ul);
        return;
      }

      const p = document.createElement("p");
      p.textContent = linhas.join(" ");
      p.style.marginBottom = "0.85rem";
      p.style.lineHeight = "1.6";
      container.append(p);
    });
  }

  async function start() {
    const rota = obterRotaAtual();
    const tituloEl = document.querySelector("#pagina-titulo");
    const crumbEl = document.querySelector("#pagina-breadcrumb");
    const rotaEl = document.querySelector("#pagina-rota-label");
    const resumoEl = document.querySelector("#pagina-resumo");
    const conteudoEl = document.querySelector("#pagina-conteudo");
    const fonteEl = document.querySelector("#pagina-fonte");
    const atualizadoEl = document.querySelector("#pagina-atualizado");

    if (!rota || rota === "pagina.html") {
      tituloEl.textContent = "Página não informada";
      resumoEl.textContent = "Selecione uma página válida no menu superior.";
      return;
    }

    try {
      const base = (root.PUP && root.PUP.apiBase) || "";
      const resp = await fetch(`${base}/api/paginas/por-rota?rota=${encodeURIComponent(rota)}`, {
        headers: { Accept: "application/json" }
      });
      if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
      const p = await resp.json();

      document.title = `${p.titulo} | Portal de Utilidades Públicas`;
      tituloEl.textContent = p.titulo;
      crumbEl.textContent = p.titulo;
      rotaEl.textContent = `Rota: /${p.rota}`;
      resumoEl.textContent = p.resumo || "";
      resumoEl.hidden = !p.resumo;
      renderizarConteudo(conteudoEl, p.conteudo);

      fonteEl.replaceChildren();
      if (p.fonteOficialUrl) {
        const a = document.createElement("a");
        a.className = "text-link";
        a.href = p.fonteOficialUrl;
        a.target = "_blank";
        a.rel = "noopener noreferrer";
        a.textContent = "Consultar fonte oficial";
        fonteEl.append(a);
      } else {
        fonteEl.textContent = "Conteúdo informativo cadastrado no portal";
      }

      if (p.atualizadoEm && root.PUP && root.PUP.utils) {
        atualizadoEl.textContent = `Atualizado em ${root.PUP.utils.formatDate(p.atualizadoEm)}`;
      }
    } catch {
      document.title = "Página não encontrada | Portal de Utilidades Públicas";
      tituloEl.textContent = "Página não encontrada";
      crumbEl.textContent = "Não encontrada";
      resumoEl.textContent = `A rota '/${rota}' não existe ou não está publicada no momento.`;
      conteudoEl.replaceChildren();
    }
  }

  root.PUP = root.PUP || {};
  root.PUP.pages = root.PUP.pages || {};
  root.PUP.pages.pagina = { start };
})(window);
