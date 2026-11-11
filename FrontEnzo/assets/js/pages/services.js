(function (root) {
  function start() {
    const form = document.querySelector("#service-filter");
    if (!form) return;
    const search = document.querySelector("#service-search");
    const status = document.querySelector("#service-status");
    const location = document.querySelector("#service-location");
    const results = document.querySelector("#service-results");
    const empty = document.querySelector("#service-empty");
    const count = document.querySelector("#service-count");
    const category = document.body.dataset.category;
    function update() {
      const query = root.PUP.utils.normalize(search.value);
      const filtered = root.PUP.data.services.filter((item) => {
        const searchable = root.PUP.utils.normalize(`${item.title} ${item.description} ${item.location} ${item.status}`);
        return item.category === category && (!query || searchable.includes(query)) && (!status.value || item.status === status.value) && (!location.value || item.location === location.value);
      });
      root.PUP.components.renderList(results, filtered, root.PUP.components.serviceCard);
      count.textContent = `${filtered.length} ${filtered.length === 1 ? "registro encontrado" : "registros encontrados"} · demonstração`;
      empty.hidden = filtered.length !== 0;
      results.hidden = filtered.length === 0;
    }
    form.addEventListener("submit", (event) => { event.preventDefault(); update(); });
    form.addEventListener("reset", () => window.setTimeout(update, 0));
    [search, status, location].forEach((control) => control.addEventListener(control === search ? "input" : "change", update));
    update();
  }
  root.PUP = root.PUP || {};
  root.PUP.pages = root.PUP.pages || {};
  root.PUP.pages.services = { start };
})(window);
