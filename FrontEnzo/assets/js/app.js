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

  async function start() {
    const menuButton = document.querySelector(".menu-toggle");
    const nav = document.querySelector("#main-nav");
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
