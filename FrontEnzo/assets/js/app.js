(function (root) {
  function setupAccessibility() {
    let fontScale = 100;
    document.querySelectorAll("[data-acessib]").forEach((button) => {
      button.addEventListener("click", () => {
        const action = button.dataset.acessib;
        if (action === "imprimir") {
          window.print();
          return;
        }
        if (action === "contraste") {
          const active = document.body.classList.toggle("alto-contraste");
          button.setAttribute("aria-pressed", String(active));
          return;
        }
        if (action === "aumentar") {
          fontScale = Math.min(125, fontScale + 10);
          document.documentElement.style.fontSize = `${fontScale}%`;
          return;
        }
        if (action === "regular") {
          fontScale = 100;
          document.documentElement.style.fontSize = "";
        }
      });
    });
  }

  async function loadDynamicMenu(nav) {
    const toggle = nav.querySelector(".menu-toggle");
    const base = (root.PUP && root.PUP.apiBase) || "";
    try {
      const resp = await fetch(`${base}/api/paginas/menu`, { headers: { Accept: "application/json" } });
      if (!resp.ok) return;
      const itens = await resp.json();
      if (!Array.isArray(itens) || !itens.length) return;

      const atual = window.location.pathname.replace(/^\/+|\/+$/g, "").toLowerCase() || "index.html";
      const links = itens.map((item) => {
        const a = document.createElement("a");
        const rota = String(item.rota || "").replace(/^\/+/, "");
        a.href = `/${rota}`;
        a.textContent = item.titulo;
        if (atual === rota.toLowerCase()) a.setAttribute("aria-current", "page");
        return a;
      });

      if (document.body.dataset.page === "admin") {
        const adminLink = document.createElement("a");
        adminLink.href = "/admin.html";
        adminLink.textContent = "Administração";
        adminLink.setAttribute("aria-current", "page");
        links.push(adminLink);
      }

      nav.replaceChildren(...links);
      if (toggle) nav.append(toggle);
    } catch {
      // Mantém os links estáticos do HTML se a API estiver indisponível.
    }
  }

  async function start() {
    const menuButton = document.querySelector(".menu-toggle");
    const nav = document.querySelector("#main-nav");
    root.PUP = root.PUP || {};
    root.PUP.refreshMenu = () => (nav ? loadDynamicMenu(nav) : Promise.resolve());
    if (nav) await loadDynamicMenu(nav);
    if (menuButton && nav) {
      const menuLabel = menuButton.querySelector(".sr-only");
      menuButton.addEventListener("click", () => {
        const expanded = menuButton.getAttribute("aria-expanded") === "true";
        menuButton.setAttribute("aria-expanded", String(!expanded));
        nav.classList.toggle("is-open", !expanded);
        nav.classList.toggle("responsive", !expanded);
        if (menuLabel) menuLabel.textContent = expanded ? "Abrir menu" : "Fechar menu";
      });
    }
    setupAccessibility();
    if (root.PUP.ready) await root.PUP.ready;
    const page = document.body.dataset.page;
    if (page && root.PUP.pages[page]) root.PUP.pages[page].start();
  }

  if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", start);
  else start();
})(window);
