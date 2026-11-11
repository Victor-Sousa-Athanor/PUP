(function (root) {
  async function start() {
    const menuButton = document.querySelector(".menu-toggle");
    const nav = document.querySelector("#main-nav");
    if (menuButton && nav) {
      const menuLabel = menuButton.querySelector(".sr-only");
      menuButton.addEventListener("click", () => {
        const expanded = menuButton.getAttribute("aria-expanded") === "true";
        menuButton.setAttribute("aria-expanded", String(!expanded));
        nav.classList.toggle("is-open", !expanded);
        if (menuLabel) menuLabel.textContent = expanded ? "Abrir menu" : "Fechar menu";
      });
    }
    if (root.PUP.ready) await root.PUP.ready;
    const page = document.body.dataset.page;
    if (page && root.PUP.pages[page]) root.PUP.pages[page].start();
  }
  if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", start);
  else start();
})(window);
